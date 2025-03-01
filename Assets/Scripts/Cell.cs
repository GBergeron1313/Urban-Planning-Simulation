using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public static Cell hovering;
    public static Cell last_hovered;
    public static ZoneType paintbrush;
    public static BuildingMode building_mode;
    public static Color default_color;
    public static bool dragging;


    public GridSystem grid;
    public CreateBuilding creator;
    public Vector2Int location;
    public ZoneType zone_type;
    public Color color;
    private new Renderer renderer;

    private void OnMouseOver()
    {
    }

    private void OnMouseEnter()
    {
        hovering = this;

        if (dragging)
        {
            renderer.material.color = GridSystem.ZoneColor(paintbrush);
            zone_type = paintbrush;
            return;
        }

        Color baseColor = GridSystem.ZoneColor(zone_type);
        Color blendedHoverColor = Color.Lerp(baseColor, GridSystem.g_hoverColor, 0.3f);
        renderer.material.color = blendedHoverColor;
    }

    private void OnMouseExit()
    {
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
        if (building_mode == BuildingMode.MarkingZoneType)
        {
            dragging = true;
            renderer.material.color = GridSystem.ZoneColor(paintbrush);
            zone_type = paintbrush;
        }
        else if (building_mode == BuildingMode.PlacingBuilding)
        {
            creator.createBuilding(
                hovering.location.x,
                hovering.location.y,
                GridSystem.ZoneColor(zone_type),
                zone_type
            );
            grid.fillCell(hovering.location.x, hovering.location.y);
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
        creator = GameObject.Find("CreateBuilding").GetComponent<CreateBuilding>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}