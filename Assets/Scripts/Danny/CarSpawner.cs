using UnityEngine;
using Random = UnityEngine.Random;

public class CarSpawner : MonoBehaviour
{
    public GameObject[] carPrefabs;

    private void Start()
    {
    }

    private GameObject SelectACarPrefab()
    {
        var randomIndex = Random.Range(0, carPrefabs.Length);
        return Instantiate(carPrefabs[randomIndex], transform);
    }
}
