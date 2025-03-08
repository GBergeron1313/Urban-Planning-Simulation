using UnityEngine;
using UnityEngine.AI;

namespace Danny
{
    public class NPCMovement : MonoBehaviour
    {
        private NavMeshAgent agent;
        public float wanderRadius = 2f;
        public float wanderTimer = 15f;

        private float timer;

        void Start()
        {
            if (!SpawnManager.spawned_and_moving)
            {
                agent = GetComponent<NavMeshAgent>();
                agent.radius /= 10.0f;
                agent.acceleration /= 10.0f;
                agent.speed /= 10.0f;
                agent.updateRotation = true;
                agent.autoRepath = true;
                timer = wanderTimer;
            }
        }

        Vector3 GetRandomPoint(Vector3 origin, float dist)
        {
            Vector3 randomDirection = new Vector3(Random.insideUnitCircle.x, 0, Random.insideUnitCircle.y);
            randomDirection *= dist;
            randomDirection += origin;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, dist, 1);
            return hit.position;
        }
    }
}
