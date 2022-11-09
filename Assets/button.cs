using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button : MonoBehaviour
{
    public void buttonOnclick()
    {
        int steps;
        steps = Random.Range(2,4);
        Debug.Log(steps);
    }
}
