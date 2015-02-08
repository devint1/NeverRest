using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public Block currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public Block destBlock = null; // Block the cell is moving to
	public AudioClip spawnSound =  null;
	
	const float SPEED = 0.075f;
	const int MAX_DISEASE_ABSORBED = 8;
	
	int diseasesabsorbed = 0;
	public GameObject headingToward; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now

	public void Start(){
		AudioSource temp = gameObject.AddComponent<AudioSource> ();
		temp.clip = spawnSound;
		temp.Play ();
	}

	public void Select(){
		if(!isSelected){
			gameControl.selected.Add (this);
			gameObject.renderer.material.color = Color.blue;
		}
		isSelected = true;
	}
	
	public void DeSelect() {
		if(isSelected) {
			gameObject.renderer.material.color = Color.white;
			//game_Control.selected.Remove (this);
		}
		isSelected = false;
	}
	
	// Clicked on and selected
	void OnMouseDown() {
		if (!isSelected) {
			Select();
		} else {
			DeSelect();
		}
	}

	void CheckCollisionOnDisease(){
		ArrayList capture = new ArrayList ();

		foreach (Disease disease in currentBlock.diseases){
			if (Vector3.Distance (disease.transform.position, transform.position) < .2){
				if (!disease.captured) {
					capture.Add (disease);
					diseasesabsorbed++;
					--gameControl.numDiseaseCells;
					if (diseasesabsorbed >= MAX_DISEASE_ABSORBED) {
						destroyMe = true;
					}
				}
			}
		}
		foreach (Disease disease in capture) {
			disease.BeenCapturedBy (this.gameObject);
		}
	}

	// Movement Code
	void Update () {
		CheckCollisionOnDisease ();

		//If we are not in our desination block and not on the way to ExitPoint then get to proper exit point
		if (destBlock && destBlock != currentBlock && headingToward.tag != "ExitPoint") {
			foreach(Transform exitPoint in currentBlock.exitPoints) {
				if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, currentBlock) ) {
					headingToward = exitPoint.gameObject;
					break;
				}
			}
		}
		
		//If cell has reached is destination
		if ( (headingToward.transform.position - this.transform.position).magnitude < 0.07) {
			
			//We just reached a waypoint. Choose next destination.
			if(headingToward.tag == "ExitPoint") {
				ExitPoint exitPoint = headingToward.GetComponent<ExitPoint>();
				currentBlock = exitPoint.nextBlock;
				headingToward = exitPoint.entrancePoint;
			}
			else {
				headingToward = currentBlock.GetRandomPoint();
			}
		}
		
		Vector3 directionToDestination = (headingToward.transform.position - this.transform.position).normalized;
		
		this.transform.position = new Vector3 ((directionToDestination.x * SPEED) + this.transform.position.x,
		                                       (directionToDestination.y * SPEED) + this.transform.position.y,
		                                       this.transform.position.z);
		
		// Check to see if cell is at destination block
		if (destBlock) {
			if (destBlock == currentBlock) {
				destBlock.decreaseWBCsTargeting();
				destBlock = null;
			}
		}
	}
	
	GameObject FindExitPointToDestination(Block current, Block destination) {
		foreach (Transform exitPoint in currentBlock.exitPoints) {
			if(exitPoint.gameObject.GetComponent<ExitPoint>().nextBlock == destination) {
				return exitPoint.gameObject;
			}
			else {
				GameObject exitIntoDestination = FindExitPointToDestination(exitPoint.gameObject.GetComponent<ExitPoint>().nextBlock, destination);
				
				if(exitIntoDestination) {
					return exitPoint.gameObject;
				}
			}
		}
		
		return null;
	}
	
	bool ExitPointLeadsToDestination(GameObject exit, Block destination, Block curBlock) {
		if(exit.gameObject.GetComponent<ExitPoint>().nextBlock == destination) {
			return true;
		}
		
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
