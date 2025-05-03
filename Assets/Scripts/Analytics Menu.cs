using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsMenu : MonoBehaviour
{
    public Button toggle;
    public TextMeshProUGUI analyticsText;
    public GameObject panel;
    public bool on;
    public TextAsset analytics;
    public GameObject stats;

    
    // Start is called before the first frame update
    void Start()
    {
        toggle.onClick.AddListener(ToggleWindow);
        on = false;
        analyticsText.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (analyticsText.gameObject.activeSelf)
        {

            analyticsText.text = analytics.text;
        }
    }

    void ToggleWindow()
    {
        if (on)
        {
            analyticsText.gameObject.SetActive(false);
            panel.SetActive(false);
            on = false;
        }else
        {
            analyticsText.gameObject.SetActive(true);
            panel.SetActive(true);
            on = true;
        }
    }
}
