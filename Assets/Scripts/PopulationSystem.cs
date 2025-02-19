using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopulationSystem : MonoBehaviour
{
    [Header("Population Settings")]
    [SerializeField] private int maxCitizens = 100;
    [SerializeField] private GameObject citizenPrefab;
    [SerializeField] private float spawnInterval = 5f;

    // Population tracking
    private List<Citizen> citizens = new List<Citizen>();
    private float spawnTimer;

    void Start()
    {

    }

    void Update()
    {
        UpdatePopulation();
        HandleSpawning();
    }

    private void InitializeSystem()
    {
        //if (gridSystem == null)
          //  gridSystem = FindObjectOfType<GridSystem>();

        // Initialize NavMesh if needed
        //if (!UnityEngine.AI.NavMesh.isActiveAndEnabled)
            //Debug.LogError("NavMesh is required for citizen movement!");
    }

    public void UpdatePopulation()
    {
        // Update each citizen's behavior
        for (int i = citizens.Count - 1; i >= 0; i--)
        {
            if (citizens[i] != null)
            {
                citizens[i].UpdateBehavior();
            }
            else
            {
                citizens.RemoveAt(i);
            }
        }
    }

    private void HandleSpawning()
    {
        if (citizens.Count >= maxCitizens) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnCitizen();
            spawnTimer = 0f;
        }
    }

    public bool SpawnCitizen()
    {
        if (citizens.Count >= maxCitizens) return false;

        // Find residential zone to spawn citizen
        Vector3 spawnPosition = FindValidSpawnPosition();
        if (spawnPosition == Vector3.zero) return false;

        GameObject citizenObj = Instantiate(citizenPrefab, spawnPosition, Quaternion.identity);
        Citizen newCitizen = citizenObj.GetComponent<Citizen>();

        if (newCitizen != null)
        {
            newCitizen.Initialize(this);
            citizens.Add(newCitizen);
            return true;
        }

        return false;
    }

    private Vector3 FindValidSpawnPosition()
    {
        // Implementation depends on your GridSystem
        // This is a simplified version
        return new Vector3(0, 0, 0); // Replace with actual logic
    }

    public void RemoveCitizen(Citizen citizen)
    {
        if (citizens.Contains(citizen))
        {
            citizens.Remove(citizen);
            Destroy(citizen.gameObject);
        }
    }

    // For SimCore save/load
    public string GetSerializedData()
    {
        PopulationData data = new PopulationData
        {
            citizenData = new List<CitizenData>()
        };

        foreach (var citizen in citizens)
        {
            data.citizenData.Add(citizen.GetSerializedData());
        }

        return JsonUtility.ToJson(data);
    }

    public void LoadFromData(string jsonData)
    {
        // Clear existing population
        foreach (var citizen in citizens)
        {
            if (citizen != null)
                Destroy(citizen.gameObject);
        }
        citizens.Clear();

        // Load saved population
        PopulationData data = JsonUtility.FromJson<PopulationData>(jsonData);
        foreach (var citizenData in data.citizenData)
        {
            Vector3 position = new Vector3(citizenData.posX, citizenData.posY, citizenData.posZ);
            GameObject citizenObj = Instantiate(citizenPrefab, position, Quaternion.identity);
            Citizen citizen = citizenObj.GetComponent<Citizen>();

            if (citizen != null)
            {
                citizen.Initialize(this);
                citizen.LoadFromData(citizenData);
                citizens.Add(citizen);
            }
        }
    }
}

// Citizen class that manages individual behavior
public class Citizen : MonoBehaviour
{
    private PopulationSystem populationSystem;
    private GridSystem gridSystem;
    private UnityEngine.AI.NavMeshAgent agent;

    [SerializeField] private CitizenState currentState = CitizenState.Idle;
    private Vector3 homeLocation;
    private Vector3 workLocation;
    private float stateTimer;

    public void Initialize(PopulationSystem popSystem)
    {
        populationSystem = popSystem;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        // Assign home and work locations
        AssignLocations();
    }

    public void UpdateBehavior()
    {
        switch (currentState)
        {
            case CitizenState.Idle:
                HandleIdleState();
                break;
            case CitizenState.GoingToWork:
                HandleWorkState();
                break;
            case CitizenState.Working:
                HandleWorkingState();
                break;
            case CitizenState.GoingHome:
                HandleHomeState();
                break;
        }
    }

    private void AssignLocations()
    {
        // Implement logic to find home in residential zone
        // and work in commercial/industrial zone
        homeLocation = transform.position; // Temporary
        workLocation = transform.position + Vector3.right * 10; // Temporary
    }

    private void HandleIdleState()
    {
        // Implement idle behavior
    }

    private void HandleWorkState()
    {
        if (agent != null)
            agent.SetDestination(workLocation);
    }

    private void HandleWorkingState()
    {
        // Implement working behavior
    }

    private void HandleHomeState()
    {
        if (agent != null)
            agent.SetDestination(homeLocation);
    }

    public CitizenData GetSerializedData()
    {
        return new CitizenData
        {
            posX = transform.position.x,
            posY = transform.position.y,
            posZ = transform.position.z,
            state = currentState,
            homeX = homeLocation.x,
            homeY = homeLocation.y,
            homeZ = homeLocation.z,
            workX = workLocation.x,
            workY = workLocation.y,
            workZ = workLocation.z
        };
    }

    public void LoadFromData(CitizenData data)
    {
        currentState = data.state;
        homeLocation = new Vector3(data.homeX, data.homeY, data.homeZ);
        workLocation = new Vector3(data.workX, data.workY, data.workZ);
    }
}

// Data structures
public enum CitizenState
{
    Idle,
    GoingToWork,
    Working,
    GoingHome
}

[Serializable]
public class CitizenData
{
    public float posX, posY, posZ;
    public CitizenState state;
    public float homeX, homeY, homeZ;
    public float workX, workY, workZ;
}

[Serializable]
public class PopulationData
{
    public List<CitizenData> citizenData;
}
