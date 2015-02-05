using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public Block currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public Block destBlock = null; // Block the cell is moving to
	public AudioClip spawnSound =  null;
	
	const float SPEED = 0.005f;
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
	
	// Running into a disease: Initiate the process of sucking it in
	void OnTriggerEnter2D(Collider2D collidable) {
		if (collidable.gameObject.tag != "Disease")
			return;
		
		var diseaseScript = collidable.gameObject.GetComponent<Disease>();
		if (!diseaseScript.captured) {
			diseaseScript.BeenCapturedBy(this.gameObject);
			diseasesabsorbed++;
			--gameControl.numDiseaseCells;
			if (diseasesabsorbed >= MAX_DISEASE_ABSORBED) {
				//Destroy (this.gameObject, 2.0f);
				destroyMe = true;
			}
		} else {
			return;
		}
	}
	
	// Movement Code
	void Update () {
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
