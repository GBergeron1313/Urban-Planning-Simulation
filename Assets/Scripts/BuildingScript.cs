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
    private static GridSystem grid;
    void Start()
    {
        name = "BuildingScript";
        grid = GameObject.Find("Grid").GetComponent<GridSystem>();
    }

    void Update()
    {
    }
}
