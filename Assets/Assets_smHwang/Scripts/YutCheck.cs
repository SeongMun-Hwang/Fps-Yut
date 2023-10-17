using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutCheck : MonoBehaviour
{
    Vector3[] YutVelocity;
    public static bool[] isAdd;
    private void Awake()
    {
        YutVelocity = new Vector3[4];
        isAdd = new bool[4];
        for(int i = 0; i < 4; i++)
        {
            isAdd[i] = false;
        }
    }
    void FixedUpdate()
    {
        for (int i = 0; i < 4; i++)
        {
            YutVelocity[i] = YutScript.YutVelocity[i];
        }

    }
    void OnTriggerStay(Collider col)
    {
        string parentName = col.gameObject.transform.parent.name;

        //Rigidbody parentRb = col.gameObject.transform.parent.GetComponent<Rigidbody>();
        Rigidbody rb=col.gameObject.transform.parent.GetComponent<Rigidbody>();
        if (rb.velocity.x == 0f && rb.velocity.y == 0f && rb.velocity.z == 0f)
        {
            //Debug.Log("name : "+col.gameObject.name);

            switch (parentName)
            {
                case "yut1":
                    if (col.gameObject.name == "top" && !isAdd[0]) { 
                        YutText.result++; 
                        isAdd[0]=true;
                    }
                    break;
                case "yut2":
                    if (col.gameObject.name == "top" && !isAdd[1]) { YutText.result++;
                        isAdd[1] = true;
                    }
                    break;
                case "yut3":
                    if (col.gameObject.name == "top" && !isAdd[2]) { YutText.result++; 
                        isAdd[2] = true;
                    }
                    break;
                case "yut4":
                    if (col.gameObject.name == "top" && !isAdd[3]) { YutText.result++; YutText.isbackdo = true;
                        isAdd[3] = true;
                    }
                    break;
            }
        }
    }
}
