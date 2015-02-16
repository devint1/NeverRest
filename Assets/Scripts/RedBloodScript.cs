﻿using UnityEngine;
using System.Collections;

public class RedBloodScript : MonoBehaviour {
	public Block currentBlock;		//block cell is currently in
	public Block origBlock;			//block the red blood cell originates from, aka target block
	public Block heartBlock;		//pointer to the heart
	public GameControl gameControl;
	public bool returnToHeart;		//set to true if on path back to heart, false otherwise
	public GameObject headingToward; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now
	public bool oxygenated = true;	//set to false upon arrival at destination (room)
	
	const float SPEED = 0.0075f;
	public Block destBlock = null; // Block the cell is moving to
	bool destChanged = false;
	Vector2 userDest;
	bool hasUserDest; //Need to use this since userDest cannot = null
	ExitPoint pathingToEntrance = null;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( gameControl.CheckIfPaused() ){
			return;
		}
		
		if (!gameControl.toggleRBC) {
			Destroy(this.gameObject);
			Destroy(this);
		}
		
		if(!oxygenated)
			this.renderer.material.SetColor("_Color", new Color(172.0f / 255.0f,0.0f,0.0f));
		else
			this.renderer.material.SetColor("_Color", new Color(255.0f / 255.0f,0.0f,0.0f));
		
		//If we are at current way point or the destination has been changed
		if (!headingToward || Vector2.Distance (headingToward.transform.position, this.transform.position) < .03 || destChanged) {
			//If we have arrived at our exit node, our next node should be the next cells entrance node
			//Dest change is to check if it was a destchange request or we reach current node
			if( headingToward && headingToward.tag == "ExitPoint" && !destChanged ){
				pathingToEntrance = headingToward.GetComponent<ExitPoint>();
				headingToward = headingToward.GetComponent<ExitPoint>().entrancePoint;
			}
			//Otherwise if we are not in the correct block we need to find the next exit
			else if (destBlock && destBlock != currentBlock) {
				if (pathingToEntrance){
					currentBlock = pathingToEntrance.GetComponent<ExitPoint>().nextBlock;
					pathingToEntrance = null;
					headingToward = null;
				}
				else{
					foreach (Transform exitPoint in currentBlock.exitPoints) {
						if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, currentBlock) ) {
							headingToward = exitPoint.gameObject;
							break;
						}
					}	
				}
			}
			//If we are in the correct block we need to go to users click
			else if (hasUserDest){
				headingToward = new GameObject();
				headingToward.transform.position = userDest;
				hasUserDest = false;
			}
			//Last option is going to a random waypoint
			else{
				//headingToward = currentBlock.GetRandomPoint();
				if(origBlock != heartBlock) {
					if(!returnToHeart) {
						returnToHeart = true;
						oxygenated = false;		//set the color to dark red
						destBlock = heartBlock;
						destChanged = true;
					}
					else {
						returnToHeart = false;
						oxygenated = true;
						destBlock = origBlock;
						destChanged = true;
					}
				}
				else {
					headingToward = currentBlock.GetRandomPoint();
				}
			}
			if( destChanged ){
				destChanged = false;
			}
		}
		if (headingToward) {
			Vector2 directionToDestination = ((Vector2)headingToward.transform.position - (Vector2)this.transform.position).normalized;			
			this.transform.position = new Vector3 ((directionToDestination.x * SPEED) + this.transform.position.x,
			                                       (directionToDestination.y * SPEED) + this.transform.position.y,
			                                       this.transform.position.z);
		}
	}
	
	bool ExitPointLeadsToDestination(GameObject exit, Block destination, Block curBlock) {
		if(exit.gameObject.GetComponent<ExitPoint>().nextBlock == destination) {
			return true;
		}
		
		//TODO - We should cache lookups
		foreach(Transform exitPoint in exit.GetComponent<ExitPoint>().nextBlock.exitPoints) {
			if( curBlock != exitPoint.gameObject.GetComponent<ExitPoint>().nextBlock ) {
				if( ExitPointLeadsToDestination(exitPoint.gameObject, destination, exit.GetComponent<ExitPoint>().nextBlock) ) {
					return true;
				}
			}
		}
		
		return false;
	}
}