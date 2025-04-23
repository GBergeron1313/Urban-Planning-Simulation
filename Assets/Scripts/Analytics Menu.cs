using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsMenu : MonoBehaviour
{
    public Button toggle;
    public TextMeshProUGUI text;
    public GameObject panel;
    public bool on;
    public TextAsset analytics;
    public GameObject stats;

    
    // Start is called before the first frame update
    void Start()
    {
        toggle.onClick.AddListener(ToggleWindow);
        on = false;
        text.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleWindow()
    {
        if (on)
        {
            text.gameObject.SetActive(false);
            panel.SetActive(false);
            on = false;
        }else
        {
            text.gameObject.SetActive(true);
            panel.SetActive(true);
            on = true;
        }
    }
}
