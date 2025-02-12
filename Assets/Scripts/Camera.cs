using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public float cameraSpeed = 0.05f;
    public float scrollSpeed = 2f;

    public float rotationSpeed = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;

    private bool rotate = false;

    public float boundrySize = 0;
    public float ceilingHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            if(transform.position.x <= boundrySize && transform.position.x >= -boundrySize && transform.position.z <= boundrySize && transform.position.z >= -boundrySize)
            transform.position += cameraSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

            if (transform.position.z > boundrySize)
                transform.position = new Vector3(transform.position.x, transform.position.y, boundrySize);
            if (transform.position.z < -boundrySize)
                transform.position = new Vector3(transform.position.x, transform.position.y, - boundrySize);
            if (transform.position.x > boundrySize)
                transform.position = new Vector3(boundrySize, transform.position.y, transform.position.z);
            if (transform.position.x < -boundrySize)
                transform.position = new Vector3(-boundrySize, transform.position.y, transform.position.z);

        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            if (transform.position.y <= ceilingHeight && transform.position.y >= 0)
            transform.position += scrollSpeed * new Vector3(0, Input.GetAxis("Mouse ScrollWheel"), 0);

            if (transform.position.y > ceilingHeight)
                transform.position = new Vector3(transform.position.x, ceilingHeight, transform.position.z);
            if (transform.position.y < 0)
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
            yaw += rotationSpeed * Input.GetAxis("Mouse X");
            pitch -= rotationSpeed * Input.GetAxis("Mouse Y");

            transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            transform.position = new Vector3(0, 1, -10);
            transform.eulerAngles = new Vector3(0.0f, 0.0f, 0.0f);
        }
    }
}
