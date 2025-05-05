using BuildingUtils;
using UnityEngine;
using UnityEngine.UI;

public enum ZoneType
{
    None,
    Residential,
    Commercial,
    Industrial,
    Restricted
}

public enum BuildingMode
{
    None,
    PlacingBuilding,
    PlacingRoad,
    MarkingZoneType,
    Removal,

    // Always keep TotalModes at the end
    TotalModes
}


public class GridSystem : MonoBehaviour
{
    [Header("Grid Settings")] public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float gridHeight = 0f;
    public Material gridMaterial;

    [Header("Zone Settings")] public Color residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public Color commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public Color industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public Color restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);

    [Header("Interaction Settings")] public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;

    private GameObject[,] gridCells;
    private ZoneType[,] zoneGrid;
    private Material defaultMaterial;
    private GameObject lastHovered;
    private GameObject selectedCell;
    private bool[,] filledCells;

    public GameObject uiText;
    private TMPro.TextMeshProUGUI text_display;

    public static readonly Color g_residentialZoneColor = new Color(0.2f, 0.8f, 0.2f, 0.5f);
    public static readonly Color g_commercialZoneColor = new Color(0.2f, 0.2f, 0.8f, 0.5f);
    public static readonly Color g_industrialZoneColor = new Color(0.8f, 0.2f, 0.2f, 0.5f);
    public static readonly Color g_restrictedZoneColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
    public static Color g_defaultColor = Color.green;
    public static readonly Color g_hoverColor = Color.yellow;

    private Button BTN_change_building_mode;

    //public GameObject buildingUI;
    public Button zoneButton;

    void Awake()
    {
        BTN_change_building_mode = GameObject.Find("ModeButton").GetComponent<Button>();
        BTN_change_building_mode.onClick.AddListener(Cell.CycleBuildingMode);
        text_display = uiText.GetComponent<TMPro.TextMeshProUGUI>();
    }

    /// Initializes the grid arrays and generates the grid structure
    void Start()
    {
        // Initialize arrays to store grid cells, zone types, and occupancy status
        gridCells = new GameObject[width, height];
        zoneGrid = new ZoneType[width, height];
        filledCells = new bool[width, height];

        // Initialize all cells as empty and with no zone type
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                filledCells[x, z] = false;
                zoneGrid[x, z] = ZoneType.None;
            }
        }


        //zoneButton.onClick.AddListener(Cell.CycleZoneType);

        GenerateGrid();
        InvokeRepeating("check_rot_model", 5f, 10f);
    }

    public void ClearGridReset()
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Don't want dangling prefabs
                GameObject.Destroy(gridCells[x, z]);
            }
        }
        Cell.all_cells.Clear();
        Start();
    }


    /// Creates the visual grid structure with cell objects and colliders
    void GenerateGrid()
    {
        // Create a parent object to organize all grid cells
        GameObject gridParent = new GameObject("Grid");
        gridParent.transform.parent = transform;

        Cell.grid = this;

        // Calculate starting position to center the grid
        Vector3 startPos = transform.position - new Vector3(width * cellSize / 2f, 0, height * cellSize / 2f);

        // Create individual grid cells
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                // Create a quad primitive for the cell
                GameObject cell = GameObject.CreatePrimitive(PrimitiveType.Quad);
                cell.name = $"Cell_{x}_{z}"; // Name format used for position lookup
                cell.transform.parent = gridParent.transform;

                // Position the cell in the grid
                Vector3 pos = startPos + new Vector3(x * cellSize, gridHeight, z * cellSize);
                cell.transform.position = pos;
                cell.transform.rotation = Quaternion.Euler(90, 0, 0); // Rotate to lay flat
                cell.transform.localScale = new Vector3(cellSize, cellSize, 1);

                // Apply material if provided
                if (gridMaterial != null)
                {
                    Material cellMaterial = new Material(gridMaterial);
                    cell.GetComponent<Renderer>().material = cellMaterial;
                }

                // Store reference to cell
                gridCells[x, z] = cell;

                Cell c = cell.AddComponent<Cell>();

                c.location = new Vector2Int(x, z);
                c.SetZoneTypeAndUpdate(ZoneType.None);
                c.SetWalkableAndUpdate(false);
                c.register();
            }
        }

        // Store reference to default material for color restoration
        if (gridCells[0, 0] != null)
        {
            defaultMaterial = new Material(gridCells[0, 0].GetComponent<Renderer>().material);
        }
    }

    private float t_last_rot_model_check = 10f;

    /// Updates grid state each frame
    public void Update()
    {
        if (t_last_rot_model_check > 0)
        {
            t_last_rot_model_check -= Time.unscaledDeltaTime;
        }
        else
        {
            t_last_rot_model_check = 10f;
            check_rot_model();
        }
        HandleGridInteraction();
    }

    private void check_rot_model()
    {
        var roads = Cell.all_cells.FindAll((Cell c) => c.cell_type.is_road());
        foreach (var road in roads)
        {
            int neighbors = road.number_of_neighbors();
            if (neighbors != road.contents.model.appropriate_neighbor_count())
            {
                BuildingModel next_model = neighbors.as_model_from_neighbor_count();
                road.change_model_to(next_model);
            }
        }
    }

    void HandleGridInteraction()
    {
        // Cycles between PlacingBuilding, PlacingRoad, MarkingZoneType, and None
        var e = Input.GetKeyDown(KeyCode.E);

        // Cycles paintbrush settings for ZoneType assignment
        bool r = Input.GetKeyDown(KeyCode.R);

        if (e)
        {
            Cell.CycleBuildingMode();
        }

        if (r)
        {
            Cell.CycleZoneType();
        }

        show_text();
    }

    private void show_text()
    {
        if (SimCore.Instance.view_mode != ViewMode.Default)
        {
            text_display.text = "";
            return;
        }
        string focus;
        int num_cells = Cell.all_cells.Count;
        var time = SimCore.Time.now;
        var building = Building.hovering?.legible;
        var cell = Cell.hovering?.location.ToString();
        ZoneType zone_type;
        if (cell is not null)
        {
            focus = cell;
            zone_type = Cell.hovering.zone_type;
        }
        else if (building is not null)
        {
            focus = building;
            zone_type = ZoneType.None;
        }
        else
        {
            focus = "N/A";
            zone_type = ZoneType.None;
        }

        text_display.text =
            $"Time: {time}\n" +
            $"Focus: {focus}\n" +
            $"Zone Type: {zone_type}\n" +
            $"Paintbrush: {Cell.paintbrush}\n" +
            $"Building Placement Mode: {Cell.building_mode}\n" +
            $"# Cells: {num_cells}\n" +
            $"# Buildings: {Building.building_positions.Count}";
    }

    public static Color ZoneColor(ZoneType zoneType)
    {
        switch (zoneType)
        {
            case ZoneType.Residential: return g_residentialZoneColor;
            case ZoneType.Commercial: return g_commercialZoneColor;
            case ZoneType.Industrial: return g_industrialZoneColor;
            case ZoneType.Restricted: return g_restrictedZoneColor;
            default: return g_defaultColor;
        }
    }

    /// Returns the GameObject at the specified grid coordinates
    public GameObject GetCellAt(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        {
            return gridCells[x, z];
        }

        return null;
    }
}
