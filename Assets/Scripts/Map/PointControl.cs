using UnityEngine;
using System.Collections;

public class PointControl : MonoBehaviour {
	public int x,y;
	public bool visited = false;
	public bool isSelected = false;
	public Map map;
	public bool isExit = false;
	public bool isShop = false;
	public bool isPlayerOn = false;
	public EventControl events; 
	private LineRenderer line; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (visited) {
			gameObject.renderer.material.color = Color.red;
			 
		} 
		else if (Vector2.Distance (map.player.transform.position, gameObject.transform.position )< 3.2 && !isPlayerOn){
			gameObject.renderer.material.color = Color.green;



		}
		else {
			this.gameObject.renderer.material.color = Color.white;

		}


	}

	void OnMouseOver(){
		//this.isSelected = true;
		 
		if (Input.GetMouseButtonDown (1)) {
			//move player
			map.pointPos = this.transform.position;
			map.isPointSelected = true;
		}	
	} 
	void OnTriggerEnter2D( Collider2D other )
	{
		if (other.gameObject.tag == "Player") {
			visited = true;
			isPlayerOn = true;
			if (isShop){
				events.pointPos = gameObject.transform.position;
				events.dialogOpen = EventControl.PointType.POINT_TYPE_SHOP;
			}
			else if (isExit){
				EventControl.WinCon();
			}


		}


	}
	void OnTriggerExit2D( Collider2D other )
	{

		isPlayerOn = false;

		
		
	}

}
