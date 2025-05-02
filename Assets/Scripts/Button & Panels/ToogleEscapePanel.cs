using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooglePanel : MonoBehaviour
{
    private GameObject currentPanel;
    public bool isOpen;

    // Start is called before the first frame update
    void Start()
    {
        currentPanel = GameObject.Find("EscapePanel");
        currentPanel.SetActive(false);
        isOpen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePanel();
        }
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
        currentPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        isOpen = false;
        currentPanel.SetActive(false);
    }
}
