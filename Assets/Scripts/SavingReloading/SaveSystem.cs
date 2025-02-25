using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace SavingReloading
{
    public class SaveSystem : MonoBehaviour
    {
        private float lastSaveTime;
        private float lastLoadTime;
        private string savePath;
        const string GridDataSaveFileName = "grid_data_save.data";
        GridSystem gridSystem;
        private CreateBuilding buildingCreator;


        private void Start()
        {
            savePath = Application.persistentDataPath;
            gridSystem = FindObjectOfType<GridSystem>();
            lastSaveTime = Time.time;
            buildingCreator = FindObjectOfType<CreateBuilding>();
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
            }
            else if (l)
            {
                lastLoadTime = Time.time;
                LoadSaveData();
            }
        }

        private void LoadSaveData()
        {
            string path = Path.Combine(savePath, GridDataSaveFileName);
            using StreamReader reader = new StreamReader(path);
            string fmt = reader.ReadLine();

            Assert.AreEqual(fmt, "x,y=zone_type,is_filled");

            foreach (var z in Enumerable.Range(0, gridSystem.height))
            {
                foreach (var x in Enumerable.Range(0, gridSystem.width))
                {
                    string[] coordToValue = reader.ReadLine()?.Split('=');
                    if (coordToValue == null) return;
                    // Debug.Log($"coordToValue = \"{coordToValue[0]}, {coordToValue[1]}\"");

                    string[] coords = coordToValue[0].Split(',');
                    Assert.IsNotNull(coords);
                    Assert.IsTrue(coords.Length == 2);
                    // Debug.Log($"coords = \"{coords[0]}, {coords[1]}\"");
                    
                    int cellX = int.Parse(coords[0]);
                    int cellY = int.Parse(coords[1]);
                    Assert.AreEqual(cellX, x);
                    Assert.AreEqual(cellY, z);

                    string[] zoneTypeAndIsFilled = coordToValue[1].Split(',');
                    Assert.IsNotNull(zoneTypeAndIsFilled);
                    Assert.IsTrue(zoneTypeAndIsFilled.Length == 2);
                    // Debug.Log($"zoneTypeAndIsFilled = \"{zoneTypeAndIsFilled[0]}, {zoneTypeAndIsFilled[1]}\"");
                    
                    ZoneType zoneType = (ZoneType)int.Parse(zoneTypeAndIsFilled[0]);
                    int isFilled = int.Parse(zoneTypeAndIsFilled[1]);
                    if (isFilled == 1)
                    {
                        gridSystem.fillCell(x, z);
                        buildingCreator.createBuilding(x, z);
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
            foreach (UInt32 z in Enumerable.Range(0, gridSystem.height))
            {
                foreach (UInt32 x in Enumerable.Range(0, gridSystem.width))
                {
                    var zoneType = (int)gridSystem.GetZoneType((int)x, (int)z);
                    int cellFilled = gridSystem.isCellFilled((int)x, (int)z) ? 1 : 0;
                    string output =
                        $"{x},{z}={zoneType},{cellFilled}";
                    writer.WriteLine(output);
                }
            }
        }
    }
}