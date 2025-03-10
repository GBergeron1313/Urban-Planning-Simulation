using System.Collections.Generic;
using Citizens;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public enum ZoneType
{
    None,
    Residential,
    Commercial,
    Industrial,
    Restricted
}

public enum BuildingMode
{
    None,
    PlacingBuilding,
    PlacingRoad,
    MarkingZoneType,

    // Always keep TotalModes at the end
    TotalModes
}


public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")] public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float gridHeight = 0f;
    public Material gridMaterial;

    [Header("Zone Settings")] public Color residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public Color commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public Color industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public Color restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    [Header("Interaction Settings")] public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;

    private GameObject[,] gridCells;
    private ZoneType[,] zoneGrid;
    private Material defaultMaterial;
    private GameObject lastHovered;
    private GameObject selectedCell;
    private bool[,] filledCells;

    public GameObject uiText;

    public static readonly Color g_residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public static readonly Color g_commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public static readonly Color g_industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public static readonly Color g_restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    public static Color g_defaultColor = Color.green;
    public static readonly Color g_hoverColor = Color.yellow;

    private Button BTN_change_building_mode;

    void Awake()
    {
        BTN_change_building_mode = GameObject.Find("ModeButton").GetComponent<Button>();
        BTN_change_building_mode.onClick.AddListener(Cell.CycleBuildingMode);
    }

    /// Initializes the grid arrays and generates the grid structure
    void Start()
    {
        // Initialize arrays to store grid cells, zone types, and occupancy status
        gridCells = new GameObject[width, height];
        zoneGrid = new ZoneType[width, height];
        filledCells = new bool[width, height];

        // Initialize all cells as empty and with no zone type
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                filledCells[x, z] = false;
                zoneGrid[x, z] = ZoneType.None;
            }
        }

        GenerateGrid();
    }

    public void ClearGridReset()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Don't want dangling prefabs
                GameObject.Destroy(gridCells[x, z]);
            }
        }
        Cell.all_cells.Clear();
        Start();
    }


    /// Creates the visual grid structure with cell objects and colliders
    void GenerateGrid()
    {
        // Create a parent object to organize all grid cells
        GameObject gridParent = new GameObject("Grid");
        gridParent.transform.parent = transform;

        Cell.grid = this;

        // Calculate starting position to center the grid
        Vector3 startPos = transform.position - new Vector3(width * cellSize / 2f, 0, height * cellSize / 2f);

        // Create individual grid cells
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Create a quad primitive for the cell
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.name = $"Cell_{x}_{z}"; // Name format used for position lookup
                cell.transform.parent = gridParent.transform;

                // Position the cell in the grid
                Vector3 pos = startPos + new Vector3(x * cellSize, gridHeight, z * cellSize);
                cell.transform.position = pos;
                cell.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to lay flat
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);

                // Apply material if provided
                if (gridMaterial != null)
                {
                    Material cellMaterial = new Material(gridMaterial);
                    cell.GetComponent<Renderer>().material = cellMaterial;
                }

                // Add thin collider for mouse interaction
                BoxCollider collide = cell.AddComponent<BoxCollider>();
                // The name "collider" was causing conflicts
                collide.size = new Vector3(1, 1, 0.1f);

                // Store reference to cell
                gridCells[x, z] = cell;

                Cell c = cell.AddComponent<Cell>();

                c.location = new Vector2Int(x, z);
                c.SetZoneTypeAndUpdate(ZoneType.None);
                c.SetWalkableAndUpdate(false);
            }
        }

        // Store reference to default material for color restoration
        if (gridCells[0, 0] != null)
        {
            defaultMaterial = new Material(gridCells[0, 0].GetComponent<Renderer>().material);
        }
    }


    /// Updates grid state each frame
    public void Update()
    {
        HandleGridInteraction();
    }

    void HandleGridInteraction()
    {
        // Cycles between PlacingBuilding, MarkingZoneType, and None
        var e = Input.GetKeyDown(KeyCode.E);

        // Cycles paintbrush settings for ZoneType assignment
        bool r = Input.GetKeyDown(KeyCode.R);

        if (e)
        {
            Cell.CycleBuildingMode();
        }

        if (r)
        {
            Cell.CycleZoneType();
        }

        uiText.GetComponent<TMPro.TextMeshProUGUI>().text =
            $"Simulation Time: {SimCore.Instance.simulationClock}\n" +
            $"Cell: {(Cell.hovering is null ? "N/A" : Cell.hovering.location)}\n" +
            $"Zone Type: {(Cell.hovering is null ? "N/A" : Cell.hovering.zone_type)}\n" +
            $"Paintbrush: {Cell.paintbrush}\n" +
            $"Building Placement Mode: {Cell.building_mode}\n" +
            $"Number of Buildings: {Building.building_positions.Count}";

    }

    public List<GameObject> GetBuildings()
    {
        if (filledCells is null || gridCells is null) return null;
        List<GameObject> buildings = new List<GameObject>();



        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                if (filledCells[x, z])
                {
                    buildings.Add(gridCells[x, z]);
                }
            }
        }

        return buildings;
    }

    public List<GameObject> GetCells()
    {
        if (filledCells is null || gridCells is null) return null;
        Assert.AreEqual(filledCells.GetLength(0), width);
        Assert.AreEqual(filledCells.GetLength(1), height);
        List<GameObject> cells = new List<GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                cells.Add(gridCells[x, z]);
            }
        }

        return cells;
    }


    /// Assigns a zone type to a grid cell
    /*public void SetZone(int x, int z, ZoneType zoneType)*/
    /*{*/
    /*    zoneGrid[x, z] = zoneType;*/
    /*}*/


    /// Updates the cell color based on its zone type
    /*public void UpdateCellColor(GameObject cell, int x, int z)*/
    /*{*/
    /*    Color zoneColor = GetZoneColor(zoneGrid[x, z]);*/
    /*    cell.GetComponent<Renderer>().material.color = zoneColor;*/
    /*}*/


    /// Returns the color associated with a zone type
    /*public Color GetZoneColor(ZoneType zoneType)*/
    /*{*/
    /*    switch (zoneType)*/
    /*    {*/
    /*        case ZoneType.Residential: return residentialZoneColor;*/
    /*        case ZoneType.Commercial: return commercialZoneColor;*/
    /*        case ZoneType.Industrial: return industrialZoneColor;*/
    /*        case ZoneType.Restricted: return restrictedZoneColor;*/
    /*        default: return defaultMaterial.color;*/
    /*    }*/
    /*}*/

    public static Color ZoneColor(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Residential: return g_residentialZoneColor;
            case ZoneType.Commercial: return g_commercialZoneColor;
            case ZoneType.Industrial: return g_industrialZoneColor;
            case ZoneType.Restricted: return g_restrictedZoneColor;
            default: return g_defaultColor;
        }
    }

    ///Returns the Zone type of a cell
    public ZoneType GetZoneType(int x, int z)
    {
        return zoneGrid[x, z];
    }


    /// Returns the GameObject at the specified grid coordinates
    public GameObject GetCellAt(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridCells[x, z];
        }

        return null;
    }

    /// Checks if a cell is filled or restricted
    /// Returns true if cell is filled, restricted, or out of bounds
    public bool isCellFilled(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height && zoneGrid[x, z] != ZoneType.Restricted)
        {
            return filledCells[x, z];
        }

        return true; // Return true for out of bounds or restricted zones to prevent building
    }

    /// Marks a cell as filled if it's within grid bounds
    public void fillCell(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            filledCells[x, z] = true;
        }
        else
        {
            throw new UnityException($"Attempted to Fill Cell {x}, {z}");
        }
    }

    /// Marks a cell as empty if you move somthing off the cell
    public void emptyCell(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            filledCells[x, z] = false;
        }
    }
}
