using UnityEngine;
using System.Collections;

public class PlayerControl : MonoBehaviour {
	public bool isSelected = false;
	bool isMove = false;
	bool mouseDown = false;
	public Map map; 
	Vector2 userDest;

	// Use this for initialization
	void Start () {
	
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
			map.isPlayerSelected = false;

			//Debug.Log("right click");

		}
		else if(Input.GetMouseButtonDown(0)  ){
			//this.isSelected = false;

		}

		
		
	}
	void OnMouseDown(){
		isSelected = true;
		map.isPlayerSelected = true;
		//Debug.Log (map.isPlayerSelected);
		//Debug.Log (" PLayer is selected ");


	}


}
