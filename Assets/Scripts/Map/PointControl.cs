
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PointControl : MonoBehaviour {

	public PlayerControl playerControl;
	public BlockType blockType;
	public GameObject destMarkPrefab;
	
	private Vector3 mousePos;
	private Tesselator tesselator;

	
	void Start() {
		var collider = this.GetComponent<PolygonCollider2D> ();
		if (collider) {
			tesselator = new Tesselator (collider.points);
		}
	}
	void Update(){

	}

	void OnMouseDown() {
		
		
		gameObject.renderer.material.color = Color.blue;
		Debug.Log ("Point selected");
	}
	public void DeSelect() {

		gameObject.renderer.material.color = Color.white;

	}
}