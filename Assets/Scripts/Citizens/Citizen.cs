using System;
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

    public struct CitizenInfo
    {
        public CitizenModel model_type;
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

        public static void ClearCitizens()
        {
            foreach (var citizen in go_citizens)
            {
                Destroy(citizen);
            }
            go_citizens.Clear();
            go_citizens = new List<GameObject>();
        }

        public static void SetSpeedCitizens(float adjust)
        {
            if (adjust <= 0f)
            {
                throw new UnityException($"Can't call SetSpeedCitizens with {adjust}!");
            }
            foreach (var citizen in go_citizens)
            {
                /*var nma = citizen.GetComponent<NavMeshAgent>();*/
                /*nma.speed = DefaultCitizenInfo.speed * adjust;*/
                /*nma.acceleration = DefaultCitizenInfo.acceleration * adjust;*/
                /*nma.angularSpeed = DefaultCitizenInfo.angular_speed;*/
                /*nma.stoppingDistance = 0f;*/
                /*nma.autoBraking = true;*/
                var anim = citizen.GetComponent<Animator>();
                anim.logWarnings = true;
                anim.SetLookAtPosition(Vector3.up * 100f);
                print(anim.isMatchingTarget);
                print(anim.isInitialized);
            }
        }

        // The concern right now is getting things to work.
        // Performance comes later.
        private static Vector3[] velocities = new Vector3[100];

        public static void EnableMovement(bool is_enabled)
        {
            var idx = 0;
            foreach (var citizen in go_citizens)
            {

                var nma = citizen.GetComponent<NavMeshAgent>();
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

        private void Start()
        {
            throw new NotImplementedException();
        }

        private void Update()
        {
            throw new NotImplementedException();
        }
    }
}
