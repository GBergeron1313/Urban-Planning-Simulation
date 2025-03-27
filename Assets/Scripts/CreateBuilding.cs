using System.Collections.Generic;
using Citizens;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CreateBuilding : MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;
    public GameObject[] prefabList;

    public Slider pollutionSlider;
    public Slider noiseSlider;
    public Slider capacitySlider;

    public TMP_Dropdown buildingDropdown;

    public int totalPop;

    public TextMeshProUGUI popCount;

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

    /*public void createBuilding()*/
    /*{*/
    /*    print("Building Created");*/
    /*    var nextBuilding = Instantiate(buildingPrefab);*/
    /*    prefabs.Add(nextBuilding);*/
    /*    nextBuilding.transform.position = new Vector3(6.0f, 0.5f, 0.0f);*/
    /*}*/

    private GameObject getCellTypeShape(CellType cell_type)
    {
        buildingPrefab = prefabList[buildingDropdown.value];

        switch (cell_type)
        {
            case CellType.Building:
                return Instantiate(buildingPrefab);
            case CellType.Road:
                GameObject ret = GameObject.CreatePrimitive(PrimitiveType.Cube);
                ret.gameObject.transform.localScale = new Vector3(1, 0.1f, 1);
                return ret;
            default:
                throw new UnityException($"CellType {cell_type} unexpected");
        }
    }

    private void applyRoadShapeTransformations(GameObject road, ZoneType zone_type, int x, int z)
    {
        road.transform.Translate(Vector3.down * 2);
        road.transform.localScale.Set(1.0f, 0.1f, 1.0f);

        /*Cell c;*/
        /*if (!road.TryGetComponent<Cell>(out c))*/
        /*{*/
        /*    Debug.Log("Cell wasn't attached. Attaching...");*/
        /**/
        /*    c = road.AddComponent<Cell>();*/
        /*}*/
        /*c.location = new Vector2Int((int)x, (int)z);*/
        /*c.cell_type = CellType.Road;*/
        /*c.SetZoneTypeAndUpdate(zone_type);*/
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

    public void attach_building(Cell cell)
    {

        print($"attach_building: {cell.location}, {cell.cell_type}");
        var next_prefab = getCellTypeShape(cell.cell_type);
        next_prefab.transform.position = cell.gameObject.transform.position;
        Building bs = next_prefab.AddComponent<Building>();

        bs.air_pollution = pollutionSlider.value;
        bs.noise_pollution = noiseSlider.value;
        bs.max_capacity = capacitySlider.value;
        bs.building_type = cell.zone_type;
        bs.attached_to = cell;

        cell.contents = bs;

        totalPop += (int)bs.max_capacity;
        popCount.text = "Total Population: " + totalPop;

        prefabs.Add(next_prefab);
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

        Building bs;
        if (!next_prefab.TryGetComponent<Building>(out bs))
        {
            bs = next_prefab.AddComponent<Building>();
        }

        bs.air_pollution = pollutionSlider.value;
        bs.noise_pollution = noiseSlider.value;
        bs.max_capacity = capacitySlider.value;
        bs.building_type = zone_type;

        prefabs.Add(next_prefab);

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

    public void checkBuildingType(GameObject buildingPrefab)
    {
        buildingPrefab = prefabList[buildingDropdown.value];
    }
}
