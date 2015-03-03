using UnityEngine;
using System.Collections;

public class PointControl : MonoBehaviour {
	public int x,y;
	public bool visited = false;
	public EventControl pointEvent;
	public bool isSelected = false;
	public PlayerControl player;
	public Map map;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (visited) {
			gameObject.renderer.material.color = Color.red;
			 
		} 
		else if (Vector2.Distance (map.player.transform.position, gameObject.transform.position )< 3.2){
			gameObject.renderer.material.color = Color.green;
		}
		else {
			this.gameObject.renderer.material.color = Color.white;

		}


	}

	void OnMouseOver(){
		//this.isSelected = true;
		 
		if (Input.GetMouseButtonDown (1)) {
			//Debug.Log (" point is selected ");
			map.isPointSelected =true;

			//Debug.Log ("gaME obj " + gameObject.name);
		}
	
	} 
	void OnTriggerEnter2D( Collider2D other )
	{
		 
		visited = true;
		//map.vistedpoints.Add(gameObject);


	}

}
