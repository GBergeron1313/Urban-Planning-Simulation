using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TogglePanels : MonoBehaviour
{
    [SerializeField] public Button button;
    [SerializeField] public GameObject targetPanel;
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {

        targetPanel.SetActive(false);
        isOpen = false;

        button.onClick.AddListener(TogglePanel);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePanel()
    {
        if (isOpen)
        {
            ClosePanel();
        }
        else
        {
            OpenPanel();
        }
    }

    public void OpenPanel()  
    {
        isOpen = true;
        targetPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        isOpen = false;
        targetPanel.SetActive(false);
    }
}
