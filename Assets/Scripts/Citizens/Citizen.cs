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


    public class Citizen : MonoBehaviour
    {
        public static bool citizens_enabled = false;
        public static List<GameObject> go_citizens = new List<GameObject>();

        public static Cell CitizenCellLocation(Vector3 position)
        {
            return new Cell();
        }

        public static void ClearCitizens()
        {
            foreach (var citizen in go_citizens)
            {
                Destroy(citizen);
            }
            go_citizens.Clear();
            go_citizens = new List<GameObject>();
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
                    SpawnManager.spawned_and_moving = false;
                    Citizen.citizens_enabled = false;
                } else {
                    nma.velocity = velocities[idx++];
                    nma.speed = 0.35f;
                    nma.acceleration = 0.8f;
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
