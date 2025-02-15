using System;
using UnityEngine;

public enum ZoneType
{
    None,
    Residential,
    Commercial,
    Industrial,
    Restricted
}

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float gridHeight = 0f;
    public Material gridMaterial;

    [Header("Zone Settings")]
    public Color residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public Color commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public Color industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public Color restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    [Header("Interaction Settings")]
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;

    private GameObject[,] gridCells;
    private ZoneType[,] zoneGrid;
    private Material defaultMaterial;
    private GameObject lastHovered;
    private GameObject selectedCell;
    private bool[,] filledCells;

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
                cell.name = $"Cell_{x}_{z}";  // Name format used for position lookup
                cell.transform.parent = gridParent.transform;

                // Position the cell in the grid
                Vector3 pos = startPos + new Vector3(x * cellSize, gridHeight, z * cellSize);
                cell.transform.position = pos;
                cell.transform.rotation = Quaternion.Euler(90, 0, 0);  // Rotate to lay flat
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);

                // Apply material if provided
                if (gridMaterial != null)
                {
                    Material cellMaterial = new Material(gridMaterial);
                    cell.GetComponent<Renderer>().material = cellMaterial;
                }

                // Add thin collider for mouse interaction
                BoxCollider collider = cell.AddComponent<BoxCollider>();
                collider.size = new Vector3(1, 1, 0.1f);

                // Store reference to cell
                gridCells[x, z] = cell;
            }
        }

        // Store reference to default material for color restoration
        if (gridCells[0, 0] != null)
        {
            defaultMaterial = new Material(gridCells[0, 0].GetComponent<Renderer>().material);
        }
    }


    /// Updates grid state each frame

    void Update()
    {
        HandleGridInteraction();
    }


    /// Handles mouse interaction with the grid including hover effects and zone assignment

    void HandleGridInteraction()
    {
        // Cast ray from mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;
            // Extract grid coordinates from cell name
            string[] coordinates = hitObject.name.Split('_');

            if (coordinates.Length >= 3)
            {
                int x = int.Parse(coordinates[1]);
                int z = int.Parse(coordinates[2]);

                // Handle hover effect
                if (lastHovered != hitObject)
                {
                    // Reset previous hover effect
                    if (lastHovered != null && lastHovered != selectedCell)
                    {
                        UpdateCellColor(lastHovered);
                    }

                    // Apply hover effect to new cell
                    if (hitObject != selectedCell)
                    {
                        hitObject.GetComponent<Renderer>().material.color = hoverColor;
                    }

                    lastHovered = hitObject;
                }

                // Zone assignment with Shift + number keys
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1)) SetZone(x, z, ZoneType.Residential);
                    else if (Input.GetKeyDown(KeyCode.Alpha2)) SetZone(x, z, ZoneType.Commercial);
                    else if (Input.GetKeyDown(KeyCode.Alpha3)) SetZone(x, z, ZoneType.Industrial);
                    else if (Input.GetKeyDown(KeyCode.Alpha4)) SetZone(x, z, ZoneType.Restricted);
                    else if (Input.GetKeyDown(KeyCode.Alpha0)) SetZone(x, z, ZoneType.None);
                }

                // Handle cell selection with left click
                if (Input.GetMouseButtonDown(0))
                {
                    // Reset previous selection
                    if (selectedCell != null)
                    {
                        selectedCell.GetComponent<Renderer>().material.color = defaultMaterial.color;
                    }

                    // Apply selection effect
                    selectedCell = hitObject;
                    selectedCell.GetComponent<Renderer>().material.color = selectedColor;
                    Debug.Log($"Selected grid cell at: ({x}, {z})");
                }
            }
        }
        else
        {
            // Reset hover effect when not over grid
            if (lastHovered != null && lastHovered != selectedCell)
            {
                UpdateCellColor(lastHovered);
                lastHovered = null;
            }
        }
    }


    /// Assigns a zone type to a grid cell

    public void SetZone(int x, int z, ZoneType zoneType)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            zoneGrid[x, z] = zoneType;
            UpdateCellColor(gridCells[x, z]);
        }
    }


    /// Updates the cell color based on its zone type

    private void UpdateCellColor(GameObject cell)
    {
        string[] coordinates = cell.name.Split('_');
        if (coordinates.Length >= 3)
        {
            int x = int.Parse(coordinates[1]);
            int z = int.Parse(coordinates[2]);

            Color zoneColor = GetZoneColor(zoneGrid[x, z]);
            cell.GetComponent<Renderer>().material.color = zoneColor;
        }
    }


    /// Returns the color associated with a zone type

    private Color GetZoneColor(ZoneType zoneType)
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
    }
}