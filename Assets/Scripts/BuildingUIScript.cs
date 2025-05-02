using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIScript : MonoBehaviour
{
    public TextMeshProUGUI pollutionText;
    public TextMeshProUGUI noiseText;
    public TextMeshProUGUI capacityText;
    public TextMeshProUGUI roadType;
    TextMeshProUGUI nameText;
    GameObject[] UIElements;
    public GameObject buildingPanel, roadPanel;

    public Button building1, building2, building3, building4, building5, building6, building7, road, delete, next, prev;

    public TMP_Dropdown buildingDropdown;

    // Start is called before the first frame update
    void Start()
    {
        
        pollutionText = GameObject.FindGameObjectWithTag("Pollution Text").GetComponent<TextMeshProUGUI>();
        noiseText = GameObject.FindGameObjectWithTag("Noise Text").GetComponent<TextMeshProUGUI>();
        capacityText = GameObject.FindGameObjectWithTag("Capacity Text").GetComponent<TextMeshProUGUI>();
        nameText = GameObject.FindGameObjectWithTag("Building Name Text").GetComponent<TextMeshProUGUI>();
        roadType = GameObject.FindGameObjectWithTag("Road Type Text").GetComponent<TextMeshProUGUI>();
        UIElements = GameObject.FindGameObjectsWithTag("Building UI");

        pollutionText.gameObject.SetActive(false);
        noiseText.gameObject.SetActive(false);
        capacityText.gameObject.SetActive(false);
        roadType.gameObject.SetActive(false);
        

        building1.onClick.AddListener(() => BuildingButton(0));
        building2.onClick.AddListener(() => BuildingButton(1));
        building3.onClick.AddListener(() => BuildingButton(2));
        building4.onClick.AddListener(() => BuildingButton(3));
        building5.onClick.AddListener(() => BuildingButton(4));
        building6.onClick.AddListener(() => BuildingButton(5));
        building7.onClick.AddListener(() => BuildingButton(6));
        road.onClick.AddListener(() => BuildingButton(7));
        delete.onClick.AddListener(() => BuildingButton(8));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void NextRoad()
    {
        if (buildingDropdown.value != 6)
        {
            buildingDropdown.value ++;
            if (buildingDropdown.value == 5)
                buildingDropdown.value = 6;
        }
        else
        {
            buildingDropdown.value = 1; 
        }
        switch (buildingDropdown.value)
        {
            case 1:
                roadType.text = "Straight Road";
                break;
            case 2:
                roadType.text = "Curve";
                break;
            case 3:
                roadType.text = "Three Way";
                break;
            case 4:
                roadType.text = "Four Way";
                break;
            case 5:
                roadType.text = "Value 5";
                break;
            case 6:
                roadType.text = "Dead End";
                break;
            case 7:
                roadType.text = "Value 7";
                break;
            default:
                roadType.text = "Straight Road";
                break;
        }
    }

    void PrevRoad()
    {        
        if (buildingDropdown.value != 1)
        {
            buildingDropdown.value--;
            if (buildingDropdown.value == 5)
                buildingDropdown.value = 4;
        }
        else
        {
            buildingDropdown .value = 6;
        }
        switch (buildingDropdown.value)
        {
            case 1:
                roadType.text = "Straight Road";
                break;
            case 2:
                roadType.text = "Curve";
                break;
            case 3:
                roadType.text = "Three Way";
                break;
            case 4:
                roadType.text = "Four Way";
                break;
            case 5:
                roadType.text = "Value 5";
                break;
            case 6:
                roadType.text = "Dead End";
                break;
            case 7:
                roadType.text = "Value 7";
                break;
            default:
                roadType.text = "Straight Road";
                break;
        }
    }
    void BuildingButton(int buttonNumber)
    {   
        if (buttonNumber < 7)
        {
            buildingDropdown.value = buttonNumber;
            pollutionText.gameObject.SetActive(true);
            noiseText.gameObject.SetActive(true);
            capacityText.gameObject.SetActive(true);
            roadType.gameObject.SetActive(false);
            prev.gameObject.SetActive(false);
            next.gameObject.SetActive(false);
            Cell.building_mode = BuildingMode.PlacingBuilding;
            nameText.text = "Placing Building Type: " + buttonNumber;
            for (int i = 0; i < UIElements.Length; i++)
            {
                UIElements[i].SetActive(true);
            }

            buildingPanel.gameObject.SetActive(false);
        }
        else
        {
            pollutionText.gameObject.SetActive(false);
            noiseText.gameObject.SetActive(false);
            capacityText.gameObject.SetActive(false); 
            
            
            for (int i = 0; i < UIElements.Length; i++)
            {
                
                UIElements[i].SetActive(false);
            }

            if(buttonNumber == 7)
            {               
                roadType.text = "Straight Road";
                buildingDropdown.value = 1;
                roadType.gameObject.SetActive(true);
                prev.gameObject.SetActive(true);
                next.gameObject.SetActive(true);
                
                Cell.building_mode = BuildingMode.PlacingRoad;
                nameText.text = "Placing Road";

                roadPanel.gameObject.SetActive(false);
            }
            else if(buttonNumber == 8)
            {
                Cell.building_mode = BuildingMode.Removal;
                nameText.text = "Removal";
            }
        }
    }
}
