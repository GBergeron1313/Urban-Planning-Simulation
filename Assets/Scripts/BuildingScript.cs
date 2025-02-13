using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingScript : MonoBehaviour
{

    Ray ray;
    RaycastHit hit;

    public Camera mainCam;

    private bool move;

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
        }

        if (Input.GetMouseButtonUp(0))
        {
            move = false;
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
        }


        if (move)
        {
            transform.position += 0.5f * new Vector3(Input.GetAxis("Mouse X"), 0, Input.GetAxis("Mouse Y"));
        }
    }
}
