using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Citizens;

public class PollutionSystem : MonoBehaviour
{
    private GridSystem gridSystem;

    private const float POLLUTION_INTERVAL = 5f;
    private const float INDUSTRIAL_POLLUTION = 2f;
    private const float COMMERCIAL_POLLUTION = 0.5f;
    private const float RESIDENTIAL_POLLUTION = 0.2f;
    private const float POLLUTION_DECAY_RATE = 0.05f;
    private const float MAX_POLLUTION = 10f;
    private const float SPREAD_FACTOR = 0.1f;
    private const float MIN_SPREAD = 0.5f;
    private const float MIN_VISIBLE = 0.1f;

    [SerializeField] private bool showPollution = false;
    [SerializeField] private Color lowPollution = Color.yellow;
    [SerializeField] private Color highPollution = Color.red;

    [SerializeField] private float overlayHeight = 0.1f;
    [SerializeField] private float minAlpha = 0.2f;
    [SerializeField] private float maxAlpha = 0.7f;

    private float timer = 0f;
    private float[,] pollutionLevels;
    private float[,] pollutionSpreadLevels;
    private Dictionary<Vector2Int, GameObject> pollutionVisuals = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, ZoneType> cellZoneTypes = new Dictionary<Vector2Int, ZoneType>();
    private Material pollutionMaterial;

    private void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        // Setup arrays
        pollutionLevels = new float[gridSystem.width, gridSystem.height];
        pollutionSpreadLevels = new float[gridSystem.width, gridSystem.height];
        // Shared material
        pollutionMaterial = new Material(Shader.Find("Transparent/Diffuse"));
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= POLLUTION_INTERVAL)
        {
            UpdatePollution();
            SpreadPollution();

            if (showPollution)
            {
                UpdateVisuals();
            }

            timer = 0f;
        }

        // Toggle visuals
        if (Input.GetKeyDown(KeyCode.P))
        {
            showPollution = !showPollution;
            ToggleVisuals();
        }
    }

    private void UpdatePollution()
    {
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                // Decay
                pollutionLevels[x, y] = Mathf.Max(0, pollutionLevels[x, y] - POLLUTION_DECAY_RATE);

                GameObject cellObject = gridSystem.GetCellAt(x, y);
                if (cellObject == null) continue;

                Cell cell = cellObject.GetComponent<Cell>();
                if (cell == null) continue;


                cellZoneTypes[new Vector2Int(x, y)] = cell.zone_type;


                switch (cell.zone_type)
                {
                    case ZoneType.Industrial:
                        pollutionLevels[x, y] += INDUSTRIAL_POLLUTION;
                        break;
                    case ZoneType.Commercial:
                        pollutionLevels[x, y] += COMMERCIAL_POLLUTION;
                        break;
                    case ZoneType.Residential:
                        pollutionLevels[x, y] += RESIDENTIAL_POLLUTION;
                        break;
                }

                // Cap 
                pollutionLevels[x, y] = Mathf.Min(pollutionLevels[x, y], MAX_POLLUTION);
            }
        }
    }

    private void SpreadPollution()
    {
        // Reset
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                pollutionSpreadLevels[x, y] = 0f;
            }
        }


        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                // Skip low pollution
                if (pollutionLevels[x, y] < MIN_SPREAD) continue;

                float spreadAmount = pollutionLevels[x, y] * SPREAD_FACTOR;
                pollutionSpreadLevels[x, y] -= spreadAmount * 8; // Remove spredd from source cell

                // Spread to neighbors
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;

                        int nx = x + dx;
                        int ny = y + dy;

                        if (nx >= 0 && nx < gridSystem.width && ny >= 0 && ny < gridSystem.height)
                        {
                            pollutionSpreadLevels[nx, ny] += spreadAmount;
                        }
                    }
                }
            }
        }

        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                pollutionLevels[x, y] = Mathf.Max(0, pollutionLevels[x, y] + pollutionSpreadLevels[x, y]);
            }
        }
    }

    private void UpdateVisuals()
    {
        for (int x = 0; x < gridSystem.width; x++)
        {
            for (int y = 0; y < gridSystem.height; y++)
            {
                UpdateCellVisual(new Vector2Int(x, y));
            }
        }
    }

    private void UpdateCellVisual(Vector2Int cell)
    {
        GameObject cellObject = gridSystem.GetCellAt(cell.x, cell.y);
        if (cellObject == null) return;

        float pollution = pollutionLevels[cell.x, cell.y];
        GameObject pollutionOverlay;


        if (!pollutionVisuals.TryGetValue(cell, out pollutionOverlay) || pollutionOverlay == null)
        {
            pollutionOverlay = GameObject.CreatePrimitive(PrimitiveType.Quad);
            pollutionOverlay.name = $"Pollution" + cell.x + " , " + cell.y;

            Renderer renderer = pollutionOverlay.GetComponent<Renderer>();
            renderer.material = new Material(pollutionMaterial);
            pollutionOverlay.transform.rotation = Quaternion.Euler(90, 0, 0);

            pollutionVisuals[cell] = pollutionOverlay;
        }


        if (pollution > MIN_VISIBLE)
        {
            pollutionOverlay.SetActive(true);

            float percent = Mathf.Clamp01(pollution / MAX_POLLUTION);

            Color finalColor = Color.Lerp(lowPollution, highPollution, percent);
            finalColor.a = Mathf.Lerp(minAlpha, maxAlpha, percent);

            Renderer renderer = pollutionOverlay.GetComponent<Renderer>();
            renderer.material.color = finalColor;

            // Position
            Vector3 cellPosition = cellObject.transform.position;
            pollutionOverlay.transform.position = new Vector3(
                cellPosition.x,
                cellPosition.y + overlayHeight,
                cellPosition.z
            );
        }
        else
        {
            pollutionOverlay.SetActive(false);
        }
    }

    private void ToggleVisuals()
    {
        if (showPollution)
        {
            UpdateVisuals();
            Debug.Log("Pollution on");
        }
        else
        {
            foreach (var overlay in pollutionVisuals.Values)
            {
                if (overlay != null)
                {
                    overlay.SetActive(false);
                }
            }
            Debug.Log("Pollution off");
        }
    }
}