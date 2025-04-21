using UnityEngine;

public enum BuildingType
{
    None,
    Residential,
    Commercial,
    Industrial,
}

public class BuildingScript : MonoBehaviour
{

    public GameObject[] prefabs;
    public int prefab_Number;

    private static GridSystem grid;
    void Start()
    {
        name = "BuildingScript";
        grid = GameObject.Find("Grid").GetComponent<GridSystem>();
        prefab_Number = 0;
    }

    void Update()
    {
        
    }
}
