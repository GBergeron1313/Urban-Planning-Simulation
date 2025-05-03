using System.IO;
using Citizens;
using UrbanPlanning;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using System.Collections.Generic;
using BuildingUtils;

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

        private MetricsSystem metricsSystem;

        private void Start()
        {
            savePath = Application.persistentDataPath;
            gridSystem = FindObjectOfType<GridSystem>();
            metricsSystem = FindObjectOfType<MetricsSystem>();
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

        private void SaveAllData()
        {
            lastSaveTime = Time.time;
            SaveGridData();
            if (Citizen.citizens_enabled)
                SaveCurrentCitizenData();
        }

        private void LoadAllData()
        {
            ClearCurrentData();
            LoadGridData();
            PlacementAnim.OnAllAnimsOver = () =>
            {
                LoadCitizenData();
            };
        }

        private void ClearCurrentData()
        {
            creator.clearBuildings();
            gridSystem.ClearGridReset();
            Building.ClearBuildings();
            if (Citizen.citizens_enabled)
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

        private void LoadCitizenData()
        {
            string path = Path.Combine(savePath, CitizenDataSaveFileName);
            if (!File.Exists(path))
            {
                throw new UnityException("CitizenDataSaveFileName Does not exist");
            }
            using StreamReader reader = new StreamReader(path);

            List<string> citizen_names = new List<string>();
            List<Vector3> destinations = new List<Vector3>();
            List<Vector3> positions = new List<Vector3>();

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
                    break;
                }

                string[] name_values = line.Split('=');
                string citizen_name = name_values[0];
                string values = name_values[1];

                // Making sure I have the right gameObject.
                Assert.IsTrue(citizen_name.StartsWith("Citizen"));

                string[] pos_dest_velocity_stopped_prefab = values.Split('|');
                Assert.IsNotNull(pos_dest_velocity_stopped_prefab);

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

                positions.Add(pos);
                citizen_names.Add(citizen_name);
                destinations.Add(dest);
            }

            if (positions.Count == 0
                || citizen_names.Count == 0
                || destinations.Count == 0)
            {
                Debug.LogWarning("No citizens could be spawned.");
                return;
            }

            spawner.SpawnNPCsFrom(citizen_names.ToArray(), positions.ToArray(), destinations.ToArray());
        }

        private void LoadGridData()
        {
            string exp_path = Path.Combine(savePath, "exp_grid_data.data");
            var serial_lines = File.ReadAllLines(exp_path);
            Assert.IsNotNull(serial_lines);
            Assert.IsNotNull(Cell.all_cells);
            var idx = 0;
            foreach (Cell c in Cell.all_cells)
            {
                Assert.IsNotNull(c);
                c.from_serial(serial_lines[idx]);
                idx++;
            }
        }

        private void SaveGridData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine($"x,y=zone_type,cell_type");
            using var exp_writer = File.CreateText(Path.Combine(savePath, "exp_grid_data.data"));

            foreach (Cell cell in Cell.all_cells)
            {
                exp_writer.WriteLine(cell.into_serial());

                int zt = (int)cell.zone_type;
                int ct = (int)cell.cell_type;
                int x = cell.location.x;
                int z = cell.location.y;
                int model = ((int?)cell.contents?.model) ?? 0;
                string output =
                    $"{x},{z}={zt},{ct},{model}";
                writer.WriteLine(output);
            }
        }
    }
}
