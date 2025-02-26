using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    public float wanderRadius = 10f;
    public float wanderTimer = 5f;

    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        timer = wanderTimer;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= wanderTimer)
        {
            Vector3 newPos = GetRandomPoint(transform.position, wanderRadius);
            agent.SetDestination(newPos);
            timer = 0;
        }
    }

    Vector3 GetRandomPoint(Vector3 origin, float dist)
    {
        Vector3 randomDirection = Random.insideUnitSphere * dist;
        randomDirection += origin;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomDirection, out hit, dist, 1);
        return hit.position;
    }
}
