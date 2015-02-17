﻿using UnityEngine;
using System.Collections;

public class Disease : MonoBehaviour {
	public GameControl gameControl;
	public Block currentBlock;
	public GameObject diseasePrefab;
	public bool captured = false;
	public float speed = 0.005f;
	public float heartHealthDamagePerSec = 0.01f;
	
	Vector3 destination;
	
	const int MAX_DISEASE_RESPAWN_TIME = 10;
	const int MIN_DISEASE_RESPAWN_TIME = 20;
	
	void Start() {
		if (currentBlock) {
			destination = currentBlock.GetRandomPoint ();
		}
		
		StartCoroutine(MoveCycle());
		StartCoroutine(DuplicateCycle());
		StartCoroutine(DamageHeart());
		
		if(currentBlock != null)
			currentBlock.diseases.Add (this);
	}
	
	// Movement Code
	void Update () {
		if( gameControl.CheckIfPaused() ){
			return;
		}
		//If disease has reached is destination
		if ( destination != null && (destination - this.transform.position).magnitude < 0.07) {
			//If were captured then we just reached the inside of White Blood Cell. Immobilze ourselves and attach to White Blood Cell
			if(captured) {
				Destroy(gameObject.GetComponent<Rigidbody>());
				Destroy(gameObject.GetComponent<CircleCollider2D>());
				transform.position = destination;
				// Destroy(this);
			}
			else {
				// Else we just reached a waypoint. Choose next destination.
				// Roll a random dice with 3% chance of exiting
				float dice = Random.value;
				bool exit = dice >= 0.97f;
				if(exit) {
					ExitPoint exitPoint = currentBlock.GetExitPoint();
					currentBlock = exitPoint.nextBlock;
					destination = currentBlock.GetRandomPoint();
				}
				else {
					destination = currentBlock.GetRandomPoint();
				}
			}
		}

		if (destination != null && !captured) {
			Vector2 directionToDestination = ((Vector2)destination - (Vector2)this.transform.position).normalized;

			this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
                               (directionToDestination.y * speed) + this.transform.position.y,
                               this.transform.position.z);
		}
	}
	
	// Sends to next block every x seconds
	IEnumerator MoveCycle() {
		yield return new WaitForSeconds(30);
		if (!captured 
		    && currentBlock.GetExitPoint().nextBlock.diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK 
		    && currentBlock.GetExitPoint().isExitToHeart) {
			
			currentBlock.diseases.Remove (this);
			currentBlock = currentBlock.GetExitPoint().nextBlock;
			currentBlock.diseases.Add (this);
			StartCoroutine (MoveCycle ());
		} else if (!captured) {
			StartCoroutine (MoveCycle ());
		}
	}
	
	// Creates new disease every x seconds
	IEnumerator DuplicateCycle() {
		while (!captured) {
			int waitFor = Random.Range (MIN_DISEASE_RESPAWN_TIME, MAX_DISEASE_RESPAWN_TIME);
			yield return new WaitForSeconds(waitFor);
			
			if (!captured && currentBlock.diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK) {
				GameObject newDisease = (GameObject)Instantiate (diseasePrefab, this.transform.position, this.transform.rotation);
				Disease newDiseaseScript = newDisease.GetComponent<Disease>();
				newDiseaseScript.currentBlock = currentBlock;
				newDiseaseScript.gameControl = gameControl;
				newDiseaseScript.destination = destination;
				++gameControl.numDiseaseCells;
			}
		}
		
	}
	
	IEnumerator DamageHeart() {
		yield return new WaitForSeconds(1);
		
		if (!captured && currentBlock.blockType == BlockType.HEART) {
			gameControl.healthLevel -= heartHealthDamagePerSec;
		}
		StartCoroutine(DamageHeart());
	}
	
	public void BeenCapturedBy(GameObject whiteBloodCell) {
		destination = whiteBloodCell.transform.position;
		captured = true;
		speed *= 2.5f;
		currentBlock.diseases.Remove(this);
	}
}
