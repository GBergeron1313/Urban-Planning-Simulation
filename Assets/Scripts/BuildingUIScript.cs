using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingUIScript : MonoBehaviour
{
    public Slider pollutionSlider;
    public Slider noiseSlider;
    public Slider capacitySlider;

    public TextMeshProUGUI pollutionText;
    public TextMeshProUGUI noiseText;
    public TextMeshProUGUI capacityText;
    TextMeshProUGUI nameText;
    GameObject[] UIElements;

    public Button building1, building2, building3, building4, building5, building6, building7, road, delete;

    public TMP_Dropdown buildingDropdown;

    // Start is called before the first frame update
    void Start()
    {
        pollutionSlider = GameObject.FindGameObjectWithTag("Pollution Slider").GetComponent<Slider>();
        noiseSlider = GameObject.FindGameObjectWithTag("Noise Slider").GetComponent<Slider>();
        capacitySlider = GameObject.FindGameObjectWithTag("Capacity Slider").GetComponent<Slider>();
        pollutionText = GameObject.FindGameObjectWithTag("Pollution Text").GetComponent<TextMeshProUGUI>();
        noiseText = GameObject.FindGameObjectWithTag("Noise Text").GetComponent<TextMeshProUGUI>();
        capacityText = GameObject.FindGameObjectWithTag("Capacity Text").GetComponent<TextMeshProUGUI>();
        nameText = GameObject.FindGameObjectWithTag("Building Name Text").GetComponent<TextMeshProUGUI>();
        UIElements = GameObject.FindGameObjectsWithTag("Building UI");

        for (int i = 0; i < UIElements.Length; i++)
        {
            UIElements[i].SetActive(false);
        }

        pollutionSlider.gameObject.SetActive(false);
        noiseSlider.gameObject.SetActive(false);
        capacitySlider.gameObject.SetActive(false);
        pollutionText.gameObject.SetActive(false);
        noiseText.gameObject.SetActive(false);
        capacityText.gameObject.SetActive(false);

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
        if (pollutionSlider.IsActive())
            pollutionText.text = " " + (int)pollutionSlider.value;
        if (noiseSlider.IsActive())
            noiseText.text = " " + (int)noiseSlider.value;
        if (capacitySlider.IsActive())
            capacityText.text = " " + (int)capacitySlider.value;
    }

    void BuildingButton(int buttonNumber)
    {
        

        buildingDropdown.value = buttonNumber;

        if (buttonNumber < 7)
        {
            pollutionText.gameObject.SetActive(true);
            noiseText.gameObject.SetActive(true);
            capacityText.gameObject.SetActive(true);
            pollutionSlider.gameObject.SetActive(true);
            noiseSlider.gameObject.SetActive(true);
            capacitySlider.gameObject.SetActive(true);
            Cell.building_mode = BuildingMode.PlacingBuilding;
            nameText.text = "Placing Building Type: " + buttonNumber;
            for (int i = 0; i < UIElements.Length; i++)
            {
                UIElements[i].SetActive(true);
            }
        }
        else
        {
            pollutionText.gameObject.SetActive(false);
            noiseText.gameObject.SetActive(false);
            capacityText.gameObject.SetActive(false);
            pollutionSlider.gameObject.SetActive(false);
            noiseSlider.gameObject.SetActive(false);
            capacitySlider.gameObject.SetActive(false);            
            
            for (int i = 0; i < UIElements.Length; i++)
            {
                UIElements[i].SetActive(false);
            }

            if(buttonNumber == 7)
            {
                Cell.building_mode = BuildingMode.PlacingRoad;
                nameText.text = "Placing Road";
            }
            else if(buttonNumber == 8)
            {
                Cell.building_mode = BuildingMode.Removal;
                nameText.text = "Removal";
            }
        }


    }
}
