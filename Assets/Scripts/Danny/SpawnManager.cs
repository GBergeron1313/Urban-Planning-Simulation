using UnityEngine;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] npcPrefabs; // Assign 4 prefabs in Inspector
    public Button npcButton; // Assign your button in Inspector
    public Transform spawnArea; // Assign an area where NPCs will spawn
    public int npcCount = 100; // Number of NPCs to spawn

    void Start()
    {
        npcButton.onClick.AddListener(SpawnNPCs);
    }

    void SpawnNPCs()
    {
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-5f, 5f),  // Adjust X range for a closer spread
                0f, // Adjust Y based on terrain
                Random.Range(-5f, 5f)   // Adjust Z range
            );
            int randomIndex = Random.Range(0, npcPrefabs.Length);
            GameObject npc = Instantiate(npcPrefabs[randomIndex], randomPosition, Quaternion.identity);

            // Ensure the animator is enabled
            Animator npcAnimator = npc.GetComponent<Animator>();
            if (npcAnimator != null)
            {
                npcAnimator.SetTrigger("Idle");  // Ensure NPC starts animating
            }
        }
    }
}