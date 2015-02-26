using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {
	public bool isPlayerSelected = false;
	public bool isPointSelected = false;
	public PointControl point;
	private Vector3 mousePos;
	public PlayerControl player;
	public Transform playerStart, playerEnd;
	// Use this for initialization
	void Start () {
	
		ArrayList points;
		ArrayList vistedpoints;

	}
	
	// Update is called once per frame
	void Update () {
		//Raycasting (); 
		MouseSelection ();
	}
	void Raycasting(){
		//Debug.DrawLine (playerStart.position, playerEnd.position, Color.red);
		isPointSelected = Physics2D.Linecast (Input.mousePosition, Input.mousePosition, 1 << LayerMask.NameToLayer ("point"));
		Debug.Log (isPointSelected+ " ray result");
		if (isPointSelected) {
			Debug.Log("Moving to point");
		}

	}

	void MouseSelection(){
		if (Input.GetMouseButton (0)) {
			//Debug.Log (Input.mousePosition);
			Debug.Log ("gaME obj" + gameObject.tag +" "+gameObject.name);
		} else if (Input.GetMouseButton (1) && isPlayerSelected) {
			Raycasting (); 
			//Debug.Log ("do ray"+isPlayerSelected);
		}
	}
}
