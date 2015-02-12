using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BlockType { HEART, OTHER }

public class Block : MonoBehaviour {
	public GameControl gameControl;
	public BlockType blockType;
	public ArrayList diseases = new ArrayList();
	public GameObject destMarkPrefab;

	private Vector3 mousePos;
	private Tesselator tesselator;

	public static int MAX_NUM_DISEASE_PER_BLOCK = 200;
	
	List<Transform> points = new List<Transform>();
	public List<Transform> exitPoints = new List<Transform>(); //First one is always exit point that leads to heart
	
	void Start() {
		var collider = this.GetComponent<PolygonCollider2D> ();
		if (collider) {
			tesselator = new Tesselator (collider.points);
			tesselator.DrawTriangles(this.transform.gameObject);
		}
		if (points.Count == 0)
			PopulatePointsLists ();
	}
	
	void PopulatePointsLists() {
		foreach (Transform child in transform) //Iterate through all children
		{
			if(child.tag == "Point") {
				points.Add (child);
			}
			else if(child.tag == "ExitPoint") {
				ExitPoint exitPoint = child.GetComponent<ExitPoint>();
				
				if(exitPoint.isExitToHeart) {
					exitPoints.Insert(0, child);
				}
				else {
					exitPoints.Add (child);
				}
			}
		}
	}

	// Block clicked. Send selected WhiteBloodCell here
	void OnMouseOver() {
		//Quit out if not a right click
		if (!Input.GetMouseButtonDown(1)){
			return;
		}
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		foreach (WhiteBloodCell cell in gameControl.selected) {
			cell.renderer.material.color = Color.white;
			cell.isSelected = false;
			cell.SetDestination (this, mousePos);
		}
		StartCoroutine(FireMouseClick());
		gameControl.selected.Clear();
	}
	
	public GameObject GetRandomPoint() {
		// Needs some work
		/*
		Vector2[] triangle = tesselator.GetRandomTriangle ();
		float randomWeight1 = Random.value;
		float randomWeight2 = Random.value;
		float randomWeight3 = Random.value;
		triangle [0] *= randomWeight1;
		triangle [1] *= randomWeight2;
		triangle [2] *= randomWeight3;

		GameObject point = new GameObject ();
		point.transform.position = new Vector3((triangle [0].x + triangle [1].x + triangle [2].x) / 3,
			(triangle [0].y + triangle [1].y + triangle [2].y / 3),
			transform.position.z);

		return point;
		*/

		if (points.Count == 0){
			PopulatePointsLists ();
		}
		
		int randomPointIndex = Random.Range (0, points.Count-1);
		return points [randomPointIndex].gameObject;
	}

	IEnumerator FireMouseClick()
	{
		if (!destMarkPrefab.activeSelf) {
			destMarkPrefab.SetActive(true);
		}
		GameObject mouseTarget = (GameObject)Instantiate (destMarkPrefab, (Vector2)mousePos, Quaternion.identity);
		Color c = Color.green;
		while (c.a > 0){
			yield return new WaitForSeconds(.1f);
			c.a -= .05f;
			mouseTarget.renderer.material.color = c;
		}
		Destroy (mouseTarget);
	}
}
