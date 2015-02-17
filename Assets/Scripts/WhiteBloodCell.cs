using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public Block currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public AudioClip spawnSound =  null;

	public Vector3 destination; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now
	
	const float SPEED = 0.0075f;
	const int MAX_DISEASE_ABSORBED = 8;
	
	int diseasesabsorbed = 0;
	Block destBlock = null; // Block the cell is moving to
	bool destChanged = false;
	Vector2 userDest;
	bool hasUserDest; //Need to use this since userDest cannot = null
	ExitPoint pathingToEntrance = null;
	ArrayList capture = new ArrayList ();

	public void Start(){
		AudioSource temp = gameObject.AddComponent<AudioSource> ();
		temp.clip = spawnSound;
		temp.Play ();
	}

	public void SetDestination(Block dest, Vector2 coords){
		destBlock = dest;
		destChanged = true;
		userDest = coords;
		hasUserDest = true;
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
		foreach (Disease disease in currentBlock.diseases){
			if (Vector3.Distance (disease.transform.position, transform.position) < .2){
				if (!disease.captured) {
					disease.captured = true;
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
		if( gameControl.CheckIfPaused() ){
			return;
		}
		CheckCollisionOnDisease ();

		//If we are at current way point or the destination has been changed
		if (destination == null || Vector2.Distance (destination, this.transform.position) < .03 || destChanged) {
			//If we have arrived at our exit node, our next node should be the next cells entrance node
			//Dest change is to check if it was a destchange request or we reach current node
			//Otherwise if we are not in the correct block we need to find the next exit
			if (destBlock && destBlock != currentBlock) {
				foreach (ExitPoint exitPoint in currentBlock.GetExitPoints()) {
					if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, currentBlock) ) {
						destination = exitPoint.gameObject.transform.position;
						currentBlock = exitPoint.nextBlock;
						break;
					}
				}
			}
			//If we are in the correct block we need to go to users click
			else if (hasUserDest){
				destination = (Vector3)userDest;
				hasUserDest = false;
			}
			//Last option is going to a random waypoint
			else{
				destination = currentBlock.GetRandomPoint();
			}
			if( destChanged ){
				destChanged = false;
			}
		}
		if (destination != null) {
			Vector2 directionToDestination = ((Vector2)destination - (Vector2)this.transform.position).normalized;			
			this.transform.position = new Vector3 ((directionToDestination.x * SPEED) + this.transform.position.x,
			                                       (directionToDestination.y * SPEED) + this.transform.position.y,
			                                       this.transform.position.z);
		}
	}
	
	GameObject FindExitPointToDestination(Block current, Block destination) {
		foreach (ExitPoint exitPoint in currentBlock.GetExitPoints()) {
			if(exitPoint.nextBlock == destination) {
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

		//TODO - We should cache lookups
		foreach(ExitPoint exitPoint in exit.GetComponent<ExitPoint>().nextBlock.GetExitPoints()) {
			if( curBlock != exitPoint.gameObject.GetComponent<ExitPoint>().nextBlock ) {
				if( ExitPointLeadsToDestination(exitPoint.gameObject, destination, exit.GetComponent<ExitPoint>().nextBlock) ) {
					return true;
				}
			}
		}
		
		return false;
	}
}
