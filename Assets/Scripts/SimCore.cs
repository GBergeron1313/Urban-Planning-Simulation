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

    private Button pauseSimulationButton;
    private Button playSimulationButton;

    public static class Time
    {
        public static float time_step;
        public static float now;
    }

    public float SimSpeed
    {
        get;
        private set;
    }

    public ViewMode view_mode
    {
        get;
        private set;
    }

    public SimState state
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
            Instance.SimSpeed = 1f;
        }
        else
        {
            Destroy(gameObject);
        }

        playSimulationButton = GameObject.Find("PlayButton").GetComponent<Button>();
        pauseSimulationButton = GameObject.Find("PauseButton").GetComponent<Button>();
    }

    void Start()
    {
        PauseSimulation();
        InitializeSystems();
    }

    void Update()
    {
        if (Instance.state == SimState.Running)
        {
            SimCore.Time.time_step = UnityEngine.Time.unscaledDeltaTime * SimSpeed;
            SimCore.Time.now += SimCore.Time.time_step;
        }

        if (Input.GetKeyDown(KeyCode.V))
            CycleViewModes();

        if (Input.GetKeyDown(KeyCode.F))
            IncreaseSimulationSpeed();

        if (Input.GetKeyDown(KeyCode.G))
            DecreaseSimulationSpeed();
    }

    private void DecreaseSimulationSpeed()
    {
        SimSpeed *= 0.5f;
        SimSpeed = Mathf.Clamp(SimSpeed, 0.03125f, 32.0f);
    }

    private void IncreaseSimulationSpeed()
    {
        SimSpeed *= 2.0f;
        SimSpeed = Mathf.Clamp(SimSpeed, 0.03125f, 32.0f);
    }

    private void CycleSimulationStates()
    {
        state++;
        if (state >= SimState.TotalSimStates)
            state = SimState.Default;
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
        state = SimState.Running;
        Citizen.EnableMovement(true);
        Debug.Log("Simulation Started");
        playSimulationButton.targetGraphic.color = Color.green;
        pauseSimulationButton.targetGraphic.color = Color.red;
        pauseSimulationButton.enabled = true;
        playSimulationButton.enabled = false;
    }

    public void PauseSimulation()
    {
        state = SimState.Paused;
        Citizen.EnableMovement(false);
        Debug.Log("Simulation Paused");
        pauseSimulationButton.targetGraphic.color = Color.green;
        playSimulationButton.targetGraphic.color = Color.red;
        pauseSimulationButton.enabled = false;
        playSimulationButton.enabled = true;
    }

    /*public void SetSimulationSpeed(float speed)*/
    /*{*/
    /*    SimSpeed = Mathf.Clamp(speed, 0.1f, 3f);*/
    /*    Time.timeScale = SimSpeed;*/
    /*}*/

    // Core Update Loop
    private void UpdateSimulation()
    {
        SimCore.Time.now += UnityEngine.Time.unscaledDeltaTime * SimSpeed;
    }

    private void InitializeSystems()
    {
        // Verify all required systems are present
        if (gridSystem == null)
            Debug.LogError("GridSystem reference missing in SimCore!");
    }
}

