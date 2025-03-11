using System.Collections.Generic;
using Danny;
using UnityEngine;
using UnityEngine.AI;

namespace Citizens
{
    public enum CitizenModel
    {
        female_casual = 0,
        female_dress,
        male_casual,
        male_suit,

        CITIZEN_MODELS_MAX
    }

    public class DefaultCitizenInfo
    {
        public const float speed = 0.35f;
        public const float acceleration = 0.8f;
        public const float angular_speed = 120f;
        public const float area_cost = 1f;
    }


    public class Citizen : MonoBehaviour
    {
        public static bool citizens_enabled = false;
        public static List<GameObject> go_citizens = new List<GameObject>();

        private static List<NavMeshAgent> nma_citizens = new List<NavMeshAgent>();

        // The concern right now is getting things to work.
        // Performance comes later.
        private static Vector3[] velocities = new Vector3[100];


        NavMeshAgent agent;

        public CitizenModel prefab_idx;


        public static void ClearCitizens()
        {
            foreach (var citizen in go_citizens)
            {
                Destroy(citizen);
            }
            go_citizens.Clear();
            go_citizens = new List<GameObject>();
        }


        public static void EnableMovement(bool is_enabled)
        {
            var idx = 0;
            foreach (var citizen in go_citizens)
            {

                var nma = citizen.GetComponent<NavMeshAgent>();
                nma_citizens.Add(nma);

                nma.isStopped = !is_enabled;
                if (nma.isStopped)
                {
                    velocities[idx++] = nma.velocity;
                    nma.velocity = Vector3.zero;
                    nma.speed = 0f;
                    nma.acceleration = 0f;
                    nma.updateRotation = false;

                    SpawnManager.spawned_and_moving = false;
                    Citizen.citizens_enabled = false;
                }
                else
                {
                    nma.velocity = velocities[idx++];
                    nma.speed = DefaultCitizenInfo.speed;
                    nma.acceleration = DefaultCitizenInfo.acceleration;
                    nma.angularSpeed = DefaultCitizenInfo.angular_speed;
                    nma.updateRotation = false;

                    SpawnManager.spawned_and_moving = true;
                    Citizen.citizens_enabled = true;
                }
            }
        }


        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = true;
            agent.updatePosition = true;
        }


        private void Start()
        {
        }

        private void Update()
        {
            if (SimCore.Instance.sim_state == SimState.Running)
            {
                agent.transform.position =
                    Vector3.MoveTowards(
                        agent.transform.position,
                        agent.steeringTarget,
                        0.01f * SimCore.Instance.SimSpeed);
            }
        }
    }
}
