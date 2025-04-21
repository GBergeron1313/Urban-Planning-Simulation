using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using System;
using BuildingUtils;

public enum CellType
{
    None,
    Building,
    Road,
}

[Serializable]
public class CellSerial
{
    public Vector2Int location;
    public ZoneType zone_type;
    public Color color;
    public CellType cell_type;
    // FIXME: These should be part of an optional BuildingSerial field
    public BuildingModel building_model;
    public Quaternion rotation;
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

    private Renderer m_renderer;

    public Vector2Int location;
    public ZoneType zone_type;
    public Color color;
    public CellType cell_type;
    public bool walkable;

    private static AudioManager am;

    public Building contents;


    public void SetWalkableAndUpdate(bool is_walkable)
    {
        walkable = is_walkable;

        if (walkable)
        {
            var nms = gameObject.AddComponent<NavMeshSurface>();
            Assert.IsNotNull(nms);
            nms.useGeometry = NavMeshCollectGeometry.PhysicsColliders;

            var nmo = GetComponent<NavMeshObstacle>();
            Assert.IsNotNull(nmo);
            nmo.enabled = false;
            nms.BuildNavMesh();
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
        if (building_mode == BuildingMode.PlacingBuilding)
        {
            creator.SetDropdownValues(CellType.Building);
        }
        else if (building_mode == BuildingMode.PlacingRoad)
        {
            creator.SetDropdownValues(CellType.Road);
        }
    }

    public void SetZoneTypeAndUpdate(ZoneType zt)
    {
        zone_type = zt;
        color = GridSystem.ZoneColor(zone_type);
        m_renderer.material.color = color;
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

    private void PushRoad(BuildingModel model)
    {
        cell_type = CellType.Road;
        creator.attach_building(this, model);
        config_anim();

        SetWalkableAndUpdate(true);
    }

    private void config_anim()
    {
        Assert.IsNotNull(contents);

        var animation = contents.gameObject.AddComponent<PlacementAnim>();

        animation.Origin = contents.transform.position + Vector3.up * 2;
        animation.Target = contents.transform.position;
        animation.AnimSizeOrigin = Vector3.zero;
        animation.AnimSizeTarget = Vector3.one;
        animation.AnimStepBy = 0.05f;
        animation.Postponed = (location.x + location.y) * 0.05f;

        PlacementAnim initial_anim;
        if (gameObject.TryGetComponent<PlacementAnim>(out initial_anim))
        {
            animation.Target = initial_anim.Target;
        }

        var rends = contents.GetComponentsInChildren<Renderer>();
        foreach (var rend in rends)
            rend.enabled = false;

        animation.OnAnimStart = () =>
        {
            foreach (var rend in rends)
                rend.enabled = true;
        };

        animation.OnAnimOver = () =>
        {
            am.PlaySFX(am.placedown);
        };

        if (!animation.InitAnim())
        {
            Debug.LogWarning($"Animation for {contents} not Initialized. It might not play.");
        }
    }

    public void FromModelAndUpdate(BuildingModel model)
    {
        CellType ct = model.as_cell_type();
        if (ct == CellType.Building)
        {
            PushBuilding(model);
        }
        else if (ct == CellType.Road)
        {
            PushRoad(model);
        }
        else
        {
            cell_type = ct;
            SetWalkableAndUpdate(false);
        }
    }

    private void PushBuilding(BuildingModel model)
    {
        cell_type = CellType.Building;
        Building.building_positions.Add(transform.position);
        creator.attach_building(this, model);
        SetWalkableAndUpdate(true);
        config_anim();
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
        m_renderer.material.color = blendedHoverColor;
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
        m_renderer.material.color = GridSystem.ZoneColor(zone_type);
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
            hovering.m_renderer.material.color = blendedHoverColor;
        }
    }

    private void OnMouseDrag()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (building_mode != BuildingMode.MarkingZoneType) return;
        m_renderer.material.color = GridSystem.ZoneColor(paintbrush);
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
        else
        {
            switch (building_mode)
            {
                case BuildingMode.PlacingBuilding:
                    FromModelAndUpdate(creator.buildingDropdown.value.as_building_model());
                    break;

                case BuildingMode.PlacingRoad:
                    FromModelAndUpdate(creator.buildingDropdown.value.as_road_model());
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
        m_renderer ??= gameObject.GetComponent<Renderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        creator ??= GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>();
        am ??= GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        Assert.IsNotNull(all_cells, "Cell::Start(): all_cells found null");
    }

    void OnDestroy()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    internal void register()
    {
        Cell.all_cells.Add(this);
    }
}

