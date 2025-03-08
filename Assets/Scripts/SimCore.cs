using UnityEngine;

public class SimCore : MonoBehaviour
{
    // Singleton pattern for easy access
    public static SimCore Instance { get; private set; }

    // Core system references
    [SerializeField] private GridSystem gridSystem;

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
        Time.timeScale = simulationSpeed;
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

    private void InitializeSystems()
    {
        // Verify all required systems are present
        if (gridSystem == null)
            Debug.LogError("GridSystem reference missing in SimCore!");
    }
}

