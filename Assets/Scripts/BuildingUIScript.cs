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

    // Start is called before the first frame update
    void Start()
    {
        pollutionSlider = GameObject.FindGameObjectWithTag("Pollution Slider").GetComponent<Slider>();
        noiseSlider = GameObject.FindGameObjectWithTag("Noise Slider").GetComponent<Slider>();
        capacitySlider = GameObject.FindGameObjectWithTag("Capacity Slider").GetComponent<Slider>();
        pollutionText = GameObject.FindGameObjectWithTag("Pollution Text").GetComponent<TextMeshProUGUI>();
        noiseText = GameObject.FindGameObjectWithTag("Noise Text").GetComponent<TextMeshProUGUI>();
        capacityText = GameObject.FindGameObjectWithTag("Capacity Text").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if(pollutionSlider.IsActive())
        pollutionText.text = " " + (int)pollutionSlider.value;
        if(noiseSlider.IsActive())
        noiseText.text = " " + (int)noiseSlider.value;
        if(capacitySlider.IsActive())
        capacityText.text = " " + (int)capacitySlider.value;
    }
}
