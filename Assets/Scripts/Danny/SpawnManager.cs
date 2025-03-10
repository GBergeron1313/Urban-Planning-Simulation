using System.Collections.Generic;
using Citizens;
/*using Unity.AI.Navigation;*/
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Danny
{
    public class SpawnManager : MonoBehaviour
    {
        public GameObject[] npcPrefabs; // Assign 4 prefabs in Inspector
        public Button npcButton; // Assign your button in Inspector
        public Transform spawnArea; // Assign an area where NPCs will spawn
        public static readonly int npcCount = 100; // Number of NPCs to spawn
        public static bool spawned_and_moving = false;

        void Start()
        {
            name = "SpawnManager";
            npcButton.onClick.AddListener(SpawnNPCs);
        }

        public void SpawnNPCsFrom(string[] citizen_names, Vector3[] positions, Vector3[] destinations)
        {
            if (Building.building_positions.Count == 0)
                throw new UnityException("Can't spawn npcs when there are no buildings");

            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;

            if (Citizen.go_citizens is not null
                &&
                Citizen.go_citizens.Count > 0)
            {
                Citizen.ClearCitizens();
            }
            else
            {
                Citizen.go_citizens = new List<GameObject>(npcCount);
            }


            for (int i = 0; i < npcCount; i++)
            {
                string[] name_id_index = citizen_names[i].Split('_');
                int prefab_index = int.Parse(name_id_index[2]);
                GameObject npc =
                    Instantiate(npcPrefabs[prefab_index],
                            positions[i],
                            Quaternion.identity);
                npc.name = citizen_names[i];
                npc.tag = "Citizens";

                // Ensure the animator is enabled
                Animator npcAnimator = npc.GetComponent<Animator>();
                if (npcAnimator is not null)
                {
                    var nma = npc.GetComponent<NavMeshAgent>();

                    Assert.IsTrue(nma.Warp(positions[i]));
                    Assert.IsTrue(nma.SetDestination(destinations[i]));
                    nma.radius /= 10.0f;
                    nma.acceleration /= 10.0f;
                    nma.speed /= 10.0f;
                    nma.updateRotation = true;
                    nma.autoRepath = true;
                    Citizen.go_citizens.Add(npc);

                }
                else
                {
                    throw new UnityException("NPCAnimator was null");
                }
            }

            if (SimCore.Instance.isSimulationRunning)
            {
                Citizen.EnableMovement(true);
            }
            else
            {
                Citizen.EnableMovement(false);
            }
        }

        public void SpawnNPCs()
        {
            if (Building.building_positions.Count == 0)
                throw new UnityException("Can't spawn npcs when there are no buildings");

            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;
            Citizen.go_citizens = new List<GameObject>(npcCount);

            for (int i = 0; i < npcCount; i++)
            {
                int rand_prefab_idx = Random.Range(0, npcPrefabs.Length);
                int rand_position_idx = Random.Range(0, Building.building_positions.Count);
                GameObject npc = Instantiate(
                        npcPrefabs[rand_prefab_idx],
                        Building.building_positions[rand_position_idx],
                        Quaternion.identity);

                // For Saving and reloading, each citizen needs a unique name.
                // Saving the prefab index is somewhat hacky, but it works for now.
                // TODO: Make a real "Citizen" class to store this kind of info.
                npc.name = $"Citizen_{i}_{rand_prefab_idx}";

                npc.tag = "Citizens";

                // Ensure the animator is enabled
                Animator npcAnimator = npc.GetComponent<Animator>();
                if (npcAnimator is not null)
                {
                    // npcAnimator.SetTrigger("Idle"); // Ensure NPC starts animating
                    Citizen.go_citizens.Add(npc);
                }
                else
                {
                    throw new UnityException("NPCAnimator was null");
                }
            }

            Citizen.citizens_enabled = true;
        }
    }
}
