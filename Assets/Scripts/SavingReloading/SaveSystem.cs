using System.IO;
using System.Linq;
using Citizens;
using Unity.VisualScripting;
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
        const string RoadDataSaveFileName = "road_data_save.data";
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
                // SaveCurrentCitizenData();
            }
            else if (l)
            {
                lastLoadTime = Time.time;
                LoadSaveData();
                // LoadCitizenData();
            }
        }

        // private void SaveCurrentRoadData()
        // {
        //     var building = GetComponent<Building>();
        //     var roadData = building.GetPathRenderer().Serialize().json;
        //     string roadPath = Path.Combine(savePath, RoadDataSaveFileName);
        //     
        //     using StreamWriter roadWriter = new StreamWriter(roadPath);
        //     roadWriter.Write(roadData);
        // }
        //
        // private void LoadRoadData()
        // {
        //     var building = GetComponent<Building>();
        //     string roadPath = Path.Combine(savePath, RoadDataSaveFileName);
        //
        //     using StreamReader roadReader = new StreamReader(roadPath);
        //     string roadData = roadReader.ReadToEnd();
        //     SerializationData fromJson = new SerializationData(roadData);
        //     building.SetPathRenderer(fromJson.Deserialize() as LineRenderer);
        // }
        
        private void SaveCurrentCitizenData()
        {
            var building = GameObject.Find("BuildingController").GetComponent<Building>();
            var data = building.GetCitizens().Serialize().json;
            string path = Path.Combine(savePath, CitizenDataSaveFileName);
            
            using StreamWriter writer = new StreamWriter(path);
            writer.Write(data);
        }
        
        private void LoadCitizenData()
        {
            var building = GameObject.Find("BuildingController").GetComponent<Building>();
            building.RequestUpdate();
            string path = Path.Combine(savePath, CitizenDataSaveFileName);

            using StreamReader reader = new StreamReader(path);
            string data = reader.ReadToEnd();
            SerializationData fromJson = new SerializationData(data);
            building.SetCitizens(fromJson.Deserialize() as NavMeshAgent[]);
        }

        private void LoadSaveData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamReader reader = new StreamReader(path);
            string fmt = reader.ReadLine();

            Assert.AreEqual(fmt, "x,y=zone_type,is_filled");

            for (int x = 0; x < gridSystem.width; x++)
            {
                for (int z = 0; z < gridSystem.height; z++)
                {
                    string[] coordToValue = reader.ReadLine()?.Split('=');
                    if (coordToValue == null) return;

                    string[] coords = coordToValue[0].Split(',');
                    Assert.IsNotNull(coords);
                    Assert.IsTrue(coords.Length == 2);

                    int cellX = int.Parse(coords[0]);
                    int cellY = int.Parse(coords[1]);
                    Assert.AreEqual(cellX, x);
                    Assert.AreEqual(cellY, z);

                    string[] zoneTypeAndIsFilled = coordToValue[1].Split(',');
                    Assert.IsNotNull(zoneTypeAndIsFilled);
                    Assert.IsTrue(zoneTypeAndIsFilled.Length == 2);

                    ZoneType zoneType = (ZoneType)int.Parse(zoneTypeAndIsFilled[0]);
                    int isFilled = int.Parse(zoneTypeAndIsFilled[1]);
                    if (isFilled == 1)
                    {
                        Color color = gridSystem.GetZoneColor(zoneType);
                        gridSystem.fillCell(x, z);
                        creator.createBuilding(x, z, color);
                    }
                    else
                    {
                        gridSystem.emptyCell(x, z);
                    }

                    gridSystem.SetZone(x, z, zoneType);
                }
            }
        }

        private void SaveCurrent()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamWriter writer = new StreamWriter(path, false);
            writer.WriteLine($"x,y=zone_type,is_filled");
            var cells = gridSystem.GetCells();
            if (cells is null)
            {
                throw new UnityException("Cells was null");
            }

            for (int x = 0; x < gridSystem.width; x++)
            {
                for (int z = 0; z < gridSystem.height; z++)
                {
                    var zt = (int)gridSystem.GetZoneType(x, z);
                    var cellFilled = gridSystem.isCellFilled(x, z) ? 1 : 0;
                    string output =
                        $"{x},{z}={zt},{cellFilled}";
                    // Debug.Log(output);
                    writer.WriteLine(output);
                }
            }
        }
    }
}