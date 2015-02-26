using UnityEngine;
using System.Collections;

public class PointControl : MonoBehaviour {
	public int x,y;
	public bool visted = false;
	public EventControl pointEvent;
	public bool isSelected = false;
	public PlayerControl player;
	public Map map;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (isSelected) {
			gameObject.renderer.material.color = Color.blue;
			 
		} else {
			this.gameObject.renderer.material.color = Color.white;
		}

	}

	void OnMouseOver(){
		//this.isSelected = true;
		 
		if (Input.GetMouseButtonDown (1)) {
			Debug.Log (" point is selected ");
		}



		
	}
}
