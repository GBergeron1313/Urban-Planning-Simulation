using Citizens;
using UnityEngine;
using UnityEngine.UI;


public enum SimState
{
    Default,
    Running = Default,
    Paused,

    TotalSimStates,
}

public enum ViewMode
{
    Default,
    Clear,

    TotalViewModes,
}

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

    private Button pauseSimulationButton;
    private Button playSimulationButton;

    public float simulationClock = 0f;


    public float SimSpeed
    {
        get { return simulationSpeed; }
    }

    public ViewMode view_mode
    {
        get;
        private set;
    }

    public SimState sim_state
    {
        get;
        private set;
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
        if (sim_state == SimState.Running)
            simulationClock += Time.unscaledDeltaTime * simulationSpeed;
        if (Input.GetKeyDown(KeyCode.V))
            CycleViewModes();

        if (Input.GetKeyDown(KeyCode.F))
            IncreaseSimulationSpeed();

        if (Input.GetKeyDown(KeyCode.G))
            DecreaseSimulationSpeed();
    }

    private void DecreaseSimulationSpeed()
    {
        simulationSpeed *= 0.5f;
        simulationSpeed = Mathf.Clamp(simulationSpeed, 0.03125f, 32.0f);
        Citizen.SetSpeedCitizens(simulationSpeed);
    }
    private void IncreaseSimulationSpeed()
    {
        simulationSpeed *= 2.0f;
        simulationSpeed = Mathf.Clamp(simulationSpeed, 0.03125f, 32.0f);
        Citizen.SetSpeedCitizens(simulationSpeed);
    }

    private void CycleSimulationStates()
    {
        sim_state++;
        if (sim_state >= SimState.TotalSimStates)
            sim_state = SimState.Default;
    }

    private void CycleViewModes()
    {
        view_mode++;
        if (view_mode >= ViewMode.TotalViewModes)
            view_mode = ViewMode.Default;
    }

    // Time Control Methods
    public void PlaySimulation()
    {
        sim_state = SimState.Running;
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
        sim_state = SimState.Paused;
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

