using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceNumberTextScript : MonoBehaviour {

	TextMeshProUGUI text;
	public static int diceNumber;

	// Use this for initialization
	void Start () {
		text = GetComponent<TextMeshProUGUI> ();
	}
	
	// Update is called once per frame
	void Update () {
        if (diceNumber == 1)
        {
			text.text = "아래";
		}
		else if (diceNumber == 2)
		{
			text.text = "위";
		}
	}
}
