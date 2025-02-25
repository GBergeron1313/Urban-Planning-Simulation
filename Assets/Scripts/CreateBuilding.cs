using UnityEngine;

public class CreateBuilding: MonoBehaviour
{
    public GameObject MainGrid;

    public GameObject buildingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createBuilding()
    {
        print("Building Created"); 
        var nextBuilding = Instantiate(buildingPrefab);
        nextBuilding.transform.position = new Vector3(6.0f, 0.5f, 0.0f);
        nextBuilding.GetComponent<BuildingScript>().mainGrid = MainGrid;        
    }

    public void createBuilding(float x, float z)
    {
        print($"Building Being placed at {x}, {z}");
        var nextBuilding = Instantiate(buildingPrefab);
        nextBuilding.transform.position = new Vector3(x - 4.5f, 0.5f, z - 4.5f);
        nextBuilding.GetComponent<BuildingScript>().mainGrid = MainGrid;
        nextBuilding.GetComponent<BuildingScript>().checkBuildingColor((int)x,(int)z);
    }
}
