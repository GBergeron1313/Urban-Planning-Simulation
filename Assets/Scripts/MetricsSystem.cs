using Citizens;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

public class MetricsSystem : MonoBehaviour
{
    private GridSystem gridSystem;
    private PollutionSystem pollutionSystem;

    // cell script
    private int totalCells = 0;
    private int residentialCells = 0;
    private int commericalCells = 0;
    private int industrialCells = 0;

    // building script
    private int totalBuildings = 0;
    private int residentialBuildings = 0;
    private int commericalBuildings = 0;
    private int industrialBuildings = 0;

    // citizen script
    private int totalCitizens = 0;
    private int avgTravelTime = 0;
    private int maxTravelTime = 0;
    private int minTravelTime = 0;
    private int totalTravelTime = 0;


    // pollution script
    private float avgPollution = 0;
    private float maxGridPollution = 0;

    Dictionary<int, Vector2Int> citizenPositions = new Dictionary<int, Vector2Int>();
    Dictionary<Citizen, float> pathLengths = new Dictionary<Citizen, float>();

    // Start is called before the first frame update
    void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        pollutionSystem = GetComponent<PollutionSystem>();

    }

    // Update is called once per frame
    void Update()
    {
        CellCount();
        BuildingCount();
        CitizenCount();
        PollutionCount();
        SaveMetricsData();
        CitizenDensityMax();
        CitizenTravelLength();
    }

    public void CellCount()
    {
        totalCells = gridSystem.width * gridSystem.height;
        residentialCells = 0;
        commericalCells = 0;
        industrialCells = 0;

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
                    }
                }

            }
        }
    }

    public void BuildingCount()
    {
        residentialBuildings = 0;
        commericalBuildings = 0;
        industrialBuildings = 0;

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                GameObject cellObject = gridSystem.GetCellAt(x, y);
                Cell cell = cellObject.GetComponent<Cell>();

                if (cell.contents != null)
                {
                    totalBuildings++;

                    switch (cell.zone_type)
                    {
                        case ZoneType.Residential: residentialBuildings++;break;
                        case ZoneType.Commercial: commericalBuildings++; break;
                        case ZoneType.Industrial: industrialBuildings++; break;
                    }
                }

            }
        }

        totalBuildings = residentialBuildings + commericalBuildings + industrialBuildings;
    }

    public void CitizenCount()
    {
        totalCitizens = 0;

        Citizen[] citizens = FindObjectsOfType<Citizen>();

        totalCitizens = citizens.Length;
    }

    public (int x, int y) CitizenDensityMax()
    {
        citizenPositions.Clear();

        Citizen[] citizens = FindObjectsOfType<Citizen>();

        for (int i = 0; i < citizens.Length; i++)
        {
            Vector3 position = citizens[i].transform.position;

            int x = Convert.ToInt32(position.x);
            int y = Convert.ToInt32(position.y);

            Vector2Int citizenPos = new Vector2Int(x, y);

            citizenPositions.Add(i, citizenPos);
        }

        Dictionary<int, int> xPos = new Dictionary<int, int>();
        Dictionary<int, int> yPos = new Dictionary<int, int>();

        foreach (Vector2Int pos in citizenPositions.Values)
        {
            if (!xPos.ContainsKey(pos.x))
            {
                xPos.Add(pos.x, 1);
            }
            else
            {
                xPos[pos.x]++;
            }

            if (!yPos.ContainsKey(pos.y))
            {
                yPos.Add(pos.y, 1);
            }
            else
            {
                yPos[pos.y]++;
            }

        }

        int maxX = 0;
        int maxY = 0;

        foreach (KeyValuePair<int, int> pair in xPos)
        {
            if (pair.Value > maxX)
            {
                maxX = pair.Value;
            }
        }
        foreach (KeyValuePair<int, int> pair in yPos)
        {
            if (pair.Value > maxY)
            {
                maxY = pair.Value;
            }
        }

        return (maxX, maxY);

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
        List<Vector2Int > roadTracker = new List<Vector2Int>();

        for(int x = 0; x < gridSystem.width; x++)
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
        pathLengths.Clear();
        
        Citizen[] citizens = FindObjectsOfType<Citizen>();
        List<Vector2Int> roadCells = RoadCells();

        foreach (Citizen c in citizens)
        {
            NavMeshAgent navMeshAgent = c.GetComponent<NavMeshAgent>();
            if (navMeshAgent == null) continue;

            Vector3 startingPoint = c.transform.position;
            Vector3 endPoint = navMeshAgent.destination;

            NavMeshPath path = new NavMeshPath();
            navMeshAgent.CalculatePath(endPoint, path);

            float pathLength = 0f;
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                for (int i = 0; i < path.corners.Length; i++)
                {
                    pathLength += Vector3.Distance(path.corners[i], path.corners[i + 1]);
                }

                pathLengths.Add(c, pathLength);

            }
        }

    }

    public void TravelEfficency()
    {
        maxTravelTime = (int)pathLengths.Values.Max();
        minTravelTime = (int)pathLengths.Values.Min();
        avgTravelTime = (int)pathLengths.Values.Average();
        totalTravelTime = (int)pathLengths.Values.Sum();
    }

    public void SaveMetricsData()
    {

        string saveFolder = "MetricsData";
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
               "-------------------------------------------------------" +
               " Buildings: " + totalBuildings + Environment.NewLine +
               " Residential: " + residentialBuildings + Environment.NewLine +
               " Commercial: " + commericalBuildings + Environment.NewLine +
               " Industrial: " + industrialBuildings + Environment.NewLine +
               "-------------------------------------------------------" +
               " Citizens: " + totalCitizens + Environment.NewLine +
               " Citizens Highest Travel Time: " + maxTravelTime + Environment.NewLine +
               " Citizens Lowest Travel Time: " + minTravelTime + Environment.NewLine +
               " Citizens Avergae Travel Time: " + avgTravelTime + Environment.NewLine +
               " Citizens Total Travel Time: " + totalTravelTime + Environment.NewLine +
               " Citizen Highest Density Cell: " + CitizenDensityMax() + Environment.NewLine +
               "-------------------------------------------------------" +
               " Average Pollution: " + avgPollution + Environment.NewLine +
               " Max Pollution: " + maxGridPollution + Environment.NewLine;

        try
        {
            File.WriteAllText(filePath, dataToSave);
            Debug.Log("Metrics Saved");
        }
        catch (Exception ex)
        {
            Debug.LogError("ERROR: " + ex.Message);
        }

    }



















}
