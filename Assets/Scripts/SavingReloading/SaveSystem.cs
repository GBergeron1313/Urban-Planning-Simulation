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

        // const string RoadDataSaveFileName = "road_data_save.data";
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
                SaveCurrent();
                if (Building.citizens_enabled)
                    SaveCurrentCitizenData();
            }
            else if (l)
            {
                lastLoadTime = Time.time;
                LoadSaveData();
                // LoadCitizenData();
            }
        }

        private void SaveCurrentCitizenData()
        {
            if (!Building.citizens_enabled)
            {
                throw new UnityException("Attempted to save Citizens but citizens weren't enabled");
            }

            var citizens = GameObject.FindGameObjectsWithTag("Citizens");

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

                writer.WriteLine($"{citizen_name}={s_pos}|{s_dest}|{s_velocity}|{stopped}");
            }
        }

        private void LoadCitizenData()
        {
            string path = Path.Combine(savePath, CitizenDataSaveFileName);
            using StreamReader reader = new StreamReader(path);

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

                string[] pos_dest_velocity_stopped = values.Split('|');
                Assert.IsNotNull(pos_dest_velocity_stopped);
                print($"pos_dest_velocity_stopped.Length = {pos_dest_velocity_stopped.Length}");
                string output = "";
                foreach (var s in pos_dest_velocity_stopped)
                {
                    output += s;
                }

                print($"pos_dest_velocity_stopped.Length = {output}");

                string s_pos = pos_dest_velocity_stopped[0];
                string s_dest = pos_dest_velocity_stopped[1];
                string s_velocity = pos_dest_velocity_stopped[2];
                string s_stopped = pos_dest_velocity_stopped[3];

                Vector3 pos = JsonUtility.FromJson<Vector3>(s_pos);
                Vector3 dest = JsonUtility.FromJson<Vector3>(s_dest);
                Vector3 velocity = JsonUtility.FromJson<Vector3>(s_velocity);
                int stopped = int.Parse(s_stopped);

                print($"{citizen_name} is {stopped} stopped at {pos} heading to {dest} with velocity {velocity}");
            }
        }

        private void LoadSaveData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            StreamReader reader = new StreamReader(path);
            string fmt = reader.ReadLine();
            Assert.AreEqual(fmt, "x,y=zone_type,cell_type");
            string[] lines = reader.ReadToEnd().Split('\n');
            reader.Dispose();

            var buildings = gridSystem.GetCells();
            if (buildings is null)
            {
                throw new UnityException("Couldn't access Cells from SaveSystem");
            }

            for (int i = 0; i < buildings.Count; i++)
            {
                string[] cell_serial = lines[i].Split('=');
                string[] xz = cell_serial[0].Split(',');

                int x = int.Parse(xz[0]);
                int z = int.Parse(xz[1]);

                var cell = buildings[i].GetComponent<Cell>();

                Assert.AreEqual(cell.location.x, x);
                Assert.AreEqual(cell.location.y, z);

                string[] zt_ct = cell_serial[1].Split(',');

                ZoneType zt = (ZoneType)int.Parse(zt_ct[0]);
                CellType ct = (CellType)int.Parse(zt_ct[1]);

                cell.SetZoneTypeAndUpdate(zt);

                switch (ct)
                {
                    case CellType.Building:
                        cell.PushBuilding();
                        break;
                    case CellType.Road:
                        cell.PushRoad();
                        break;
                    case CellType.None:
                        break;
                    default:
                        throw new UnityException("Default shouldn't ever happen");
                }
            }
        }

        private void SaveCurrent()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine($"x,y=zone_type,cell_type");
            var buildings = gridSystem.GetCells();
            if (buildings is null)
            {
                throw new UnityException("Cells was null");
            }

            foreach (var building in buildings)
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
