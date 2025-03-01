using System.Collections.Generic;
using Citizens;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Danny
{
    public class SpawnManager : MonoBehaviour
    {
        public GameObject[] npcPrefabs; // Assign 4 prefabs in Inspector
        public Button npcButton; // Assign your button in Inspector
        public Transform spawnArea; // Assign an area where NPCs will spawn
        public static readonly int npcCount = 100; // Number of NPCs to spawn

        void Start()
        {
            npcButton.onClick.AddListener(SpawnNPCs);
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
                npc.name = $"Citizen_{i}";

                npc.tag = "Citizens";

                // Ensure the animator is enabled
                Animator npcAnimator = npc.GetComponent<Animator>();
                if (npcAnimator is not null)
                {
                    // npcAnimator.SetTrigger("Idle"); // Ensure NPC starts animating
                    Building.go_citizens.Add(npc);
                    string foo = npc.Serialize().json;
                    print(foo);
                }
                else
                {
                    throw new UnityException("NPCAnimator was null");
                }
            }

            Building.citizens_enabled = true;
        }
    }
}