using UnityEngine;

public class CreateBuilding : MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        name = "CreateBuilding";
    }

    // Update is called once per frame

    public void createBuilding()
    {
        print("Building Created");
        var nextBuilding = Instantiate(buildingPrefab);
        nextBuilding.transform.position = new Vector3(6.0f, 0.5f, 0.0f);
        nextBuilding.GetComponent<BuildingScript>().mainGrid = MainGrid;
    }

    public void createBuilding(float x, float z, Color color, ZoneType zone_type)
    {
        print($"Building Being placed at {x}, {z}");
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