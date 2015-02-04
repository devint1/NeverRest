﻿using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public GameObject current_Block;
	public GameControl game_Control;
	public bool bIsSelected = false;
	public bool destroy_me = false;
	public Block destBlock = null; // Block the cell is moving to
	
	float SPEED = 0.1f;
	float MAX_TURN_DEGREES = 90f;
	int MAX_DISEASE_ABSORBED = 8;

	float turn_Degrees = 0f;
	int diseases_absorbed = 0;
	
	void Start(){
		game_Control.
		StartCoroutine(Change_Turn_Degrees_Cycle());
	}

	public void Select() {
		if(!bIsSelected) {
			game_Control.selected.Add(this);
			gameObject.renderer.material.color = Color.blue;
		}
		bIsSelected = true;
	}

	public void DeSelect() {
		if(bIsSelected) {
			gameObject.renderer.material.color = Color.white;
			//game_Control.selected.Remove (this);
		}
		bIsSelected = false;
	}

	// Clicked on and selected
	void OnMouseDown() {
		if (!bIsSelected) {
			Select();
		} else {
			DeSelect();
		}
	}

	// Running into a disease: Initiate the process of sucking it in
	void OnTriggerEnter2D(Collider2D collidable) {
		if (collidable.gameObject.tag == "Disease") {
			var disease_script = collidable.gameObject.GetComponent<Disease> ();
			if (!disease_script.captured) {
				disease_script.current_Block = this.gameObject;
				disease_script.captured = true;
				disease_script.speed *= 2;

				diseases_absorbed++;
				if (diseases_absorbed >= MAX_DISEASE_ABSORBED) {
					//Destroy (this.gameObject, 2.0f);
					destroy_me = true;
				}
			}
		} else {
			return;
		}

	}

	// Movement Code
	void Update() {
		if (!current_Block.GetComponent<Renderer>().bounds.Contains(this.transform.position)) {
			var direction = current_Block.transform.position - this.transform.position;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		} else {
			Vector3 turn_rotation = new Vector3(0f, 0f, turn_Degrees * Time.deltaTime);
			this.transform.Rotate(turn_rotation);
		}
		this.transform.Translate(this.transform.right * SPEED * Time.deltaTime, Space.World);

		// Check to see if cell has collided with its destination block
		if (destBlock) {
			// Normal collision detection seems to not work for Unity2D
			// Using overlapping bounds detection instead
			if (this.renderer.bounds.Intersects(destBlock.renderer.bounds)) {
				Debug.Log("Entered destination block");
				destBlock.decreaseWBCsTargeting();
				destBlock = null;
			}
		}
	}
	
	IEnumerator Change_Turn_Degrees_Cycle() {
		yield return new WaitForSeconds(1);
		
		turn_Degrees = Random.Range (-MAX_TURN_DEGREES, MAX_TURN_DEGREES);
		
		StartCoroutine(Change_Turn_Degrees_Cycle());
	}

}
