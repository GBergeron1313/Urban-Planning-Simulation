using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;
    public float spawnHeightOffset = 0.1f;

    public void SpawnCar()
    {
        // Get all road cells
        List<Cell> roadCells = GetRoadCells();

        if (roadCells.Count == 0)
        {
            Debug.LogWarning("No roads available for car spawning!");
            return;
        }

        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogError("Car prefabs are not assigned!");
            return;
        }

        // Select a random road cell
        Cell randomRoad = roadCells[Random.Range(0, roadCells.Count)];
        Vector3 spawnPosition = randomRoad.transform.position;
        spawnPosition.y += spawnHeightOffset;

        // Instantiate the car
        Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)],
                   spawnPosition,
                   Quaternion.identity);
    }

    private List<Cell> GetRoadCells()
    {
        List<Cell> roadCells = new List<Cell>();
        foreach (Cell cell in Cell.all_cells)
        {
            if (cell.cell_type == CellType.Road)
            {
                roadCells.Add(cell);
            }
        }
        return roadCells;
    }
}
