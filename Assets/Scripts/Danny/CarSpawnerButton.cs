using UnityEngine;
using UnityEngine.UI;

public class CarSpawnerButton : MonoBehaviour
{
    public GameObject carSpawnerPrefab; // Reference to the CarSpawner prefab
    public Transform spawnPoint; // Assign this in the Inspector

    private void Start()
    {
        // Get the Button component and add a listener to the onClick event
        Button button = GetComponent<Button>();
        button.onClick.AddListener(SpawnCarSpawner);
    }

    public void SpawnCarSpawner()
    {
        if (carSpawnerPrefab != null)
        {
            Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;

            // Instantiate the CarSpawner at the specified position
            GameObject carSpawnerInstance = Instantiate(carSpawnerPrefab, spawnPosition, Quaternion.identity);

            // Call the SpawnCar method on the instantiated CarSpawner
            CarSpawner carSpawnerScript = carSpawnerInstance.GetComponent<CarSpawner>();
            if (carSpawnerScript != null)
            {
                carSpawnerScript.SpawnCar(); // Spawn a car immediately
            }
            else
            {
                Debug.LogError("CarSpawner script not found on the instantiated prefab!");
            }
        }
        else
        {
            Debug.LogError("CarSpawner prefab is not assigned!");
        }
    }
}