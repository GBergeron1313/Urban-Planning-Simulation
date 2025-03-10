using Citizens;
using UnityEngine;
using UnityEngine.UI;

public class SimCore : MonoBehaviour
{
    // Singleton pattern for easy access
    public static SimCore Instance { get; private set; }

    // Core system references
    [SerializeField] private GridSystem gridSystem;

    // Simulation state
    public bool isSimulationRunning = false;
    private float simulationSpeed = 1f;
    private float simulationTimer = 0f;
    private float updateInterval = 1f; // One second intervals

    private Button pauseSimulationButton;
    private Button playSimulationButton;

    public float simulationClock = 0f;

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
        }
        else
        {
            Destroy(gameObject);
        }

        playSimulationButton = GameObject.Find("PLAY").GetComponent<Button>();
        pauseSimulationButton = GameObject.Find("Pause").GetComponent<Button>();
    }

    void Start()
    {
        // Initialize systems
        InitializeSystems();
        PauseSimulation();
    }

    void Update()
    {
        if (isSimulationRunning)
            simulationClock += Time.unscaledDeltaTime * simulationSpeed;
    }

    // Time Control Methods
    public void PlaySimulation()
    {
        isSimulationRunning = true;
        Citizen.EnableMovement(true);
        Debug.Log("Simulation Started");
        playSimulationButton.targetGraphic.color = Color.green;
        pauseSimulationButton.targetGraphic.color = Color.red;
        pauseSimulationButton.enabled = true;
        playSimulationButton.enabled = false;
    }

    public void PauseSimulation()
    {
        isSimulationRunning = false;
        Citizen.EnableMovement(false);
        Debug.Log("Simulation Paused");
        pauseSimulationButton.targetGraphic.color = Color.green;
        playSimulationButton.targetGraphic.color = Color.red;
        pauseSimulationButton.enabled = false;
        playSimulationButton.enabled = true;
    }

    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = Mathf.Clamp(speed, 0.1f, 3f);
        Time.timeScale = simulationSpeed;
    }

    // Core Update Loop
    private void UpdateSimulation()
    {
        simulationClock += Time.unscaledDeltaTime * simulationSpeed;
    }

    private void InitializeSystems()
    {
        // Verify all required systems are present
        if (gridSystem == null)
            Debug.LogError("GridSystem reference missing in SimCore!");
    }
}

