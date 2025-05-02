using BuildingUtils;
using UnityEngine;
using UnityEngine.UI;

public class CarSpawnerButton : MonoBehaviour
{
    public GameObject carSpawnerPrefab; // Reference to the CarSpawner prefab
    public Button carButton; // Assign your button in Inspector

    private CarSpawner carSpawnerInstance; // Store spawned CarSpawner

    private void Start()
    {
        
    }

    public void SpawnCars()
    {
        // Ensure there are buildings before spawning cars
        if (Building.building_positions.Count == 0)
        {
            Debug.LogError("Can't spawn cars when there are no buildings!");
            return;
        }

        carButton.interactable = true;

        // Restore original functionality
        carButton.onClick.AddListener(SpawnCars);

        // Make sure the component is enabled
        carButton.enabled = true;

        // Check if CarSpawner exists; if not, spawn it
        if (carSpawnerInstance == null)
        {
            GameObject carSpawnerObj = Instantiate(carSpawnerPrefab, Vector3.zero, Quaternion.identity);
            carSpawnerInstance = carSpawnerObj.GetComponent<CarSpawner>();
        }

        if (carSpawnerInstance != null)
        {
            for (int i = 0; i < 1; i++) // Adjust number of cars as needed
            {
                carSpawnerInstance.SpawnCar();
            }
        }
        else
        {
            Debug.LogError("CarSpawner script not found on the instantiated prefab!");
        }
    }
}
