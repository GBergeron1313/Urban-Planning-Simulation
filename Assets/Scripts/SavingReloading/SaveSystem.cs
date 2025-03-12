using System.IO;
using Citizens;
using Danny;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace SavingReloading
{
    public class SaveSystem : MonoBehaviour
    {
        private float lastSaveTime;
        private float lastLoadTime;
        private string savePath;

        const string GridDataSaveFileName = "grid_data_save.data";
        const string CitizenDataSaveFileName = "citizen_data_save.data";
        GridSystem gridSystem;
        private CreateBuilding creator;

        private void Start()
        {
            savePath = Application.persistentDataPath;
            gridSystem = FindObjectOfType<GridSystem>();
            lastSaveTime = Time.time;
            creator = FindObjectOfType<CreateBuilding>();
            // Let them load their data right on startup.
            // See LoadSaveData() for explanation.
            lastLoadTime = Time.time - 5.0f;
            Assert.IsTrue(gridSystem != null);
        }

        private void Update()
        {
            // Most of the time, they just won't be holding control,
            // so return to prevent extra work.
            if (!Input.GetKey(KeyCode.LeftControl)) return;

            // Can't save or load multiple times within 5 seconds
            bool s = Input.GetKey(KeyCode.T) && ((Time.time - lastSaveTime) > 5.0f);
            bool l = Input.GetKey(KeyCode.L) && ((Time.time - lastLoadTime) > 5.0f);

            if (s)
            {
                lastSaveTime = Time.time;
                SaveGridData();
                if (Citizen.citizens_enabled)
                    SaveCurrentCitizenData();
            }
            else if (l)
            {
                lastLoadTime = Time.time;
                LoadAllData();
            }
        }

        private void LoadAllData()
        {
            ClearCurrentData();
            LoadGridData();
            LoadCitizenData();
        }

        private void ClearCurrentData()
        {
            creator.clearBuildings();
            gridSystem.ClearGridReset();
            Building.ClearBuildings();
            Citizen.ClearCitizens();
        }

        private void SaveCurrentCitizenData()
        {
            var citizens = GameObject.FindGameObjectsWithTag("Citizens");

            Debug.LogWarning($"citizens = {citizens}, {citizens.Length}");

            string path = Path.Combine(savePath, CitizenDataSaveFileName);
            using StreamWriter writer = new StreamWriter(path, false);

            for (int i = 0; i < citizens.Length; i++)
            {
                string citizen_name = citizens[i].gameObject.name;

                // Making sure I have the right gameObject.
                if (!citizen_name.StartsWith("Citizen"))
                {
                    Debug.LogError($"citizen_name = {citizen_name}");
                }



                var nma = citizens[i].GetComponent<NavMeshAgent>();

                Vector3 pos = nma.nextPosition;
                Vector3 dest = nma.destination;
                Vector3 velocity = nma.velocity;
                int stopped = nma.isStopped ? 1 : 0;

                string s_pos = JsonUtility.ToJson(pos);
                string s_dest = JsonUtility.ToJson(dest);
                string s_velocity = JsonUtility.ToJson(velocity);
                string s_prefab_index = citizen_name.Split('_')[2];

                writer.WriteLine($"{citizen_name}={s_pos}|{s_dest}|{s_velocity}|{stopped}|{s_prefab_index}");
            }
        }

        // This is just a pile of trash, this function.
        // But it does work.
        private void LoadCitizenData()
        {

            string path = Path.Combine(savePath, CitizenDataSaveFileName);
            using StreamReader reader = new StreamReader(path);

            string[] citizen_names = new string[SpawnManager.npcCount];
            Vector3[] destinations = new Vector3[SpawnManager.npcCount];
            Vector3[] positions = new Vector3[SpawnManager.npcCount];

            SpawnManager spawner = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
            if (spawner is null)
            {
                throw new UnityException("Why was SpawnManager null when trying to load NPCs?");
            }

            for (int i = 0; i < SpawnManager.npcCount; i++)
            {
                string line = reader.ReadLine();
                if (line is null)
                {
                    throw new UnityException("Attempted to Load citizen data: Line was null");
                }

                string[] name_values = line.Split('=');
                string citizen_name = name_values[0];
                string values = name_values[1];

                // Making sure I have the right gameObject.
                Assert.IsTrue(citizen_name.StartsWith("Citizen"));

                string[] pos_dest_velocity_stopped_prefab = values.Split('|');
                Assert.IsNotNull(pos_dest_velocity_stopped_prefab);
                string output = "";
                foreach (var s in pos_dest_velocity_stopped_prefab)
                {
                    output += s;
                }

                string s_pos = pos_dest_velocity_stopped_prefab[0];
                string s_dest = pos_dest_velocity_stopped_prefab[1];
                string s_velocity = pos_dest_velocity_stopped_prefab[2];
                string s_stopped = pos_dest_velocity_stopped_prefab[3];
                string s_prefab_index = pos_dest_velocity_stopped_prefab[4];

                Vector3 pos = JsonUtility.FromJson<Vector3>(s_pos);
                Vector3 dest = JsonUtility.FromJson<Vector3>(s_dest);
                Vector3 velocity = JsonUtility.FromJson<Vector3>(s_velocity);
                int stopped = int.Parse(s_stopped);
                int prefab_index = int.Parse(s_prefab_index);

                positions[i] = pos;
                citizen_names[i] = citizen_name;
                destinations[i] = dest;
            }

            spawner.SpawnNPCsFrom(citizen_names, positions, destinations);
        }

        private void LoadGridData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            StreamReader reader = new StreamReader(path);
            string fmt = reader.ReadLine();
            Assert.AreEqual(fmt, "x,y=zone_type,cell_type");
            string[] lines = reader.ReadToEnd().Split('\n');
            reader.Dispose();

            var cells = gridSystem.GetCells();
            if (cells is null)
            {
                throw new UnityException("Couldn't access Cells from SaveSystem");
            }

            for (int i = 0; i < cells.Count; i++)
            {
                string[] cell_serial = lines[i].Split('=');
                string[] xz = cell_serial[0].Split(',');

                int x = int.Parse(xz[0]);
                int z = int.Parse(xz[1]);

                var cell = cells[i].GetComponent<Cell>();

                Assert.AreEqual(cell.location.x, x);
                Assert.AreEqual(cell.location.y, z);

                string[] zt_ct = cell_serial[1].Split(',');

                ZoneType zt = (ZoneType)int.Parse(zt_ct[0]);
                CellType ct = (CellType)int.Parse(zt_ct[1]);

                cell.SetZoneTypeAndUpdate(zt);
                cell.SetCellTypeAndUpdate(ct);

                /*switch (ct)*/
                /*{*/
                /*    case CellType.Building:*/
                /*        cell.PushBuilding();*/
                /*        /*cell.SetWalkableAndUpdate(true);*/
                /*        break;*/
                /*    case CellType.Road:*/
                /*        cell.PushRoad();*/
                /*        /*cell.SetWalkableAndUpdate(true);*/
                /*        break;*/
                /*    case CellType.None:*/
                /*        /*cell.SetWalkableAndUpdate(false);*/
                /*        break;*/
                /*    default:*/
                /*        throw new UnityException("Default shouldn't ever happen");*/
                /*}*/
            }
        }

        private void SaveGridData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine($"x,y=zone_type,cell_type");
            var cells = gridSystem.GetCells();
            if (cells is null)
            {
                throw new UnityException("Cells was null");
            }

            foreach (var building in cells)
            {
                var cell = building.GetComponent<Cell>();
                if (cell is null)
                {
                    throw new UnityException($"Cell was null on building: {building}");
                }
                int zt = (int)cell.zone_type;
                int ct = (int)cell.cell_type;
                int x = cell.location.x;
                int z = cell.location.y;
                string output =
                    $"{x},{z}={zt},{ct}";
                Debug.Log(output);
                writer.WriteLine(output);
            }
        }
    }
}
