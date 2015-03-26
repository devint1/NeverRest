﻿using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public bool isSelected = false;
	bool isMove = false;
	bool mouseDown = false;
	public Map map; 
	Vector2 userDest;
	public int playerFood = 10;
	public int playerMoney = 50; 
	public int playerHealth = 100;

	// Use this for initialization
	void Start () {
		Debug.Log ( this.transform.position);
	}
	 
	// Update is called once per frame
	void Update () {
	
		if (isSelected) {
			gameObject.renderer.material.color = Color.blue;

		} else {
			gameObject.renderer.material.color = Color.white;
		}
		if ( (Input.GetMouseButton (1))) {
			this.isSelected = false;
			//map.isPlayerSelected = false;

			//Debug.Log("right click");

		}
		else if(Input.GetMouseButtonDown(0)  ){
			//this.isSelected = false;

		}

		
		
	}
	void OnMouseDown(){
		isSelected = true;
		map.isPlayerSelected = true;
		 
		//


	}


}
