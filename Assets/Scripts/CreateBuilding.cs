using System.Collections.Generic;
using BuildingUtils;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class CreateBuilding : MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;
    public GameObject[] building_prefabs;
    public GameObject[] road_prefabs;

    public Slider pollutionSlider;
    public Slider noiseSlider;
    public Slider capacitySlider;

    public Sprite[] buildingSprites;

    public TMP_Dropdown buildingDropdown;

    public int totalPop, totalPol, totalNoise, polPerCit, noisePerCit;
    public int buildingCount = 0;

    public TextMeshProUGUI popCount;

    // Keeping references to prefabs for later removal.
    private List<GameObject> prefabs;

    // Start is called before the first frame update
    void Start()
    {
        name = "CreateBuilding";
        prefabs = new List<GameObject>();
    }

    public void SetDropdownValues(CellType ct)
    {
        Assert.IsTrue(ct == CellType.Building || ct == CellType.Road);
        BuildingModel start, end;
        if (ct == CellType.Building)
        {
            start = (BuildingModel)(((int)BuildingModel.BUILDING_MIN + 1));
            end = BuildingModel.BUILDING_MAX;
            buildingDropdown.captionText.SetText(buildingDropdown.value.as_building_model().ToString());
        }
        else
        {
            start = (BuildingModel)(((int)BuildingModel.ROAD_MIN + 1));
            end = BuildingModel.ROAD_MAX;
            buildingDropdown.captionText.SetText(buildingDropdown.value.as_road_model().ToString());
        }
        var options = buildingDropdown.options;
        options.Clear();

        for (; start < end; start++)
        {
            options.Add(new TMP_Dropdown.OptionData(start.ToString()));
        }
    }

    private int model_as_offset(BuildingModel model, out CellType cell_type)
    {
        if (BuildingModel.BUILDING_MIN < model && model < BuildingModel.BUILDING_MAX)
        {
            cell_type = CellType.Building;
            return (int)model - ((int)BuildingModel.BUILDING_MIN + 1);
        }
        if (BuildingModel.ROAD_MIN < model && model < BuildingModel.ROAD_MAX)
        {
            cell_type = CellType.Road;
            return (int)model - ((int)BuildingModel.ROAD_MIN + 1);
        }
        Debug.LogWarning($"hydrate_model didn't catch model: {model.ToString()}");
        cell_type = CellType.None;
        return -1;
    }

    GameObject hydrate_from_model(BuildingModel model)
    {
        int idx = model_as_offset(model, out CellType which);
        Assert.AreNotEqual(idx, -1);
        Assert.AreNotEqual(which, CellType.None);

        if (which == CellType.Building)
        {
            return Instantiate(building_prefabs[idx]);
        }
        else
        {
            return Instantiate(road_prefabs[idx]);
        }
    }

    public void clearBuildings()
    {
        foreach (var prefab in prefabs)
        {
            Destroy(prefab);
        }
        Start();
    }

    private GameObject getCellTypeShape(CellType cell_type)
    {
        if (cell_type.is_road())
        {
            buildingPrefab = road_prefabs[((int)buildingDropdown.value.as_road_model())];
        }
        else if (cell_type.is_building())
        {
            buildingPrefab = building_prefabs[buildingDropdown.value];
        }

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

    private void configure_pop_info(Building bs)
    {
        if (bs.attached_to.cell_type == CellType.Building)
        {
            totalPol += (int)bs.air_pollution;
            totalNoise += (int)bs.noise_pollution;
            totalPop += (int)bs.max_capacity;
            if (totalPop > 0)
            {
                polPerCit = totalPol / totalPop;
                noisePerCit = totalNoise / totalPop;
            }
            popCount.text ="Pollution Level: " + totalPol + "\nNoise Level:" + totalNoise + "\nTotal Population: " + totalPop;
        }
    }

    public void attach_building(Cell cell, BuildingModel model)
    {
        print($"attach_building: {cell}, {model}");
        Assert.IsFalse(cell.cell_type.is_none(), "CellType can't be None when creating a building. Fix it.");
        var next_prefab = hydrate_from_model(model);
        next_prefab.transform.position = cell.gameObject.transform.position;
        Building bs = next_prefab.AddComponent<Building>();

        bs.set_model_update_info(model);
        bs.make_connection(cell);

        
        buildingCount++;
        bs.name = "Building " + buildingCount;
        bs.max_capacity = (int)capacitySlider.value;
        bs.noise_pollution = (int)noiseSlider.value;
        bs.air_pollution = (int)pollutionSlider.value;
        bs.currentSprite = buildingSprites[buildingDropdown.value];
        bs.roadModel = 11;

        configure_pop_info(bs);

        prefabs.Add(next_prefab);
    }

    /*    public void attach_building(Cell cell)*/
    /*    {*/
    /*        print($"attach_building: {cell.location}, {cell.cell_type}");*/
    /*        var next_prefab = getCellTypeShape(cell.cell_type);*/
    /*        next_prefab.transform.position = cell.gameObject.transform.position;*/
    /*        Building bs = next_prefab.AddComponent<Building>();*/
    /**/
    /*        bs.air_pollution = pollutionSlider.value + 1;*/
    /*        bs.noise_pollution = noiseSlider.value + 1;*/
    /*        bs.max_capacity = capacitySlider.value + 1;*/
    /*        bs.attached_to = cell;*/
    /**/
    /*        cell.contents = bs;*/
    /**/
    /*        if (cell.cell_type == CellType.Building)*/
    /*        {*/
    /*            totalPol += (int)bs.air_pollution;*/
    /*            totalNoise += (int)bs.noise_pollution;*/
    /*            totalPop += (int)bs.max_capacity;*/
    /*            polPerCit = totalPol / totalPop;*/
    /*            noisePerCit = totalNoise / totalPop;*/
    /*            popCount.text =*/
    /*@$"Pollution Level: {totalPol}*/
    /*Noise Level: {totalNoise}*/
    /*Total Population: {totalPop}";*/
    /*        }*/
    /**/
    /*        prefabs.Add(next_prefab);*/
    /*    }*/

    /*public GameObject createBuilding(float x, float z, Color color, ZoneType zone_type, CellType cell_type)*/
    /*{*/
    /*    print($"CreateBuilding: {x}, {z}, {color}, {zone_type}, {cell_type}");*/
    /*    var next_prefab = getCellTypeShape(cell_type);*/
    /*    if (cell_type == CellType.Building)*/
    /*    {*/
    /*        applyBuildingShapeTransformations(next_prefab, zone_type, (int)x, (int)z);*/
    /*    }*/
    /*    else if (cell_type == CellType.Road)*/
    /*    {*/
    /*        applyRoadShapeTransformations(next_prefab, zone_type, (int)x, (int)z);*/
    /*    }*/
    /**/
    /*    Building bs;*/
    /*    if (!next_prefab.TryGetComponent<Building>(out bs))*/
    /*    {*/
    /*        bs = next_prefab.AddComponent<Building>();*/
    /*    }*/
    /**/
    /*    bs.air_pollution = pollutionSlider.value;*/
    /*    bs.noise_pollution = noiseSlider.value;*/
    /*    bs.max_capacity = capacitySlider.value;*/
    /**/
    /*    prefabs.Add(next_prefab);*/
    /**/
    /*    return next_prefab;*/
    /*}*/
    public void checkBuildingType(GameObject buildingPrefab)
    {
        buildingPrefab = building_prefabs[buildingDropdown.value];
    }
}
