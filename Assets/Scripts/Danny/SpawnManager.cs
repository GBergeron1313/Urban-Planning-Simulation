using System.Collections.Generic;
using Citizens;
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
            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;


            Building.go_citizens = new List<GameObject>(npcCount);

            for (int i = 0; i < npcCount; i++)
            {
                string[] name_id_index = citizen_names[i].Split('_');
                int prefab_index = int.Parse(name_id_index[2]);
                GameObject npc =
                    Instantiate(npcPrefabs[prefab_index],
                            positions[i],
                            Quaternion.identity);

                npc.name = citizen_names[i];
                npc.transform.position = positions[i];
                npc.transform.position += Vector3.up * 10;


                npc.tag = "Citizens";

                // Ensure the animator is enabled
                Animator npcAnimator = npc.GetComponent<Animator>();
                if (npcAnimator is not null)
                {
                    var nma = npcAnimator.GetComponent<NavMeshAgent>();
                    Assert.IsTrue(nma.Warp(positions[i]));
                    Assert.IsTrue(nma.SetDestination(destinations[i]));
                    nma.radius /= 10.0f;
                    nma.acceleration /= 10.0f;
                    nma.speed /= 10.0f;
                    nma.updateRotation = true;
                    nma.autoRepath = true;
                    Building.go_citizens.Add(npc);
                }
                else
                {
                    throw new UnityException("NPCAnimator was null");
                }
            }

            spawned_and_moving = true;

            Citizen.citizens_enabled = true;
        }

        void SpawnNPCs()
        {
            // Don't let them spawn more NPCs
            npcButton.interactable = false;
            npcButton.onClick = null;
            npcButton.enabled = false;


            Building.go_citizens = new List<GameObject>(npcCount);

            for (int i = 0; i < npcCount; i++)
            {
                Vector3 randomPosition = new Vector3(
                    Random.Range(-5f, 5f), // Adjust X range for a closer spread
                    0f, // Adjust Y based on terrain
                    Random.Range(-5f, 5f) // Adjust Z range
                );
                int randomIndex = Random.Range(0, npcPrefabs.Length);
                GameObject npc = Instantiate(npcPrefabs[randomIndex], randomPosition, Quaternion.identity);

                // For Saving and reloading, each citizen needs a unique name.
                // Saving the prefab index is somewhat hacky, but it works for now.
                // TODO: Make a real "Citizen" class to store this kind of info.
                npc.name = $"Citizen_{i}_{randomIndex}";

                npc.tag = "Citizens";

                // Ensure the animator is enabled
                Animator npcAnimator = npc.GetComponent<Animator>();
                if (npcAnimator is not null)
                {
                    // npcAnimator.SetTrigger("Idle"); // Ensure NPC starts animating
                    Building.go_citizens.Add(npc);
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
