using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private float moveSpeed = 0.05f;
    private float scrollSpeed = 2f;

    public float speedH = 2.0f;
    public float speedV = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private bool rotate = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            transform.position += moveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            transform.position += scrollSpeed * new Vector3(0, Input.GetAxis("Mouse ScrollWheel"), 0);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (rotate)
            {
                rotate = false;
            }
            else
            {
                rotate = true;
            }
        }
        if (rotate)
        {
            yaw += speedH * Input.GetAxis("Mouse X");
            pitch -= speedV * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector3(0, 1, -10);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
