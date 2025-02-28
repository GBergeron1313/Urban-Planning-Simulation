using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Citizens;

public class SimCore : MonoBehaviour
{
    // Singleton
    public static SimCore Instance { get; private set; }

    // References
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private CreateBuilding buildingCreate;
    [SerializeField] private Building citizenBuilding;
    [SerializeField] private CameraControls cameraControls;

    // Simulation state
    private bool isSimulationRunning = false;
    private float simulationSpeed = 1f;
    private float simulationTimer = 0f;
    private float updateInterval = 1f; // One second intervals

    // Stats
    [Header("City Statistics")]
    public int totalBuildings = 0;
    public int redidentalBuilding = 0;
    public int commercialBuilding = 0;
    public int industrialBuilding = 0;
    public int citizenPopulation = 0;
    public float cityHappiness = 0;

    public enum SimState
    {
        Initializing,
        Planning,
        Running,
        Paused
    }

    public SimState currentState = SimState.Initializing;

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

    }

    void Start()
    {
        // Initialize systems
        if(gridSystem == null) gridSystem = FindAnyObjectByType<GridSystem>();
        if(buildingCreate == null) buildingCreate = FindAnyObjectByType<CreateBuilding>();
        if (citizenBuilding == null) citizenBuilding = FindAnyObjectByType<Building>();
        if (cameraControls == null) cameraControls = FindAnyObjectByType<CameraControls>();

        currentState = SimState.Planning;
    }

    void Update()
    {
        if (isSimulationRunning)
        {
            UpdateSimulation();
            SpeedControls();
        }
    }

    // Time Control Methods
    public void PlaySimulation()
    {
        isSimulationRunning = true;
        Time.timeScale = simulationSpeed;
        Debug.Log("Simulation Started");

        gridSystem.InvalidateCells();
        CityStatistics();
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

    private void SpeedControls()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetSimulationSpeed(1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetSimulationSpeed(2f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetSimulationSpeed(3f);
        }
    }

    // Core Update Loop
    private void UpdateSimulation()
    {
        simulationTimer += Time.deltaTime * simulationSpeed;

        if (simulationTimer >= updateInterval)
        {
            gridSystem.Update();
            simulationTimer = 0f;
        }
    }

    public void CityStatistics()
    {
        totalBuildings = gridSystem.GetBuildings().Count;
        citizenPopulation = citizenBuilding.GetCitizens().Length;

        for (int i = 0; i < gridSystem.width; i++)
        {
            for (int j = 0; j < gridSystem.height; j++)
            {
                if (gridSystem.GetZoneType(i,j) == ZoneType.Residential)
                {
                    redidentalBuilding++;
                }
                if (gridSystem.GetZoneType(i, j) == ZoneType.Commercial)
                {
                    commercialBuilding++;
                }
                if (gridSystem.GetZoneType(i,j) == ZoneType.Industrial)
                {
                    industrialBuilding++;
                }
            }
        }
    }
}

