using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map : MonoBehaviour {
	public bool isPlayerSelected = false;
	public bool isPointSelected = false;
	//public bool onExit = false;
	public Vector2 pointPos;
	private Vector2 mousePos;

	public PlayerControl player;
	public Transform playerStart, playerEnd;
	public ArrayList vistedpoints;
	// Use this for initialization
	void Start () {
	
	

	}
	
	// Update is called once per frame
	void Update () {
		 
		MouseSelection ();
	}
	void doMovement(){
		// get mouse movement
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
		
		// if a point was clicked
		if(isPointSelected && Vector2.Distance (player.transform.position,pointPos )< 3.2){
			while (Vector2.Distance (player.transform.position,pointPos ) >.03) {
			//Debug.Log (" point : " + player.transform.position);
				player.transform.position = Vector2.MoveTowards (new Vector2 (player.transform.position.x, player.transform.position.y), pointPos,  Time.deltaTime);
			}
		}
		isPointSelected= false;

	}

	void MouseSelection(){

		if (Input.GetMouseButton (0)) {
			//Debug.Log (Input.mousePosition);

		} else if (Input.GetMouseButton (1) ) {
			 
			doMovement();

			//Debug.Log ("do ray"+isPlayerSelected);
		}
	}
}
