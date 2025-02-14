using System;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")]
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float gridHeight = 0f;
    public Material gridMaterial;

    [Header("Interaction Settings")]
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;

    private GameObject[,] gridCells;
    private Material defaultMaterial;
    private GameObject lastHovered;
    private GameObject selectedCell;

    private bool[,] filledCells;

    void Start()
    {
        gridCells = new GameObject[width, height];
        filledCells = new bool[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                filledCells[x, z] = false;
            }
        }
                GenerateGrid();
    }

    void GenerateGrid()
    {
        // Create parent object for grid
        GameObject gridParent = new GameObject("Grid");
        gridParent.transform.parent = transform;

        // Calculate starting position to center the grid
        Vector3 startPos = transform.position - new Vector3(width * cellSize / 2f, 0, height * cellSize / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Create cell
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.name = $"Cell_{x}_{z}";
                cell.transform.parent = gridParent.transform;

                // Position and rotate cell
                Vector3 pos = startPos + new Vector3(x * cellSize, gridHeight, z * cellSize);
                cell.transform.position = pos;
                cell.transform.rotation = Quaternion.Euler(90, 0, 0);
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);

                // Add material
                if (gridMaterial != null)
                {
                    cell.GetComponent<Renderer>().material = gridMaterial;
                }

                // Add collider for interaction
                BoxCollider collider = cell.AddComponent<BoxCollider>();
                collider.size = new Vector3(1, 1, 0.1f);

                // Store reference
                gridCells[x, z] = cell;
            }
        }

        // Store default material
        if (gridCells[0, 0] != null)
        {
            defaultMaterial = gridCells[0, 0].GetComponent<Renderer>().material;
        }
    }

    void Update()
    {
        HandleGridInteraction();
    }

    void HandleGridInteraction()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Handle hover
            if (lastHovered != hitObject)
            {
                // Reset last hovered cell
                if (lastHovered != null && lastHovered != selectedCell)
                {
                    lastHovered.GetComponent<Renderer>().material.color = defaultMaterial.color;
                }

                // Highlight new cell
                if (hitObject != selectedCell)
                {
                    hitObject.GetComponent<Renderer>().material.color = hoverColor;
                }

                lastHovered = hitObject;
            }

            // Handle selection
            if (Input.GetMouseButtonDown(0))
            {
                // Reset previous selection
                if (selectedCell != null)
                {
                    selectedCell.GetComponent<Renderer>().material.color = defaultMaterial.color;
                }

                // Set new selection
                selectedCell = hitObject;
                selectedCell.GetComponent<Renderer>().material.color = selectedColor;

                // Get grid coordinates
                string[] coordinates = hitObject.name.Split('_');
                if (coordinates.Length >= 3)
                {
                    int x = int.Parse(coordinates[1]);
                    int z = int.Parse(coordinates[2]);
                    Debug.Log($"Selected grid cell at: ({x}, {z})");
                }
            }
        }
        else
        {
            // Reset hover when not over grid
            if (lastHovered != null && lastHovered != selectedCell)
            {
                lastHovered.GetComponent<Renderer>().material.color = defaultMaterial.color;
                lastHovered = null;
            }
        }
    }

    // Helper method to get cell at grid coordinates
    public GameObject GetCellAt(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridCells[x, z];
        }
        return null;
    }

    public bool isCellFilled(int x, int z)
    {
        if (filledCells[x, z])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void fillCell(int x, int z)
    {
        filledCells[x, z] = true;
    }
}