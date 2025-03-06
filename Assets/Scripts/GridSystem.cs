using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

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

    private BuildingMode currentMode;

    public static readonly Color g_residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public static readonly Color g_commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public static readonly Color g_industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public static readonly Color g_restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    public static Color g_defaultColor = Color.green;
    public static readonly Color g_hoverColor = Color.yellow;

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
                GameObject.Destroy(gridCells[x, z].GetComponent<Cell>());
                GameObject.Destroy(gridCells[x, z]);
            }
        }
        Start();
    }


    /// Creates the visual grid structure with cell objects and colliders
    void GenerateGrid()
    {
        // Create a parent object to organize all grid cells
        GameObject gridParent = new GameObject("Grid");
        gridParent.transform.parent = transform;

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

                Cell c = cell.AddComponent<Cell>();
                c.location = new Vector2Int(x, z);
                c.zone_type = ZoneType.None;
                c.grid = this;

                // Store reference to cell
                gridCells[x, z] = cell;
            }
        }

        // Store reference to default material for color restoration
        if (gridCells[0, 0] != null)
        {
            defaultMaterial = new Material(gridCells[0, 0].GetComponent<Renderer>().material);
            Cell.default_color = defaultMaterial.color;
            g_defaultColor = defaultMaterial.color;
        }
    }


    /// Updates grid state each frame
    public void Update()
    {
        UpdateBuildingMode();
        HandleGridInteraction();
    }

    private void UpdateBuildingMode()
    {
        // Toggles between PlacingBuilding and MarkingZoneType
        var e = Input.GetKeyDown(KeyCode.E);

        if (e)
        {
            currentMode++;
            if (currentMode >= BuildingMode.TotalModes)
            {
                currentMode = BuildingMode.None;
            }

            Debug.Log($"Switched to {currentMode}");
        }
    }


    void HandleGridInteraction()
    {
        if (!Cell.hovering) return;
        GameObject hitObject = Cell.hovering.gameObject;
        int x = Cell.hovering.location.x;
        int z = Cell.hovering.location.y;

        // Toggles between PlacingBuilding and MarkingZoneType
        var e = Input.GetKeyDown(KeyCode.E);

        // Cycles paintbrush settings
        bool r = Input.GetKeyDown(KeyCode.R);

        if (e)
        {
            Cell.building_mode++;
            if (Cell.building_mode >= BuildingMode.TotalModes)
            {
                Cell.building_mode = BuildingMode.None;
            }
        }

        if (r)
        {
            print($"Paintbrush = {Cell.paintbrush}");
            Cell.paintbrush++;
            if (Cell.paintbrush > ZoneType.Restricted)
            {
                Cell.paintbrush = ZoneType.Residential;
            }
        }

        Vector2Int loc;

        if (Cell.hovering is null)
        {
            loc = Cell.last_hovered?.location ?? new Vector2Int(-1, -1);
        }
        else
        {
            loc = Cell.hovering.location;
        }

        ZoneType zt_displayed = Cell.hovering?.zone_type ?? Cell.last_hovered?.zone_type ?? ZoneType.None;

        uiText.GetComponent<TMPro.TextMeshProUGUI>().text =
            $"Cell ({loc.x},{loc.y})\n" +
            $"Zone Type: {zt_displayed}\n" +
            $"Paintbrush: {Cell.paintbrush}\n" +
            $"Building Placement Mode: {Cell.building_mode}";


        // bool mouseLeftDown = Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
        // bool mouseLeftDragging = Input.GetMouseButton((int)MouseButton.LeftMouse);
        // Color nextColor = GetZoneColor(zoneGrid[x, z]);

        // switch (currentMode)
        // {
        //     case BuildingMode.MarkingZoneType:
        //         if (mouseLeftDragging)
        //         {
        //             if (selectedCell)
        //             {
        //                 UpdateCellColor(selectedCell);
        //             }
        //
        //             selectedCell = hitObject;
        //
        //             Color blendedSelectColor = Color.Lerp(nextColor, selectedColor, 0.5f);
        //             selectedCell.GetComponent<Renderer>().material.color = blendedSelectColor;
        //         }
        //
        //         break;
        //
        //     case BuildingMode.PlacingBuilding:
        //         if (mouseLeftDown)
        //         {
        //             GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>().createBuilding(
        //                 x,
        //                 z,
        //                 nextColor
        //             );
        //             fillCell(x, z);
        //         }
        //
        //         break;
        //
        //     default:
        //     case BuildingMode.None:
        //         break;
        // }

        // // Handle selection
        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (selectedCell != null)
        //     {
        //         UpdateCellColor(selectedCell);
        //     }
        //
        //     selectedCell = hitObject;
        //     Color baseColor = GetZoneColor(zoneGrid[x, z]);
        //     Color blendedSelectColor = Color.Lerp(baseColor, selectedColor, 0.5f);
        //     selectedCell.GetComponent<Renderer>().material.color = blendedSelectColor;
        // }
        // if (lastHovered != null && lastHovered != selectedCell)
        // {
        //     UpdateCellColor(lastHovered);
        //     lastHovered = null;
        // }
    }

    /// Handles mouse interaction with the grid including hover effects and zone assignment
    // void HandleGridInteraction()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     RaycastHit hit;
    //
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         GameObject hitObject = hit.collider.gameObject;
    //         string[] coordinates = hitObject.name.Split('_');
    //
    //         if (coordinates.Length >= 3)
    //         {
    //             int x = int.Parse(coordinates[1]);
    //             int z = int.Parse(coordinates[2]);
    //
    //             // Handle hover effect
    //             if (lastHovered != hitObject)
    //             {
    //                 if (lastHovered != null && lastHovered != selectedCell)
    //                 {
    //                     UpdateCellColor(lastHovered);
    //                 }
    //
    //                 if (hitObject != selectedCell)
    //                 {
    //                     Color baseColor = GetZoneColor(zoneGrid[x, z]);
    //                     Color blendedHoverColor = Color.Lerp(baseColor, hoverColor, 0.5f);
    //                     hitObject.GetComponent<Renderer>().material.color = blendedHoverColor;
    //                     uiText.GetComponent<TMPro.TextMeshProUGUI>().text =
    //                         "Cell (" + x + "," + z + ")\nZone Type: " + zoneGrid[x, z];
    //                 }
    //
    //                 lastHovered = hitObject;
    //             }
    //
    //             // Zone assignment
    //             if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
    //             {
    //                 if (Input.GetKeyDown(KeyCode.Alpha1)) SetZone(x, z, ZoneType.Residential);
    //                 else if (Input.GetKeyDown(KeyCode.Alpha2)) SetZone(x, z, ZoneType.Commercial);
    //                 else if (Input.GetKeyDown(KeyCode.Alpha3)) SetZone(x, z, ZoneType.Industrial);
    //                 else if (Input.GetKeyDown(KeyCode.Alpha4)) SetZone(x, z, ZoneType.Restricted);
    //                 else if (Input.GetKeyDown(KeyCode.Alpha0)) SetZone(x, z, ZoneType.None);
    //             }
    //
    //             bool mouseLeftDown = Input.GetMouseButtonDown((int)MouseButton.LeftMouse);
    //             bool mouseLeftDragging = Input.GetMouseButton((int)MouseButton.LeftMouse);
    //             Color nextColor = GetZoneColor(zoneGrid[x, z]);
    //
    //             switch (currentMode)
    //             {
    //                 case BuildingMode.MarkingZoneType:
    //                     if (mouseLeftDragging)
    //                     {
    //                         if (selectedCell != null)
    //                         {
    //                             UpdateCellColor(selectedCell);
    //                         }
    //
    //                         selectedCell = hitObject;
    //
    //                         Color blendedSelectColor = Color.Lerp(nextColor, selectedColor, 0.5f);
    //                         selectedCell.GetComponent<Renderer>().material.color = blendedSelectColor;
    //                     }
    //
    //                     break;
    //
    //                 case BuildingMode.PlacingBuilding:
    //                     if (mouseLeftDown)
    //                     {
    //                         GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>().createBuilding(
    //                             x,
    //                             z,
    //                             nextColor
    //                         );
    //                         fillCell(x, z);
    //                     }
    //
    //                     break;
    //
    //                 default:
    //                 case BuildingMode.None:
    //                     break;
    //             }
    //
    //             // // Handle selection
    //             // if (Input.GetMouseButtonDown(0))
    //             // {
    //             //     if (selectedCell != null)
    //             //     {
    //             //         UpdateCellColor(selectedCell);
    //             //     }
    //             //
    //             //     selectedCell = hitObject;
    //             //     Color baseColor = GetZoneColor(zoneGrid[x, z]);
    //             //     Color blendedSelectColor = Color.Lerp(baseColor, selectedColor, 0.5f);
    //             //     selectedCell.GetComponent<Renderer>().material.color = blendedSelectColor;
    //             // }
    //         }
    //     }
    //     else if (lastHovered != null && lastHovered != selectedCell)
    //     {
    //         UpdateCellColor(lastHovered);
    //         lastHovered = null;
    //     }
    // }

    [CanBeNull]
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

    [CanBeNull]
    public List<GameObject> GetCells()
    {
        if (filledCells is null || gridCells is null) return null;
        Assert.AreEqual(filledCells.GetLength(0), width);
        Assert.AreEqual(filledCells.GetLength(1), height);
        List<GameObject> buildings = new List<GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                buildings.Add(gridCells[x, z]);
            }
        }

        return buildings;
    }


    /// Assigns a zone type to a grid cell
    public void SetZone(int x, int z, ZoneType zoneType)
    {
        zoneGrid[x, z] = zoneType;
    }


    /// Updates the cell color based on its zone type
    public void UpdateCellColor(GameObject cell, int x, int z)
    {
        Color zoneColor = GetZoneColor(zoneGrid[x, z]);
        cell.GetComponent<Renderer>().material.color = zoneColor;
    }


    /// Returns the color associated with a zone type
    public Color GetZoneColor(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Residential: return residentialZoneColor;
            case ZoneType.Commercial: return commercialZoneColor;
            case ZoneType.Industrial: return industrialZoneColor;
            case ZoneType.Restricted: return restrictedZoneColor;
            default: return defaultMaterial.color;
        }
    }

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

    // public void InvalidateCells()
    // {
    //     for (int i = 0; i < width; i++)
    //     {
    //         for (int j = 0; j < height; j++)
    //         {
    //             bool result = isCellFilled(i, j);
    //
    //             if (result)
    //             {
    //                 // if cell is filled, update cell color
    //                 UpdateCellColor(gridCells[i, j], x, z);
    //
    //                 Debug.Log("Grid Updated");
    //             }
    //         }
    //     }
    // }


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
