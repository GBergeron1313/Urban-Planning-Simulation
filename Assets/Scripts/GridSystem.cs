using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : MonoBehaviour
{
    [SerializeField] private int gridSize = 5; // 5x5 mile grid
    [SerializeField] private float cellSize = 1f; // Size of each grid cell in Unity units

    // References
    [SerializeField] private GameObject residentialPrefab;
    [SerializeField] private GameObject commercialPrefab;
    [SerializeField] private GameObject industrialPrefab;

    // Grid data
    private GridCell[,] grid;
    private Dictionary<Vector2Int, GameObject> buildingObjects = new Dictionary<Vector2Int, GameObject>();

    void Start()
    {
        InitializeGrid();
    }

    private void InitializeGrid()
    {
        grid = new GridCell[gridSize, gridSize];

        // Initialize each cell
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new GridCell
                {
                    position = new Vector2Int(x, y),
                    zoneType = ZoneType.Unzoned,
                    isOccupied = false
                };
            }
        }
    }

    // Update zone type for a cell
    public bool SetZoneType(int x, int y, ZoneType newType)
    {
        if (!IsValidPosition(x, y)) return false;

        grid[x, y].zoneType = newType;
        UpdateCellVisual(x, y);
        return true;
    }

    // Place a building in a cell
    public bool PlaceBuilding(int x, int y)
    {
        if (!IsValidPosition(x, y)) return false;
        if (grid[x, y].isOccupied) return false;
        if (grid[x, y].zoneType == ZoneType.Unzoned) return false;

        Vector2Int pos = new Vector2Int(x, y);
        GameObject buildingPrefab = GetBuildingPrefab(grid[x, y].zoneType);

        if (buildingPrefab != null)
        {
            Vector3 worldPos = GetWorldPosition(x, y);
            GameObject building = Instantiate(buildingPrefab, worldPos, Quaternion.identity);

            if (buildingObjects.ContainsKey(pos))
            {
                Destroy(buildingObjects[pos]);
            }

            buildingObjects[pos] = building;
            grid[x, y].isOccupied = true;
            return true;
        }

        return false;
    }

    // Convert grid position to world position
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x * cellSize, 0, y * cellSize);
    }

    // Convert world position to grid position
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.z / cellSize);
        return new Vector2Int(x, y);
    }

    private void UpdateCellVisual(int x, int y)
    {
        // Remove existing building if present
        Vector2Int pos = new Vector2Int(x, y);
        if (buildingObjects.ContainsKey(pos))
        {
            Destroy(buildingObjects[pos]);
            buildingObjects.Remove(pos);
        }

        grid[x, y].isOccupied = false;
    }

    private GameObject GetBuildingPrefab(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Residential:
                return residentialPrefab;
            case ZoneType.Commercial:
                return commercialPrefab;
            case ZoneType.Industrial:
                return industrialPrefab;
            default:
                return null;
        }
    }

    private bool IsValidPosition(int x, int y)
    {
        return x >= 0 && x < gridSize && y >= 0 && y < gridSize;
    }

    // For SimCore save/load
    public string GetSerializedData()
    {
        GridData data = new GridData
        {
            cells = new List<SerializableGridCell>()
        };

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                data.cells.Add(new SerializableGridCell
                {
                    x = x,
                    y = y,
                    zoneType = grid[x, y].zoneType,
                    isOccupied = grid[x, y].isOccupied
                });
            }
        }

        return JsonUtility.ToJson(data);
    }

    public void LoadFromData(string jsonData)
    {
        GridData data = JsonUtility.FromJson<GridData>(jsonData);

        foreach (var cell in data.cells)
        {
            grid[cell.x, cell.y].zoneType = cell.zoneType;
            grid[cell.x, cell.y].isOccupied = cell.isOccupied;

            if (cell.isOccupied)
            {
                PlaceBuilding(cell.x, cell.y);
            }
        }
    }

    public void UpdateGrid()
    {
        // Update building states
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (grid[x, y].isOccupied)
                {
                    // Update building logic here
                    UpdateBuilding(x, y);
                }
            }
        }
    }

    private void UpdateBuilding(int x, int y)
    {
        // Add any per-building update logic here
        // For example:
        // - Check building conditions
        // - Update building state
        // - Handle building effects on surroundings

        Vector2Int pos = new Vector2Int(x, y);
        if (buildingObjects.ContainsKey(pos))
        {
            // Update building visuals or state if needed
        }
    }
}



// Data structures
[Serializable]
public class GridCell
{
    public Vector2Int position;
    public ZoneType zoneType;
    public bool isOccupied;
}

[Serializable]
public class SerializableGridCell
{
    public int x;
    public int y;
    public ZoneType zoneType;
    public bool isOccupied;
}

[Serializable]
public class GridData
{
    public List<SerializableGridCell> cells;
}

public enum ZoneType
{
    Unzoned,
    Residential,
    Commercial,
    Industrial
}
