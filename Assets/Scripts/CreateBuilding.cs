using System.Collections.Generic;
using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;

    // Keeping references to prefabs for later removal.
    private List<GameObject> prefabs;

    // Start is called before the first frame update
    void Start()
    {
        name = "CreateBuilding";
        prefabs = new List<GameObject>();
    }

    public void clearBuildings()
    {
        foreach (var prefab in prefabs)
        {
            Destroy(prefab);
        }
        Start();
    }

    public void createBuilding()
    {
        print("Building Created");
        var nextBuilding = Instantiate(buildingPrefab);
        prefabs.Add(nextBuilding);
        nextBuilding.transform.position = new Vector3(6.0f, 0.5f, 0.0f);
        nextBuilding.GetComponent<BuildingScript>().mainGrid = MainGrid;
    }

    public void createBuilding(float x, float z, Color color, ZoneType zone_type, CellType cell_type)
    {
        print($"CreateBuilding: {x}, {z}, {color}, {zone_type}, {cell_type}");
        var nextBuilding = Instantiate(buildingPrefab);
        prefabs.Add(nextBuilding);
        nextBuilding.transform.position = new Vector3(x - 4.5f, 0.5f, z - 4.5f);
        Cell c;
        if (!nextBuilding.TryGetComponent<Cell>(out c))
        {
            Debug.Log("Cell wasn't attached. Attaching...");

            c = nextBuilding.AddComponent<Cell>();
        }
        /*c.color = GridSystem.ZoneColor(zone_type);*/
        c.location = new Vector2Int((int)x, (int)z);
        c.grid = GameObject.Find("Grid").GetComponent<GridSystem>();
        /*c.zone_type = zone_type;*/
        c.cell_type = cell_type;
        c.SetZoneTypeAndUpdate(zone_type);
        /*c.GetComponent<Renderer>().material.color = color;*/
    }

    public void createBuilding(float x, float z, Color color, ZoneType zone_type)
    {
        print($"CreateBuilding: {x}, {z}, {color}, {zone_type}");
        var nextBuilding = Instantiate(buildingPrefab);
        nextBuilding.transform.position = new Vector3(x - 4.5f, 0.5f, z - 4.5f);
        Cell c = nextBuilding.AddComponent<Cell>();
        c.color = color;
        c.location = new Vector2Int((int)x, (int)z);
        c.grid = MainGrid.GetComponent<GridSystem>();
        c.zone_type = zone_type;
        c.GetComponent<Renderer>().material.color = color;
    }
}
