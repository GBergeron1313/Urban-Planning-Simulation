using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClosePanel : MonoBehaviour
{

    [SerializeField] Button button;
    [SerializeField] GameObject panel;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(HidePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HidePanel()
    {
        panel.SetActive(false);
    }
}
