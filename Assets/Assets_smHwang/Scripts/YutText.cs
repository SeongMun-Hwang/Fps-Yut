using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class YutText : MonoBehaviour
{ 
    public TextMeshProUGUI text;
    public static int result = 0;
    public static bool isbackdo = false;
    // Use this for initialization
    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (result == 0)
        {
            text.text = "모";
        }
        else if (result == 1)
        {
            if (isbackdo)
            {
                text.text = "백도";
            }
            else
            {
                text.text = "도";
            }
        }
        else if (result == 2)
        {
            text.text = "개";
        }
        else if (result == 3)
        {
            text.text = "걸";
        }
        else if (result == 4)
        {
            text.text = "윷";
        }
    }
}
