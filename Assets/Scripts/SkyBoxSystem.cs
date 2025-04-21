using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyBoxSystem : MonoBehaviour
{
    float rotationSpeed = 1.0f;
    private Material SkyBoxMaterial;

    // Update is called once per frame
    private void Start()
    {
        SkyBoxMaterial = RenderSettings.skybox;
    }

    void Update()
    {
        float currentRotation = SkyBoxMaterial.GetFloat("_Rotation"); // get

        currentRotation += rotationSpeed * Time.deltaTime;

        SkyBoxMaterial.SetFloat("_Rotation", currentRotation); // set
    }
}
