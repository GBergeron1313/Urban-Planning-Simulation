using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateBuilding : MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;
    public GameObject commercialPrefab;
    public GameObject residentialPrefab;
    public GameObject industrialPrefab;

    public Slider pollutionSlider;
    public Slider noiseSlider;
    public Slider capacitySlider;

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

    private GameObject getCellTypeShape(CellType cell_type)
    {
        switch (cell_type)
        {
            case CellType.Building:
                return Instantiate(buildingPrefab);
            case CellType.Road:
                return GameObject.CreatePrimitive(PrimitiveType.Cube);
            default:
                throw new UnityException($"CellType {cell_type} unexpected");
        }
    }

    private void applyRoadShapeTransformations(GameObject road, ZoneType zone_type, int x, int z)
    {
        road.transform.position = new Vector3(x - 4.5f, 0f, z - 4.5f);
        road.transform.Translate(new Vector3(0f, 0.05f, 0f));
        road.transform.localScale = new Vector3(1.0f, 0.1f, 1.0f);


        Cell c;
        if (!road.TryGetComponent<Cell>(out c))
        {
            Debug.Log("Cell wasn't attached. Attaching...");

            c = road.AddComponent<Cell>();
        }
        c.location = new Vector2Int((int)x, (int)z);
        /*Cell.grid = GameObject.Find("Grid").GetComponent<GridSystem>();*/
        c.cell_type = CellType.Road;
        c.SetZoneTypeAndUpdate(zone_type);
    }

    private void applyBuildingShapeTransformations(GameObject nextBuilding, ZoneType zone_type, int x, int z)
    {
        nextBuilding.transform.position = new Vector3(x - 4.5f, 0.5f, z - 4.5f);
        Cell c;
        if (!nextBuilding.TryGetComponent<Cell>(out c))
        {
            Debug.Log("Cell wasn't attached. Attaching...");

            c = nextBuilding.AddComponent<Cell>();
        }
        c.location = new Vector2Int((int)x, (int)z);
        /*c.grid = GameObject.Find("Grid").GetComponent<GridSystem>();*/
        c.cell_type = CellType.Building;
        c.SetZoneTypeAndUpdate(zone_type);
    }

    public void destroyBuilding(GameObject go)
    {
        var g = prefabs.Find((GameObject g) => { return Object.ReferenceEquals(g, go); });
        if (g is null) return;
        prefabs.Remove(g);
        Destroy(g);
    }

    public GameObject createBuilding(float x, float z, Color color, ZoneType zone_type, CellType cell_type)
    {
        print($"CreateBuilding: {x}, {z}, {color}, {zone_type}, {cell_type}");
        var next_prefab = getCellTypeShape(cell_type);
        if (cell_type == CellType.Building)
        {
            applyBuildingShapeTransformations(next_prefab, zone_type, (int)x, (int)z);
        }
        else if (cell_type == CellType.Road)
        {
            applyRoadShapeTransformations(next_prefab, zone_type, (int)x, (int)z);
        }
        prefabs.Add(next_prefab);
        next_prefab.GetComponent<BuildingScript>().Pollution = pollutionSlider.value;
        next_prefab.GetComponent<BuildingScript>().NoisePollution = noiseSlider.value;
        next_prefab.GetComponent<BuildingScript>().MaxCapacity = capacitySlider.value;
        if(zone_type == ZoneType.Commercial)
        {
            print("Commercial Building Placed");
            next_prefab.GetComponent<BuildingScript>().bType = BuildingType.Commercial;
        }
        else if (zone_type == ZoneType.Residential)
        {
            print("Residential Building Placed");
            next_prefab.GetComponent<BuildingScript>().bType = BuildingType.Residential;
        }
        else if (zone_type == ZoneType.Industrial)
        {
            print("Industrial Building Placed");
            next_prefab.GetComponent<BuildingScript>().bType = BuildingType.Industrial;
        }
        return next_prefab;
    }

    public void createBuilding(float x, float z, Color color, ZoneType zone_type)
    {
        print($"CreateBuilding: {x}, {z}, {color}, {zone_type}");
        var nextBuilding = Instantiate(buildingPrefab);
        nextBuilding.transform.position = new Vector3(x - 4.5f, 0.5f, z - 4.5f);
        Cell c = nextBuilding.AddComponent<Cell>();
        c.color = color;
        c.location = new Vector2Int((int)x, (int)z);
        c.zone_type = zone_type;
        c.GetComponent<Renderer>().material.color = color;
    }
}
