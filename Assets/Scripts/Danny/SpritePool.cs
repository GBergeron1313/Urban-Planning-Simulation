using System.Collections.Generic;
using UnityEngine;

public class SpritePool : MonoBehaviour
{
    public GameObject NPCPrefab;
    public int PoolSize = 1000;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < PoolSize; i++)
        {
            GameObject npc = Instantiate(NPCPrefab);
            npc.SetActive(false);
            pool.Enqueue(npc);
        }
    }

    public GameObject GetNPC()
    {
        if (pool.Count > 0)
        {
            GameObject npc = pool.Dequeue();
            npc.SetActive(true);
            return npc;
        }
        return null;
    }

    public void ReturnNPC(GameObject npc)
    {
        npc.SetActive(false);
        pool.Enqueue(npc);
    }
}