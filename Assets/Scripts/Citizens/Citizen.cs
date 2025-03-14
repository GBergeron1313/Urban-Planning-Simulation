using System.Collections.Generic;
/*using UrbanPlanning;*/
using UnityEngine;
using UnityEngine.AI;
/*using UnityEngine.Assertions;*/

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
        public const float speed = 0.1f;
        public const float acceleration = 0.5f;
        public const float angular_speed = 120f;
        public const float area_cost = 1f;
        public const float set_speed = 0.1f;   
    }


    public class Citizen : MonoBehaviour
    {
        public static bool citizens_enabled = false;

        private static List<NavMeshAgent> nma_citizens = new List<NavMeshAgent>();
        private float last_clock_update;
        private static float dest_update_interval = 1.0f;

        private NavMeshAgent agent;

        public CitizenModel prefab_idx;

        public Citizen with_destination(Vector3 destination)
        {
            agent.SetDestination(destination);
            return this;
        }

        public Citizen with_enabled_movement(bool enabled)
        {
            agent.isStopped = !enabled;
            return this;
        }

        public Citizen with_position(Vector3 position)
        {
            agent.Warp(position);
            return this;
        }

        public Citizen with_model(CitizenModel model)
        {
            prefab_idx = model;
            return this;
        }

        public static void ClearCitizens()
        {
            foreach (var citizen in nma_citizens)
            {
                Destroy(citizen.gameObject);
            }
            nma_citizens.Clear();
            nma_citizens = null;
        }

        public static void EnableMovement(bool is_enabled)
        {
            foreach (var citizen in nma_citizens)
            {
                citizen.isStopped = !is_enabled;
            }
        }


        private void Awake()
        {
            nma_citizens ??= new List<NavMeshAgent>();

            agent = GetComponent<NavMeshAgent>();
            agent.updateUpAxis = true;
            agent.updatePosition = true;
            agent.updateRotation = false;

            agent.speed = DefaultCitizenInfo.speed;
            agent.acceleration = DefaultCitizenInfo.acceleration;
            agent.angularSpeed = DefaultCitizenInfo.angular_speed;
            nma_citizens.Add(agent);
            last_clock_update = SimCore.Instance.simulationClock;
        }


        private void Start()
        {
        }

        private void UpdateRotation()
        {
            curr_dest = agent.steeringTarget;
            agent.transform.LookAt(agent.steeringTarget);
        }

        private void UpdateDestinationAndClock()
        {
            last_clock_update = SimCore.Instance.simulationClock;

            if (agent.remainingDistance < 0.25f)
            {
                int rand = Random.Range(0, Building.building_positions.Count);
                agent.SetDestination(Building.building_positions[rand]);
            }
        }

        Vector3 curr_dest;
        private void FixedUpdate()
        {
            if (SimCore.Instance.sim_state == SimState.Running)
            {
                if (SimCore.Instance.simulationClock
                    - last_clock_update
                    > dest_update_interval)
                {
                    UpdateDestinationAndClock();
                }


                agent.transform.position =
                    Vector3.MoveTowards(
                    agent.transform.position,
                    agent.steeringTarget,
                    DefaultCitizenInfo.set_speed * SimCore.Instance.SimSpeed
                    );

                if (curr_dest != agent.steeringTarget)
                    UpdateRotation();
            }
        }

        private void Update()
        {
        }
    }
}
