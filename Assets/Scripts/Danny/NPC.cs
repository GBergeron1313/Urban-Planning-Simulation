using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class NPC : MonoBehaviour
{
    public string ID;
    public NPCState State;
    public List<Need> Needs;
    public Demographics Stats;
    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        State = NPCState.Idle;
    }

    void Update()
    {
        switch (State)
        {
            case NPCState.Idle:
                HandleIdleState();
                break;
            case NPCState.Working:
                HandleWorkingState();
                break;
            case NPCState.Traveling:
                HandleTravelingState();
                break;
            case NPCState.SeekingServices:
                HandleSeekingServicesState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Check if the NPC needs to work
        if (Needs.Exists(n => n.Name == "Work" && n.SatisfactionLevel < 0.5f))
        {
            State = NPCState.Traveling;
            MoveTo(FindJobLocation());
        }
    }

    private void HandleWorkingState()
    {
        // Simulate working (e.g., increase income)
        Stats.Income += Time.deltaTime * 10; // Example: $10 per second
    }

    private void HandleTravelingState()
    {
        // Check if the NPC has reached the destination
        if (agent.remainingDistance < 0.1f)
        {
            State = NPCState.Working;
        }
    }

    private void HandleSeekingServicesState()
    {
        // Handle seeking services (e.g., hospital, school)
    }

    public void MoveTo(Vector3 destination)
    {
        agent.SetDestination(destination);
    }

    private Vector3 FindJobLocation()
    {
        // Query the GridManager for available jobs
        return new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10)); // Example: Random location
    }
}

public enum NPCState
{
    Idle,
    Working,
    Traveling,
    SeekingServices
}

public class Need
{
    public string Name;
    public float SatisfactionLevel;

    public void Fulfill()
    {
        SatisfactionLevel = 1.0f;
    }
}

public class Demographics
{
    public int Age;
    public bool IsEmployed;
    public float Income;
}