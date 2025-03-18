using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs; // Array of car prefabs to spawn
    public Transform[] spawnPoints; // Array of spawn points

    // Public method to spawn a car
    public void SpawnCar()
    {
        // Check if carPrefabs and spawnPoints are assigned
        if (carPrefabs == null || carPrefabs.Length == 0)
        {
            Debug.LogError("Car prefabs are not assigned!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawn points are not assigned!");
            return;
        }

        // Select a random car prefab and spawn point
        GameObject randomCarPrefab = carPrefabs[Random.Range(0, carPrefabs.Length)];
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Add a small offset to the spawn position to prevent falling through the ground
        Vector3 spawnPosition = randomSpawnPoint.position + Vector3.up * 0.5f;

        // Instantiate the car at the adjusted spawn position
        Instantiate(randomCarPrefab, spawnPosition, randomSpawnPoint.rotation);
    }
}