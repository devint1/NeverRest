using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Body : MonoBehaviour {
	public GameControl gameControl;

	List<GameObject> blocks = new List<GameObject> ();

	void Start() {
		//Find all blocks
		foreach (Transform child in transform) //Iterate through all children
		{
			blocks.Add(child.gameObject);
		}
	}

	// Block clicked. Find which one, then send selected WhiteBloodCell here
	void OnMouseDown() {
		Debug.Log ("MouseDown!");
	}
}
