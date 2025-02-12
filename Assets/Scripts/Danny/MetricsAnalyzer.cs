using System.Collections.Generic;
using UnityEngine;

public class MetricsAnalyzer : MonoBehaviour
{
    public List<NPC> npcs;

    void Update()
    {
        UpdateMetrics();
    }

    private void UpdateMetrics()
    {
        int employedCount = 0;
        foreach (var npc in npcs)
        {
            if (npc.Stats.IsEmployed) employedCount++;
        }

        Debug.Log($"Total NPCs: {npcs.Count}, Employed: {employedCount}");
    }
}