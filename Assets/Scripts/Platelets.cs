using UnityEngine;
using System.Collections;

public class Platelets : MonoBehaviour {
	public Block currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public AudioClip spawnSound =  null;
	public float speed = 0.0075f;
	public Vector3 destination; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now
	public float spawnTime;
	public bool attackingWound = false;
	public Wound woundToAttack;

	Block destBlock = null; // Block the cell is moving to
	Block nextBlock = null;
	bool destChanged = false;
	Vector2 userDest;
	bool hasUserDest; //Need to use this since userDest cannot = null
	
	public void Start(){
		nextBlock = currentBlock;
		this.gameObject.tag = "Platelet";
	}

	public void SetDestination(Block dest, Vector2 coords){
		destBlock = dest;
		destChanged = true;
		userDest = coords;
		hasUserDest = true;
	}

	public void Select(){
		if(!isSelected){
			gameControl.selected.Add (this.gameObject);
			gameObject.renderer.material.color = Color.green;
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

	// Update is called once per frame
	void Update () {
		if( gameControl.IsPaused() ){
			return;
		}

		if (Time.time - spawnTime > 120) {
			gameControl.selected.Remove (this.gameObject);
			gameControl.platelets.Remove (this);
			currentBlock.platelets.Remove(this);
			Destroy (this.gameObject);
			return;
		}
		
		if (this.speed != gameControl.rbcSpeed) {
			this.speed = gameControl.rbcSpeed / 250.0f;
		}

		if (!currentBlock.notClotted)
			speed = 0.00001f;
		else
			speed = gameControl.rbcSpeed / 250.0f;

		if (currentBlock.wounds.Count > 0 && !attackingWound) {
			woundToAttack = (Wound)(currentBlock.wounds[0]); 
			destination = woundToAttack.transform.position;
			attackingWound = true;
		}

		//If we are at current way point or the destination has been changed
		if (Vector2.Distance (destination, this.transform.position) < .03 || destChanged) {
			if(attackingWound) {
				if(woundToAttack != null) {
					woundToAttack.plateletsCount++;
					woundToAttack.plateletArrivalTime = Time.time;
				}
				gameControl.selected.Remove (this.gameObject);
				gameControl.platelets.Remove (this);
				currentBlock.platelets.Remove(this);
				Destroy (this.gameObject);
				return;
			}
			// Need to replace old destination with new one by setting nextBlock = currentBlock,
			// otherwise the cells will try to move to old block's exitPoint first
			if (destChanged) {
				nextBlock = currentBlock;
				destChanged = false;
			}
			//If we have arrived at our exit node, our next node should be the next cells entrance node
			//Dest change is to check if it was a destchange request or we reach current node
			//Otherwise if we are not in the correct block we need to find the next exit
			if (destBlock && destBlock != nextBlock) {
				foreach (ExitPoint exitPoint in nextBlock.GetExitPoints()) {
					if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, nextBlock) ) {
						destination = exitPoint.gameObject.transform.position;
						nextBlock = exitPoint.nextBlock;
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
				destination = nextBlock.GetRandomPoint();
			}
		}

		Vector2 directionToDestination = ((Vector2)destination - (Vector2)this.transform.position).normalized;			
		this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
		                                       (directionToDestination.y * speed) + this.transform.position.y,
		                                       this.transform.position.z);
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (nextBlock && other.gameObject.name == nextBlock.gameObject.name) {
			currentBlock.platelets.Remove (this);
			currentBlock = nextBlock;
			currentBlock.platelets.Add (this);
		}
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
