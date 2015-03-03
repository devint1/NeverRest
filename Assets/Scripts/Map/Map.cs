using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Map : MonoBehaviour {
	public bool isPlayerSelected = false;
	public bool isPointSelected = false;
	public PointControl point;
	private Vector3 mousePos;
	public PlayerControl player;
	public Transform playerStart, playerEnd;
	public ArrayList vistedpoints;
	// Use this for initialization
	void Start () {
	
		ArrayList points; 

	}
	
	// Update is called once per frame
	void Update () {
		//Raycasting (); 
		MouseSelection ();
	}
	void Raycasting(){
		//Debug.DrawLine (playerStart.position, playerEnd.position, Color.red);
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		mousePos.z = player.transform.position.z;
		

		if (isPointSelected) {
			while (Vector2.Distance (player.transform.position,mousePos ) >.03) {
				player.transform.position = Vector2.MoveTowards (new Vector2 (player.transform.position.x, player.transform.position.y), mousePos, 3 * Time.deltaTime);
			}
		}
		isPointSelected= false;




		//Debug.Log (" player : " + player.transform.position);
		//Debug.Log (" mouse : " + mousePos);
	}

	void MouseSelection(){

		if (Input.GetMouseButton (0)) {
			//Debug.Log (Input.mousePosition);

		} else if (Input.GetMouseButton (1) && isPlayerSelected) {
			//Raycasting (); 
			Raycasting ();

			//Debug.Log ("do ray"+isPlayerSelected);
		}
	}
}
