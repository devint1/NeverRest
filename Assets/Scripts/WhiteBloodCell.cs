using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public Block currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public AudioClip spawnSound =  null;
	public GameObject headingToward; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now
	public float speed = 0.0075f;

	const int MAX_DISEASE_ABSORBED = 8;
	
	int diseasesabsorbed = 0;
	Block destBlock = null; // Block the cell is moving to
	bool destChanged = false;
	Vector2 userDest;
	bool hasUserDest; //Need to use this since userDest cannot = null
	ExitPoint pathingToEntrance = null;

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
		if( gameControl.CheckIfPaused() ){
			return;
		}
		CheckCollisionOnDisease ();
		
		if (!currentBlock.notClotted)
			speed = 0.00001f;
		else
			speed = 0.0075f;

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
				headingToward = currentBlock.GetRandomPoint();
			}
			if( destChanged ){
				destChanged = false;
			}
		}
		if (headingToward) {
			Vector2 directionToDestination = ((Vector2)headingToward.transform.position - (Vector2)this.transform.position).normalized;			
			this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
			                                       (directionToDestination.y * speed) + this.transform.position.y,
			                                       this.transform.position.z);
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
