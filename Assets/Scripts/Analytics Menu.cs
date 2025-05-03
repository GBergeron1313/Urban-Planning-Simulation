using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AnalyticsMenu : MonoBehaviour
{
    public Button toggle;
    public TextMeshProUGUI analyticsText;
    public GameObject panel;
    public bool showing;
    public GameObject stats;


    // Start is called before the first frame update
    void Start()
    {
        toggle.onClick.AddListener(ToggleWindow);
        showing = false;
        analyticsText.gameObject.SetActive(false);
        panel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void ToggleWindow()
    {
        analyticsText.gameObject.SetActive(showing);
        panel.SetActive(showing);
        showing = !showing;
    }
}
