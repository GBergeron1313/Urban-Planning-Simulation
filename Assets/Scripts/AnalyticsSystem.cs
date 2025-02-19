using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnalyticsSystem : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private GridSystem gridSystem;
    [SerializeField] private PopulationSystem populationSystem;
    [SerializeField] private float updateInterval = 1f;

    [Header("Debug Settings")]
    [SerializeField] private bool debugMode = false;

    // Analytics data storage
    private Dictionary<string, float> currentStats = new Dictionary<string, float>();
    private List<TimeSeriesData> historicalData = new List<TimeSeriesData>();
    private float timeSinceLastUpdate = 0f;

    // Events for other systems to subscribe to
    public event Action<Dictionary<string, float>> OnStatsUpdated;

    void Start()
    {
        InitializeAnalytics();
    }

    public void UpdateAnalytics()
    {
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate >= updateInterval)
        {
            UpdateStats();
            timeSinceLastUpdate = 0f;
        }
    }

    private void InitializeAnalytics()
    {
        // Initialize tracking categories
        currentStats.Clear();
        currentStats.Add("population", 0);
        currentStats.Add("employmentRate", 0);
        currentStats.Add("residentialZones", 0);
        currentStats.Add("commercialZones", 0);
        currentStats.Add("industrialZones", 0);
        currentStats.Add("averageHappiness", 0);

        if (debugMode)
            Debug.Log("Analytics System Initialized");
    }

    private void UpdateStats()
    {
        // Update current statistics
        UpdateZoneStats();
        UpdatePopulationStats();

        // Store historical data
        StoreTimeSeriesData();

        // Notify subscribers
        OnStatsUpdated?.Invoke(currentStats);

        if (debugMode)
            LogCurrentStats();
    }

    private void UpdateZoneStats()
    {
        int residential = 0, commercial = 0, industrial = 0;

        currentStats["residentialZones"] = residential;
        currentStats["commercialZones"] = commercial;
        currentStats["industrialZones"] = industrial;
    }

    private void UpdatePopulationStats()
    {
        float population = 0; // Get from PopulationSystem
        float employed = 0;   // Get from PopulationSystem
        float happiness = 0;  // Calculate from various factors

        currentStats["population"] = population;
        currentStats["employmentRate"] = employed > 0 ? (employed / population) * 100 : 0;
        currentStats["averageHappiness"] = happiness;
    }

    private void StoreTimeSeriesData()
    {
        TimeSeriesData timeData = new TimeSeriesData
        {
            timestamp = DateTime.Now,
            stats = new Dictionary<string, float>(currentStats)
        };

        historicalData.Add(timeData);

        int maxDataPoints = 1440; // 24 * 60
        if (historicalData.Count > maxDataPoints)
        {
            historicalData.RemoveAt(0);
        }
    }

    public void ExportCSV(string filename)
    {
        try
        {
            string path = System.IO.Path.Combine(Application.persistentDataPath, filename);
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path))
            {
                // Write header
                writer.WriteLine("Timestamp," + string.Join(",", currentStats.Keys));

                // Write data
                foreach (var data in historicalData)
                {
                    string line = data.timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                    foreach (var stat in currentStats.Keys)
                    {
                        line += "," + data.stats[stat].ToString();
                    }
                    writer.WriteLine(line);
                }
            }

            Debug.Log($"CSV exported to: {path}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error exporting CSV: {e.Message}");
        }
    }

    public Report GenerateReport()
    {
        return new Report
        {
            timestamp = DateTime.Now,
            totalPopulation = (int)currentStats["population"],
            employmentRate = currentStats["employmentRate"],
            zoneDistribution = new ZoneDistribution
            {
                residential = (int)currentStats["residentialZones"],
                commercial = (int)currentStats["commercialZones"],
                industrial = (int)currentStats["industrialZones"]
            },
            averageHappiness = currentStats["averageHappiness"]
        };
    }

    private void LogCurrentStats()
    {
        string stats = "Current City Stats:\n";
        foreach (var stat in currentStats)
        {
            stats += $"{stat.Key}: {stat.Value}\n";
        }
        Debug.Log(stats);
    }

    // For SimCore save/load
    public string GetSerializedData()
    {
        AnalyticsData data = new AnalyticsData
        {
            currentStats = currentStats,
            historicalData = historicalData
        };
        return JsonUtility.ToJson(data);
    }

    public void LoadFromData(string jsonData)
    {
        AnalyticsData data = JsonUtility.FromJson<AnalyticsData>(jsonData);
        currentStats = data.currentStats;
        historicalData = data.historicalData;
    }
}

// Data structures
[Serializable]
public class TimeSeriesData
{
    public DateTime timestamp;
    public Dictionary<string, float> stats;
}

[Serializable]
public class Report
{
    public DateTime timestamp;
    public int totalPopulation;
    public float employmentRate;
    public ZoneDistribution zoneDistribution;
    public float averageHappiness;
}

[Serializable]
public class ZoneDistribution
{
    public int residential;
    public int commercial;
    public int industrial;
}

[Serializable]
public class AnalyticsData
{
    public Dictionary<string, float> currentStats;
    public List<TimeSeriesData> historicalData;
}
