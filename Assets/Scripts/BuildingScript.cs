using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    // Original variables
    Ray ray;
    RaycastHit hit;
    public Camera mainCam;
    public GameObject mainGrid;
    private bool move;
    private bool locked;

    // New variable for zone type
    private ZoneType currentZoneType = ZoneType.None;
    private Material buildingMaterial;

    void Start()
    {
        // Store reference to building material
        buildingMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // Create a ray from the mouse position into the scene
        ray = mainCam.ScreenPointToRay(Input.mousePosition);
        // Check if the ray hits something
        if (Physics.Raycast(ray, out hit))
        {
            // Left mouse button press
            if (Input.GetMouseButtonDown(0))
            {
                // If we hit this building, enable movement
                if (hit.transform == transform) {
                    GridSystem grid = mainGrid.GetComponent<GridSystem>();
                    int gridX = Mathf.RoundToInt(transform.position.x) + 5;
                    int gridZ = Mathf.RoundToInt(transform.position.z) + 5;

                    // Check if the grid cell is available
                    if (grid.isCellFilled(gridX, gridZ))
                    {
                        grid.emptyCell(gridX, gridZ);
                    }
                    move = true;
                }
            }
            // Right mouse button press
            if (Input.GetMouseButtonDown(1))
            {
                // If we hit this building, toggle locked state
                if (hit.transform == transform)
                {
                    if (!locked)
                        locked = true;
                    else
                        locked = false;
                }
            }
        }
        // Left mouse button release
        if (Input.GetMouseButtonUp(0))
        {
            if (move)
            {
                // Stop movement
                move = false;
                // Round position to nearest grid cell
                transform.position = new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    transform.position.y,
                    Mathf.RoundToInt(transform.position.z)
                );

                GridSystem grid = mainGrid.GetComponent<GridSystem>();
                int gridX = Mathf.RoundToInt(transform.position.x) + 5;
                int gridZ = Mathf.RoundToInt(transform.position.z) + 5;

                // Check if the grid cell is available
                if (!grid.isCellFilled(gridX, gridZ))
                {
                    print("empty grid cell");
                    // Get the zone type at this position
                    GameObject cell = grid.GetCellAt(gridX, gridZ);
                    if (cell != null)
                    {
                        // Copy zone color to building
                        Color zoneColor = cell.GetComponent<Renderer>().material.color;
                        buildingMaterial.color = new Color(zoneColor.r, zoneColor.g, zoneColor.b, 1f);
                    }

                    // Mark the cell as filled
                    grid.fillCell(gridX, gridZ);
                    // Lock the building in place
                    locked = true;
                }
                else
                {
                    // If cell is occupied, destroy this building
                    if(!locked)
                    Destroy(this.gameObject);
                }
            }
        }
        // If building is being moved and not locked
        if (move && !locked)
        {
            // Update position based on mouse movement
            transform.position += 0.5f * new Vector3(
                Input.GetAxis("Mouse X"),
                0,
                Input.GetAxis("Mouse Y")
            );
        }
    }
}
