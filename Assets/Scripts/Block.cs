using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BlockType { HEART, OTHER }

public class Block : MonoBehaviour {
	public GameControl gameControl;
	public BlockType blockType;
	public ArrayList diseases = new ArrayList();
	
	public static int MAX_NUM_DISEASE_PER_BLOCK = 200;
	
	private int whiteCellsTargeting = 0; // Number of WhiteBloodCells moving to this block
	GameObject destTarget = null;
	
	List<Transform> points = new List<Transform>();
	public List<Transform> exitPoints = new List<Transform>(); //First one is always exit point that leads to heart
	
	void Start() {
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
		foreach (WhiteBloodCell cell in gameControl.selected) {
			cell.renderer.material.color = Color.white;
			cell.isSelected = false;
			cell.SetDestination (this, Camera.main.ScreenToWorldPoint(Input.mousePosition));
			this.increaseWBCsTargeting();
		}
		gameControl.selected.Clear();
	}
	
	public GameObject GetRandomPoint() {
		if (points.Count == 0){
			PopulatePointsLists ();
		}
		
		int randomPointIndex = Random.Range (0, points.Count-1);
		return points [randomPointIndex].gameObject;
	}
	
	public void increaseWBCsTargeting() {
		if (whiteCellsTargeting == 0) {
			destTarget = (GameObject)Instantiate(this.gameControl.destMarkPrefab,
			                                     GetRandomPoint().transform.position,
			                                     this.transform.rotation);
		}
		whiteCellsTargeting++;
	}
	
	public void decreaseWBCsTargeting() {
		whiteCellsTargeting--;
		if (whiteCellsTargeting == 0) {
			Destroy(destTarget);	
		}
	}
}
