using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCore : MonoBehaviour
{
    // Singleton pattern for easy access
    public static SimCore Instance { get; private set; }

    // Core system references
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private PopulationSystem populationSystem;
    [SerializeField] private AnalyticsSystem analyticsSystem;

    // Simulation state
    private bool isSimulationRunning = false;
    private float simulationSpeed = 1f;
    private float simulationTimer = 0f;
    private float updateInterval = 1f; // One second intervals

    private enum SimState
    {
        Initializing,
        Running,
        Paused
    }


    // Save/Load functionality
    private string saveFilePath;

    void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        saveFilePath = Application.persistentDataPath + "/simulation_save.json";
    }

    void Start()
    {
        // Initialize systems
        InitializeSystems();
    }

    void Update()
    {
        if (isSimulationRunning)
        {
            UpdateSimulation();
        }
    }

    // Time Control Methods
    public void PlaySimulation()
    {
        isSimulationRunning = true;
        Time.timeScale = simulationSpeed;
        Debug.Log("Simulation Started");
    }

    public void PauseSimulation()
    {
        isSimulationRunning = false;
        Time.timeScale = 0;
        Debug.Log("Simulation Paused");
    }

    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = Mathf.Clamp(speed, 0.1f, 3f);
        if (isSimulationRunning)
        {
            Time.timeScale = simulationSpeed;
        }
    }

    // Core Update Loop
    private void UpdateSimulation()
    {
        simulationTimer += Time.deltaTime * simulationSpeed;

        if (simulationTimer >= updateInterval)
        {
            gridSystem.Update();
            populationSystem.UpdatePopulation();
            analyticsSystem.UpdateAnalytics();
            simulationTimer = 0f;
        }
    }

    // Save/Load Operations
    public void SaveSimulation()
    {
        try
        {
            SimulationData data = new SimulationData
            {
                timestamp = DateTime.Now.ToString(),
                populationData = populationSystem.GetSerializedData(),
                analyticsData = analyticsSystem.GetSerializedData()
            };

            string jsonData = JsonUtility.ToJson(data, true);
            System.IO.File.WriteAllText(saveFilePath, jsonData);
            Debug.Log("Simulation saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving simulation: {e.Message}");
        }
    }

    public void LoadSimulation()
    {
        try
        {
            if (System.IO.File.Exists(saveFilePath))
            {
                string jsonData = System.IO.File.ReadAllText(saveFilePath);
                SimulationData data = JsonUtility.FromJson<SimulationData>(jsonData);

                // Load data into each system
                populationSystem.LoadFromData(data.populationData);
                analyticsSystem.LoadFromData(data.analyticsData);

                Debug.Log("Simulation loaded successfully");
            }
            else
            {
                Debug.LogWarning("No save file found");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading simulation: {e.Message}");
        }
    }

    private void InitializeSystems()
    {
        // Verify all required systems are present
        if (gridSystem == null)
            Debug.LogError("GridSystem reference missing in SimCore!");
        if (populationSystem == null)
            Debug.LogError("PopulationSystem reference missing in SimCore!");
        if (analyticsSystem == null)
            Debug.LogError("AnalyticsSystem reference missing in SimCore!");
    }
}

// Data structure for save/load operations
[Serializable]
public class SimulationData
{
    public string timestamp;
    public string gridData;
    public string populationData;
    public string analyticsData;
}

