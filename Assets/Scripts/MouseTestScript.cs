using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MouseTestScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y - 75, this.transform.position.z);
    }

    public void SetText(string Text)
    {
        text.text = Text;
    }
}
