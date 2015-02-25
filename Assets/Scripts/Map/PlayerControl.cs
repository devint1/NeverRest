//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18408
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Collections;

public class PlayerControl: MonoBehaviour{
	private Vector2 pos;
	private bool moving = false;
	public bool playerisSelected = false;
	public bool pointisSelected = false;
	bool mouseDown = false;
	Vector3 mousePositionStart;
	void Start () {
		// First store our current position when the
		// script is initialized.
		pos = transform.position;
	}
	
	void Update () {
		
		CheckInput ();
		
		if (moving) {
			// pos is changed when there's input from the player
			transform.position = pos;
			moving = false;
		}
		if (playerisSelected) {
			this.renderer.material.color = Color.blue;
			Debug.Log ("Player selected2");
		} else {
			this.renderer.material.color = Color.white;
			Debug.Log ("Player deselected");
		}
	}
 
	void OnMouseDown() {
		

		
		this.playerisSelected = true;
		Debug.Log ("Player selected2");
	}
	public void DeSelect() {
		
		gameObject.renderer.material.color = Color.white;
		this.playerisSelected = false;
		Debug.Log ("Player deselected");
	}
	private void CheckInput() {
		
		// WASD control
		// We add the direction to our position,
		// this moves the character 1 unit (32 pixels)
		if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) {
			pos += Vector2.right;
			moving = true;
		}
		
		// For left, we have to subtract the direction
		else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) {
			pos -= Vector2.right;
			moving = true;
		}
		else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) {
			pos += Vector2.up;
			moving = true;
		}
		
		// Same as for the left, subtraction for down
		else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) {
			pos -= Vector2.up;
			moving = true;
		}
	}
}


