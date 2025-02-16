using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CreateBuilding: MonoBehaviour
{
    public Camera MainCam;
    public GameObject MainGrid;

    public GameObject buildingPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createBuilding()
    {
        print("Button Clicked"); 
        Instantiate(buildingPrefab);
        buildingPrefab.transform.position = new Vector3(6.0f, 0.5f, 0.0f);
        buildingPrefab.GetComponent<BuildingScript>().mainCam = MainCam;
        buildingPrefab.GetComponent<BuildingScript>().mainGrid = MainGrid;
        
    }
}
