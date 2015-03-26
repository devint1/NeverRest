using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class Map : MonoBehaviour {
	public bool isPlayerSelected = false;
	public bool isPointSelected = false;
	//public bool onExit = false;
	public Vector2 pointPos;
	private Vector2 mousePos;
	public PlayerControl player;
	public Transform playerStart, playerEnd;
	public ArrayList vistedpoints;
	public bool finishedEvent = true;
	// Use this for initialization
	void Start () {
	
	

	}
	
	// Update is called once per frame
	void Update () {
		 
		MouseSelection ();
		getPlayerResource ();
	}
	void getPlayerResource (){
	
	}
	void doMovement(){
		// get mouse movement
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition); 
		
		// if a point was clicked and point is moveable
		//if(isPointSelected && Vector2.Distance (player.transform.position,pointPos )< 3.2){
		//allowed to move any point for testing purposes
 		if(isPointSelected ){

			while (Vector2.Distance (player.transform.position,pointPos ) >.3) {
			 	player.transform.position = Vector2.MoveTowards (new Vector2 (player.transform.position.x, player.transform.position.y), pointPos,  Time.deltaTime);
			}
			player.playerFood--;
		}
		isPointSelected= false;

	}

	void MouseSelection(){

		if (Input.GetMouseButton (0)) {
			//Debug.Log (Input.mousePosition);

		} else if (Input.GetMouseButton (1) && finishedEvent ) {
			 
			doMovement();

			//Debug.Log ("do ray"+isPlayerSelected);
		}
	}
	private void CheckGameOver(){
		if (player.playerFood <= 0)
			EventControl.LoseCon();
		else if (player.playerHealth <= 0)
			EventControl.LoseCon();


	}
}
