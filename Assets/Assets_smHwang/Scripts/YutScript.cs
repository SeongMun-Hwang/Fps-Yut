using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YutScript : MonoBehaviour {

	public Rigidbody[] rb;
	public static Vector3[] YutVelocity;
	// Use this for initi alization
	void Start () {
		YutVelocity = new Vector3[4];
		//rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		for(int i = 0; i < 4; i++)
        {
			YutVelocity[i] = rb[i].velocity;
		}
		
		if (Input.GetKeyDown (KeyCode.Space)) {
			YutText.result = 0;
			for(int i = 0; i < 4; i++)
            {
				YutCheck.isAdd[i] = false;
            }
			for(int i = 0; i < 4; i++)
            {
				float dirX = Random.Range(0, 500);
				float dirY = Random.Range(0, 500);
				float dirZ = Random.Range(0, 500);
				rb[i].position = new Vector3(8, 2, -8);
				rb[i].rotation = Quaternion.identity;
				rb[i].AddForce(transform.up * 500);
				rb[i].AddTorque(dirX, dirY, dirZ);
			}
		}
	}
}
