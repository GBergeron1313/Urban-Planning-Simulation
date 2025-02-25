using Unity.VisualScripting;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    public Camera mainCam;
    public GameObject mainGrid;
    private GridSystem grid;
    private bool move;
    private bool locked;
    private Material buildingMaterial;
    private Vector3 lastValidPosition;

    void Start()
    {
        
        buildingMaterial = GetComponent<Renderer>().material;
        grid = mainGrid.GetComponent<GridSystem>();
        mainCam = Camera.main;
        lastValidPosition = transform.position;
    }

    void Update()
    {
        // The number of frames that a mouse button is down is less
        // than the number of frames a Raycast is hitting an object.
        //
        // Calculating the Raycast every frame, unconditionally, is inefficient.
        // Only calculating the Raycast if the mouse button/s was/were pressed is better.
        // 
        // The internals of Input.Get... is not actually polling for input from the hardware.
        // Input is polled and stored somewhere. We're just accessing that stored data.
        // Point Being: We're not hardware polling by accident, so this should be better, perf-wise.
        bool mouseLeftPressed = Input.GetMouseButtonDown((int)MouseButton.Left);
        bool mouseRightPressed = Input.GetMouseButtonDown((int)MouseButton.Right);
        bool mouseLeftReleased = Input.GetMouseButtonUp((int)MouseButton.Left);

        if (mouseLeftPressed || mouseRightPressed)
        {
            ray = mainCam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (mouseLeftPressed)
                {
                    if (hit.transform == transform && !locked)
                    {
                        int gridX = Mathf.RoundToInt(transform.position.x) + 5;
                        int gridZ = Mathf.RoundToInt(transform.position.z) + 5;

                        grid.emptyCell(gridX, gridZ);
                        move = true;
                        lastValidPosition = transform.position;
                    }
                }

                if (mouseRightPressed)
                {
                    if (hit.transform == transform)
                    {
                        locked = !locked;
                    }
                }
            }
        }

        if (mouseLeftReleased)
        {
            if (move)
            {
                move = false;
                Vector3 roundedPosition = new Vector3(
                    Mathf.RoundToInt(transform.position.x),
                    transform.position.y,
                    Mathf.RoundToInt(transform.position.z)
                );

                int gridX = Mathf.RoundToInt(roundedPosition.x) + 5;
                int gridZ = Mathf.RoundToInt(roundedPosition.z) + 5;

                if (!grid.isCellFilled(gridX, gridZ))
                {
                    transform.position = roundedPosition;
                    ZoneType zoneType = grid.GetZoneType(gridX, gridZ);

                    switch (zoneType)
                    {
                        case ZoneType.Residential:
                            buildingMaterial.color = new Color(0.2f, 0.8f, 0.2f, 1f);
                            break;
                        case ZoneType.Commercial:
                            buildingMaterial.color = new Color(0.2f, 0.2f, 0.8f, 1f);
                            break;
                        case ZoneType.Industrial:
                            buildingMaterial.color = new Color(0.8f, 0.2f, 0.2f, 1f);
                            break;
                        default:
                            buildingMaterial.color = Color.white;
                            break;
                    }

                    grid.fillCell(gridX, gridZ);
                    locked = true;
                    lastValidPosition = transform.position;
                }
                else
                {
                    if (!locked)
                    {
                        transform.position = lastValidPosition;
                        int lastX = Mathf.RoundToInt(lastValidPosition.x) + 5;
                        int lastZ = Mathf.RoundToInt(lastValidPosition.z) + 5;
                        grid.fillCell(lastX, lastZ);
                    }
                }
            }
        }

        if (move && !locked)
        {
            transform.position += 0.5f * new Vector3(
                Input.GetAxis("Mouse X"),
                0,
                Input.GetAxis("Mouse Y")
            );
        }
    }
}