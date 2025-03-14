using UnityEngine;
using Citizens;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.EventSystems;

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
    public static bool dragging;
    public static CreateBuilding creator;
    public static GridSystem grid;
    public static List<Cell> all_cells = new List<Cell>();

    private new Renderer renderer;

    public Vector2Int location;
    public ZoneType zone_type;
    public Color color;
    public CellType cell_type;
    public bool walkable;

    public Building contents;

    public void SetWalkableAndUpdate(bool is_walkable)
    {
        walkable = is_walkable;

        if (walkable)
        {
            var nms = gameObject.AddComponent<NavMeshSurface>();
            nms.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            nms.BuildNavMesh();

            var nmo = GetComponent<NavMeshObstacle>();
            if (nmo)
                nmo.enabled = false;
            var grid_tile_nmo = AtCoords(location.x, location.y)?.GetComponent<NavMeshObstacle>();
            if (grid_tile_nmo)
                grid_tile_nmo.enabled = false;
        }
        else
        {
            NavMeshObstacle nmo;
            if (!gameObject.TryGetComponent<NavMeshObstacle>(out nmo))
            {
                nmo = gameObject.AddComponent<NavMeshObstacle>();
            }
            nmo.enabled = true;
            nmo.carving = true;
            // The "gridCells", in GridSystem, are rotated quads.
            // This makes the X and Y dimensions responsible for
            // width and height. Maybe not in that order, but the
            // important thing to note is that Z, in the local space,
            // refers to height. 
            // This is why nmo.size is:
            // (0.15, 0.15, 0.5) 
            // instead of:
            // (0.15, 0.5, 0.15)

            nmo.size = new Vector3(0.15f, 0.15f, 0.5f);
            // TODO: Change gridCells and the way they work to more 
            // idiomatically represent themselves.
        }
    }

    public static void CycleZoneType()
    {
        Cell.paintbrush++;
        if (Cell.paintbrush > ZoneType.Restricted)
        {
            Cell.paintbrush = ZoneType.Residential;
        }
    }
    public static void CycleBuildingMode()
    {
        Cell.building_mode++;
        if (Cell.building_mode >= BuildingMode.TotalModes)
        {
            Cell.building_mode = BuildingMode.PlacingBuilding;
        }
    }

    public void SetZoneTypeAndUpdate(ZoneType zt)
    {
        zone_type = zt;
        color = GridSystem.ZoneColor(zone_type);
        renderer.material.color = color;
    }

    private bool _TryPushBuilding()
    {
        if (contents != null) return false;

        cell_type = CellType.Building;
        creator.attach_building(this);
        SetWalkableAndUpdate(true);

        return contents != null;
    }

    public bool TryPushBuilding()
    {
        return Buildable() && _TryPushBuilding();
    }

    public bool Removable()
    {
        return cell_type != CellType.None
            && contents != null;
    }

    public bool Buildable()
    {
        return zone_type != ZoneType.Restricted
            && cell_type == CellType.None
            && contents == null;
    }

    private bool _TryPushRoad()
    {
        cell_type = CellType.Road;
        creator.attach_building(this);
        SetWalkableAndUpdate(true);

        return contents != null;
    }

    public bool TryPushRoad(Color color)
    {
        if (!Buildable()) return false;
        this.color = color;
        return _TryPushRoad();
    }

    public bool TryPushRoad()
    {
        return Buildable() && _TryPushRoad();
    }

    public static void RemoveContents(Cell c)
    {
        if (!c.Removable())
        {
            print($"Can't remove at: {c.location}");
            return;
        }
        print($"Removing {c.contents}...");

        c.SetCellTypeAndUpdate(CellType.None);
        bool r = Building.building_positions.Remove(c.gameObject.transform.position)
            || Building.building_positions.Remove(c.transform.position);
        print($"{(r ? "Removed" : "Couldn't Remove")}");
        Destroy(c.contents);
        c.contents = null;
    }

    public void SetCellTypeAndUpdate(CellType ct)
    {
        if (ct == CellType.Building)
        {
            TryPushBuilding();
        }
        else if (ct == CellType.Road)
        {
            TryPushRoad();
        }
        else
        {
            cell_type = ct;
            SetWalkableAndUpdate(false);
        }
    }

    public static Cell AtCoords(Vector2Int loc)
    {
        return all_cells[loc.y + (loc.x * grid.width)];
    }

    public static GameObject ContentsAtCoords(Vector2Int loc)
    {
        return grid.GetCellAt(loc.x, loc.y);
    }

    public static GameObject ContentsAtCoords(int x, int z)
    {
        return grid.GetCellAt(x, z);
    }

    public static Cell AtCoords(int x, int z)
    {
        return grid.GetCellAt(x, z).GetComponent<Cell>();
    }

    public override string ToString()
    {
        int zt = (int)zone_type;
        int ct = (int)cell_type;
        int x = location.x;
        int z = location.y;
        return $"{x},{z}={zt},{ct}";
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            hovering = null;
            last_hovered = this;
            last_hovered.SetZoneTypeAndUpdate(last_hovered.zone_type);
            return;
        }
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
        if (EventSystem.current.IsPointerOverGameObject())
        {
            last_hovered = this;
            last_hovered.SetZoneTypeAndUpdate(last_hovered.zone_type);
            hovering = null;
            return;
        }
        if (dragging && building_mode == BuildingMode.MarkingZoneType)
        {
            last_hovered.SetZoneTypeAndUpdate(paintbrush);
        }
        hovering = null;
        last_hovered = this;
        renderer.material.color = GridSystem.ZoneColor(zone_type);
    }

    private void OnMouseOver()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            last_hovered = this;
            last_hovered.SetZoneTypeAndUpdate(last_hovered.zone_type);
            hovering = null;
        }
        else
        {
            hovering = this;
            Color baseColor = GridSystem.ZoneColor(hovering.zone_type);
            Color blendedHoverColor = Color.Lerp(baseColor, GridSystem.g_hoverColor, 0.3f);
            hovering.renderer.material.color = blendedHoverColor;
        }
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (building_mode != BuildingMode.MarkingZoneType) return;
        renderer.material.color = GridSystem.ZoneColor(paintbrush);
        zone_type = paintbrush;
        dragging = true;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        dragging = true;
        if (building_mode == BuildingMode.MarkingZoneType)
        {
            GameObject grid_tile = grid.GetCellAt((int)(gameObject.transform.position.x + 4.5),
                    (int)(gameObject.transform.position.z + 4.5));
            grid_tile.GetComponent<Cell>().SetZoneTypeAndUpdate(paintbrush);
            SetZoneTypeAndUpdate(paintbrush);
        }
        /*else if (building_mode == BuildingMode.None)*/
        /*{*/
        /*    SetWalkableAndUpdate(false);*/
        /*}*/
        else
        {
            switch (building_mode)
            {
                case BuildingMode.PlacingBuilding:
                    TryPushBuilding();
                    break;

                case BuildingMode.PlacingRoad:
                    TryPushRoad();
                    break;

                case BuildingMode.Removal:
                    RemoveContents(AtCoords(hovering.location));
                    break;

                default:
                    break;
            }
        }
    }

    private void OnMouseUp()
    {
        dragging = false;
    }

    void Awake()
    {
        renderer ??= gameObject.GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        creator ??= GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>();
        all_cells.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

