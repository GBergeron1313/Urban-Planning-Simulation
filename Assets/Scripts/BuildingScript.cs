using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{

    Ray ray;
    RaycastHit hit;

    public Camera mainCam;
    public GameObject mainGrid;

    private bool move;
    private bool locked;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (hit.transform == transform)
                move = true;
            }
            if (Input.GetMouseButtonDown(1))
            {
                if (hit.transform == transform)
                {
                    if (!locked)
                        locked = true;
                    else
                        locked = false;
                }
            }
            
        }
        if (Input.GetMouseButtonUp(0))
            {
            if (move) { 
                    move = false;
                    transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));


                    if (mainGrid.GetComponent<GridSystem>().isCellFilled(Mathf.RoundToInt(transform.position.x) + 5, Mathf.RoundToInt(transform.position.z) + 5) == false)
                    {
                        print("empty grid cell");
                        mainGrid.GetComponent<GridSystem>().fillCell(Mathf.RoundToInt(transform.position.x) + 5, Mathf.RoundToInt(transform.position.z) + 5);
                        locked = true;
                    }
                    else
                    {
                        Destroy(this.gameObject);
                    }
                }
            }
        


        if (move && !locked)
        {
            transform.position += 0.5f * new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        }
    }
}
