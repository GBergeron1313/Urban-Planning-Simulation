using Citizens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class MetricsSystem : MonoBehaviour
{
    private GridSystem gridSystem;
    private PollutionSystem pollutionSystem;

    // cell script
    private float totalCells = 0;
    private float residentialCells = 0;
    private float commericalCells = 0;
    private float industrialCells = 0;
    private float restrictedCells = 0;  // restricted zones SHOULD NOT to be able to have assigned buildings

    // building script
    private float totalBuildings = 0;
    private float residentialBuildings = 0;
    private float commericalBuildings = 0;
    private float industrialBuildings = 0;
    private float restrictedBuildings = 0;  // restricted zones SHOULD NOT to be able to have assigned buildings

    // citizen script
    private float totalCitizens = 0;
    private float avgTravelTime = 0;
    private float maxTravelTime = 0;
    private float minTravelTime = 0;
    private float totalTravelTime = 0;


    // pollution script
    private float avgPollution = 0;
    private float maxGridPollution = 0;

    private int frameCount = 0;

    Dictionary<Citizen, float> pathLengths = new Dictionary<Citizen, float>();

    public GameObject stats;

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        pollutionSystem = GetComponent<PollutionSystem>();

    }

    // Update is called once per frame
    void Update()
    {

        frameCount++;

        CellCount();
        BuildingCount();
        CitizenCount();
        PollutionCount();
        CitizenDensityMax();
        CitizenTravelLength();
        TravelEfficency();

        if (frameCount == 500)
        {
            SaveMetricsData();
            frameCount = 0;
        }

    }

    public void CellCount()
    {
        totalCells = gridSystem.width * gridSystem.height;
        residentialCells = 0;
        commericalCells = 0;
        industrialCells = 0;
        restrictedCells = 0;

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                GameObject cellObject = gridSystem.GetCellAt(x, y);
                Cell cell = cellObject.GetComponent<Cell>();

                if (cell != null)
                {
                    switch (cell.zone_type)
                    {
                        case ZoneType.Residential: residentialCells++; break;
                        case ZoneType.Commercial: commericalCells++; break;
                        case ZoneType.Industrial: industrialCells++; break;
                        case ZoneType.Restricted: restrictedCells++; break;
                    }
                }

            }
        }
    }

    public void BuildingCount()
    {
        Debug.Log("Starting building count...");
        residentialBuildings = 0;
        commericalBuildings = 0;
        industrialBuildings = 0;
        restrictedBuildings = 0;

        Building[] buildings = FindObjectsOfType<Building>();
        if (buildings == null || buildings.Length == 0) return;

        foreach (Building building in buildings)
        {
            if (building.building_type == ZoneType.Residential)
            {
                residentialBuildings++;
            }
            else if (building.building_type == ZoneType.Commercial)
            {
                commericalBuildings++;
            }
            else if (building.building_type == ZoneType.Industrial)
            {
                industrialBuildings++;
            }
            else if (building.building_type != ZoneType.Restricted)  // should be removed later
            {
                restrictedBuildings++;
            }
        }

        totalBuildings = residentialBuildings + commericalBuildings + industrialBuildings + restrictedBuildings;
        Debug.Log($"Building count complete - Residential: {residentialBuildings}, Commercial: {commericalBuildings}, Industrial: {industrialBuildings}, Total: {totalBuildings}");
    }

    public void CitizenCount()
    {
        Debug.Log("Starting citizen count...");

        totalCitizens = 0;

        Citizen[] citizens = FindObjectsOfType<Citizen>();

        totalCitizens = citizens.Length;

        Debug.Log($"Found {totalCitizens} citizens");
    }

    public Vector2Int CitizenDensityMax()
    {
        Citizen[] citizens = FindObjectsOfType<Citizen>();
        Dictionary<Vector2Int, int> posCount = new Dictionary<Vector2Int, int>();

        Vector2Int maxPos = new Vector2Int();
        int count = 0;

        for (int i = 0; i < citizens.Length; i++)
        {
            int posX = Mathf.FloorToInt(citizens[i].transform.position.x);
            int posY = Mathf.FloorToInt(citizens[i].transform.position.y);

            Vector2Int position = new Vector2Int(posX, posY);

            if (posCount.ContainsKey(position))
            {
                posCount[position]++;
            }
            else
            {
                posCount[position] = 1;
            }

        }

        foreach (var person in posCount)
        {
            if (person.Value > count)
            {
                count = person.Value;
                maxPos = person.Key;
            }
        }

        return maxPos;

    }

    public void PollutionCount()
    {
        avgPollution = 0;
        maxGridPollution = 0;
        float sumPollution = 0;

        float[,] pollutionArray = pollutionSystem.AccessPollutionData();

        int width = pollutionArray.GetLength(0);
        int height = pollutionArray.GetLength(1);


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                sumPollution += pollutionArray[x, y];

                if (pollutionArray[x, y] > maxGridPollution)
                {
                    maxGridPollution = pollutionArray[x, y];
                }
            }
        }

        avgPollution = sumPollution / (width * height);

    }

    public List<Vector2Int> RoadCells()
    {
        List<Vector2Int> roadTracker = new List<Vector2Int>();

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                GameObject cellObject = gridSystem.GetCellAt(x, y);
                if (cellObject == null) continue;

                Cell cell = cellObject.GetComponent<Cell>();
                if (cell == null) continue;

                if (cell.cell_type == CellType.Road)
                {
                    roadTracker.Add(new Vector2Int(x, y));
                }

            }
        }

        return roadTracker;

    }

    public List<Vector2Int> NeighboringRoadCells(Vector2Int cell, List<Vector2Int> roadCells)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // right
        Vector2Int rightCell = new Vector2Int(cell.x + 1, cell.y);
        if (roadCells.Contains(rightCell))
        {
            neighbors.Add(rightCell);
        }

        // left
        Vector2Int leftCell = new Vector2Int(cell.x - 1, cell.y);
        if (roadCells.Contains(leftCell))
        {
            neighbors.Add(leftCell);
        }

        // up
        Vector2Int upCell = new Vector2Int(cell.x, cell.y + 1);
        if (roadCells.Contains(upCell))
        {
            neighbors.Add(upCell);
        }

        // down
        Vector2Int downCell = new Vector2Int(cell.x, cell.y - 1);
        if (roadCells.Contains(downCell))
        {
            neighbors.Add(downCell);
        }

        return neighbors;
    }

    public void CitizenTravelLength()
    {
        Debug.Log("Starting citizen travel length calculation...");
        pathLengths.Clear();

        Citizen[] citizens = FindObjectsOfType<Citizen>();
        List<Vector2Int> roadCells = RoadCells();

        foreach (Citizen c in citizens)
        {
            NavMeshAgent navMeshAgent = c.GetComponent<NavMeshAgent>();
            if (navMeshAgent == null || !navMeshAgent.isOnNavMesh) continue;

            Vector3 startingPoint = c.transform.position;
            Vector3 endPoint = navMeshAgent.destination;

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(endPoint, path);

            float pathLength = 0f;
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < path.corners.Length - 1; i++)
                {
                    pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }

                pathLengths.Add(c, pathLength);

            }
        }

        Debug.Log("Starting citizen travel length completed");

    }

    public void TravelEfficency()
    {
        float[] values = pathLengths.Values.ToArray();
        if (values.Length == 0)
        {
            maxTravelTime = 0;
            minTravelTime = 0;
            avgTravelTime = 0;
            totalTravelTime = 0;
            return;
        }

        maxTravelTime = values.Max();
        minTravelTime = values.Min();
        avgTravelTime = values.Average();
        totalTravelTime = values.Sum();
    }

    public void MetricReload()
    {
        CellCount();
        BuildingCount();
        CitizenCount();
        PollutionCount();
        CitizenDensityMax();
        CitizenTravelLength();
        TravelEfficency();
        SaveMetricsData();
    }

    public void SaveMetricsData()
    {
        stats = GameObject.Find("CreateBuilding");

        string saveFolder = Path.Combine(Application.dataPath, "MetricsData");
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }

        string fileName = "cityData.txt";
        string filePath = Path.Combine(saveFolder, fileName);

        string dataToSave =

               " Cells: " + totalCells + Environment.NewLine +
               " Residential: " + residentialCells + Environment.NewLine +
               " Commercial: " + commericalCells + Environment.NewLine +
               " Industrial: " + industrialCells + Environment.NewLine +
               " Restricted: " + restrictedCells + Environment.NewLine +
               "-------------------------------------------------------" + Environment.NewLine +
               " Buildings: " + totalBuildings + Environment.NewLine +
               " Residential: " + residentialBuildings + Environment.NewLine +
               " Commercial: " + commericalBuildings + Environment.NewLine +
               " Industrial: " + industrialBuildings + Environment.NewLine +
               " Restricted: " + restrictedBuildings + Environment.NewLine +
               "-------------------------------------------------------" + Environment.NewLine +
               " Citizens: " + totalCitizens + Environment.NewLine +
               " Citizens Highest Travel Time: " + maxTravelTime + Environment.NewLine +
               " Citizens Lowest Travel Time: " + minTravelTime + Environment.NewLine +
               " Citizens Avergae Travel Time: " + avgTravelTime + Environment.NewLine +
               " Citizens Total Travel Time: " + totalTravelTime + Environment.NewLine +
               " Citizen Highest Density Cell: " + CitizenDensityMax() + Environment.NewLine +
               "-------------------------------------------------------" + Environment.NewLine +
               " Average Pollution: " + avgPollution + Environment.NewLine +
               " Max Pollution: " + maxGridPollution + Environment.NewLine +
               "-------------------------------------------------------" + Environment.NewLine +
               "Pollution Per Citzen: " + Cell.creator.polPerCit + "%" + Environment.NewLine +
               "Noise Per Citzen: " + Cell.creator.noisePerCit + "%" + Environment.NewLine;

        if (Cell.creator.polPerCit >= 30)
        {
            dataToSave += "Pollution Levels Too High!!!" + Environment.NewLine;
        }
        else
        {
            dataToSave += "Pollution at Acceptable Levels" + Environment.NewLine;
        }
        if (Cell.creator.polPerCit >= 30)
        {
            dataToSave += "Noise Levels Too High!!!!";
        }
        else
        {
            dataToSave += "Noise at Acceptable Levels";
        }

        GameObject analyticMenu = GameObject.Find("Analytics Menu");
        analyticMenu.GetComponent<AnalyticsMenu>().text.text = dataToSave;

        try
        {
            File.WriteAllText(filePath, dataToSave);
            Debug.Log("Metrics Saved to: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR: " + ex.Message);
        }

    }
}
