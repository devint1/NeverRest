using UnityEngine;
using System.Collections;

public class Disease : MonoBehaviour {
	public GameControl gameControl;
	public Block currentBlock;
	public GameObject diseasePrefab;
	public bool captured = false;
	public float speed = 0.005f;
	public float heartHealthDamagePerSec = 0.01f;
	
	GameObject destination;
	
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
		//If disease has reached is destination
		if ( (destination.transform.position - this.transform.position).magnitude < 0.07) {
			
			//If were captured then we just reached the inside of White Blood Cell. Immobilze ourselves and attach to White Blood Cell
			if(captured) {
				Destroy(gameObject.GetComponent<Rigidbody>());
				Destroy(gameObject.GetComponent<CircleCollider2D>());
				transform.parent = destination.transform;
				Destroy(this);
			}
			else { //Else we just reached a waypoint. Choose next destination.
				if(destination.tag == "ExitPoint") {
					ExitPoint exitPoint = destination.GetComponent<ExitPoint>();
					currentBlock = exitPoint.nextBlock;
					destination = exitPoint.entrancePoint;
				}
				else {
					destination = currentBlock.GetRandomPoint();
				}
			}
		}
		
		Vector3 directionToDestination = (destination.transform.position - this.transform.position).normalized;
		
		this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
		                                       (directionToDestination.y * speed) + this.transform.position.y,
		                                       this.transform.position.z);
	}
	
	// Sends to next block every x seconds
	IEnumerator MoveCycle() {
		yield return new WaitForSeconds(30);
		if (!captured 
		    && currentBlock.exitPoints[0].gameObject.GetComponent<ExitPoint>().nextBlock.diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK 
		    && currentBlock.exitPoints[0].gameObject.GetComponent<ExitPoint>().isExitToHeart) {
			
			currentBlock.diseases.Remove (this);
			currentBlock = currentBlock.exitPoints[0].gameObject.GetComponent<ExitPoint>().nextBlock;
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
			Debug.Log(gameControl.healthLevel);
		}
		StartCoroutine(DamageHeart());
	}
	
	public void BeenCapturedBy(GameObject whiteBloodCell) {
		destination = whiteBloodCell;
		captured = true;
		speed *= 2.5f;
		currentBlock.diseases.Remove(this);
	}
}
