using UnityEngine;
using Citizens;
using UnityEngine.AI;

public enum CellType
{
    None,
    Building,
    Road,
}
public class Cell : MonoBehaviour
{
    public static Cell hovering;
    public static Cell last_hovered;
    public static ZoneType paintbrush;
    public static BuildingMode building_mode;
    public static Color default_color;
    public static bool dragging;
    public static CreateBuilding creator;
    public static GridSystem grid;

    private new Renderer renderer;

    public Vector2Int location;
    public ZoneType zone_type;
    public Color color;
    public CellType cell_type;
    public bool walkable;


    public void SetZoneTypeAndUpdate(ZoneType zt)
    {
        zone_type = zt;
        color = GridSystem.ZoneColor(zone_type);
        renderer ??= gameObject.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    public void PushBuilding(Color color)
    {
        if (zone_type == ZoneType.Restricted) return;
        cell_type = CellType.Building;
        this.color = color;
        creator.createBuilding(location.x, location.y, color, zone_type, cell_type);
        var nmm = GetComponent<NavMeshObstacle>();
        Destroy(nmm);
        GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                (int)(gameObject.transform.position.z + 4.5));
        var grid_tile_nmm = grid_tile.GetComponent<NavMeshObstacle>();
        Destroy(grid_tile_nmm);
    }

    public void PushBuilding()
    {
        if (zone_type == ZoneType.Restricted) return;
        cell_type = CellType.Building;
        creator.createBuilding(location.x, location.y, color, zone_type, cell_type);

        var nmm = GetComponent<NavMeshObstacle>();
        Destroy(nmm);
        GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                (int)(gameObject.transform.position.z + 4.5));
        var grid_tile_nmm = grid_tile.GetComponent<NavMeshObstacle>();
        Destroy(grid_tile_nmm);

        var b = GameObject.Find("Building").GetComponent<Building>();
        if (b is null)
        {
            throw new UnityException("Why Was Building NULL?");
        }
        b.TrackPosition(this.transform.position);
    }

    public bool Buildable()
    {
        return zone_type != ZoneType.Restricted;
    }

    public void PushRoad(Color color)
    {
        if (zone_type == ZoneType.Restricted) return;
        cell_type = CellType.Road;
        creator.createBuilding(location.x, location.y, color, zone_type, cell_type);
        var nmm = GetComponent<NavMeshObstacle>();
        Destroy(nmm);
    }

    public void PushRoad()
    {
        if (zone_type == ZoneType.Restricted) return;
        cell_type = CellType.Road;
        creator.createBuilding(location.x, location.y, color, zone_type, cell_type);
        var nmm = GetComponent<NavMeshObstacle>();
        Destroy(nmm);
        GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                (int)(gameObject.transform.position.z + 4.5));
        var grid_tile_nmm = grid_tile.GetComponent<NavMeshObstacle>();
        Destroy(grid_tile_nmm);
    }

    public void SetCellTypeAndUpdate(CellType ct)
    {
        if (cell_type == CellType.Building)
        {
            PushBuilding();
        }
        else if (cell_type == CellType.Road)
        {
            PushRoad();
        }
    }

    public static Cell AtCoords(int x, int z)
    {
        return grid.GetCellAt(x, z).GetComponent<Cell>();
    }

    public override string ToString()
    {
        return JsonUtility.ToJson(this, true);
    }

    private void OnMouseEnter()
    {
        hovering = this;

        if (building_mode == BuildingMode.MarkingZoneType)
        {
            if (dragging)
            {

                // "Buildings", or really, the prefabs that represent them, have
                // a "Cell" component attached. This is why "mousing" over a building
                // gives that hover effect, just like it does for a flat tile.
                //
                // The nasty side-effect of this is that the cell underneath does not 
                // register a state change when the building is "hovered" and marked 
                // with a different zone.
                // This is because the "building"'s Cell component registered the update.
                // The Cell underneath is what gets saved to a file in SaveSystem.
                //
                // To access the cell underneath, we use the grid it's attached to.
                //
                // 4.5 is the offset applied because that will get the grid position from
                // the world position.
                GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                        (int)(gameObject.transform.position.z + 4.5));

                /*throw new UnityException($"gameObject = {grid_tile}");*/
                // ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
                // Try commenting this out. Assigning a ZoneType won't work, but...
                // What you'll notice is that while hovering over 
                // a building, and holding down left click, the message is:
                //
                // gameObject = BuildingScript
                // 
                // When hovering, and clicking, over a flat tile, the message is:
                //
                // Cell_{x}_{z}
                //
                grid_tile.GetComponent<Cell>().SetZoneTypeAndUpdate(paintbrush);
                hovering.SetZoneTypeAndUpdate(paintbrush);
                return;
            }
        }

        Color baseColor = GridSystem.ZoneColor(zone_type);
        Color blendedHoverColor = Color.Lerp(baseColor, GridSystem.g_hoverColor, 0.3f);
        renderer.material.color = blendedHoverColor;
    }

    private void OnMouseExit()
    {
        if (dragging && building_mode == BuildingMode.MarkingZoneType)
        {
            last_hovered.SetZoneTypeAndUpdate(paintbrush);
        }
        hovering = null;
        last_hovered = this;
        renderer.material.color = GridSystem.ZoneColor(zone_type);
    }

    private void OnMouseDrag()
    {
        if (building_mode != BuildingMode.MarkingZoneType) return;
        renderer.material.color = GridSystem.ZoneColor(paintbrush);
        zone_type = paintbrush;
        dragging = true;
    }

    private void OnMouseDown()
    {
        dragging = true;
        if (building_mode == BuildingMode.MarkingZoneType)
        {
            GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                    (int)(gameObject.transform.position.z + 4.5));
            grid_tile.GetComponent<Cell>().SetZoneTypeAndUpdate(paintbrush);
            SetZoneTypeAndUpdate(paintbrush);
        }
        else if (building_mode == BuildingMode.None)
        {
            var nmo = gameObject.AddComponent<NavMeshObstacle>();
            nmo.size = new Vector3(0.25f, 1f, 0.25f);
            nmo.carving = true;
        }
        else
        {
            if (Buildable())
            {
                switch (building_mode)
                {
                    case BuildingMode.PlacingBuilding:
                        grid.fillCell(hovering.location.x, hovering.location.y);
                        color = GridSystem.ZoneColor(zone_type);
                        PushBuilding(color);
                        break;
                    case BuildingMode.PlacingRoad:
                        grid.fillCell(hovering.location.x, hovering.location.y);
                        color = GridSystem.ZoneColor(zone_type);
                        PushRoad(color);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        renderer = GetComponent<Renderer>();
        grid = GameObject.Find("Grid").GetComponent<GridSystem>();
        creator ??= GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}

