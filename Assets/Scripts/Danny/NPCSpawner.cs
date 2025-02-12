using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public SpritePool spritePool;
    public int initialPopulation = 100;

    void Start()
    {
        SpawnInitialPopulation();
    }

    private void SpawnInitialPopulation()
    {
        for (int i = 0; i < initialPopulation; i++)
        {
            SpawnNPC();
        }
    }

    private void SpawnNPC()
    {
        GameObject npcObject = spritePool.GetNPC();
        if (npcObject != null)
        {
            npcObject.transform.position = new Vector3(Random.Range(0, 10), 0, Random.Range(0, 10));
            NPC npc = npcObject.GetComponent<NPC>();
            npc.ID = System.Guid.NewGuid().ToString();
            npc.Needs = new List<Need> { new Need { Name = "Work", SatisfactionLevel = 0.0f } };
            npc.Stats = new Demographics { Age = Random.Range(20, 60), IsEmployed = false, Income = 0 };
        }
    }
}