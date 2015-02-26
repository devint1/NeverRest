using UnityEngine;
using System.Collections;

public class RedBloodScript : MonoBehaviour {
	public Block currentBlock;		//block cell is currently in
	public Block origBlock;			//block the red blood cell originates from, aka target block
	public Block heartBlock;		//pointer to the heart
	public GameControl gameControl;
	public bool returnToHeart;		//set to true if on path back to heart, false otherwise
	public Vector3 destination; //Point, Exitpoint, or disease that WhiteBloodCell is moving towards right now
	public bool oxygenated = true;	//set to false upon arrival at destination (room)
	public float speed = 0.0075f;
	public Block destBlock = null; // Block the cell is moving to
	public float spawnTime;

	bool destChanged = false;
	Vector2 userDest;
	bool hasUserDest; //Need to use this since userDest cannot = null
	bool leavingCurrBlock = true;
	
	// Use this for initialization
	void Start () {
		
	}

	void SpawnRBC() {
		if (gameControl.liveRBCs < 2 * gameControl.numRBCs) {
			Vector3 randpt = gameControl.redBloodSpawnPoint.GetRandomPoint ();
			GameObject newRBC = (GameObject)Instantiate (gameControl.redBloodCellPrefab, new Vector3 (randpt.x, randpt.y, 1.0f), this.transform.rotation);
			RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
			newRedScript.currentBlock = heartBlock;
			newRedScript.destination = gameControl.redBloodSpawnPoint.GetRandomPoint ();
			newRedScript.origBlock = origBlock;
			newRedScript.destBlock = newRedScript.origBlock;
			newRedScript.heartBlock = heartBlock;
			newRedScript.gameControl = gameControl;
			newRedScript.spawnTime = Time.time;
			gameControl.liveRBCs++;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( gameControl.CheckIfPaused() ){
			return;
		}
		
		if (Time.time - spawnTime > 120) {
			Destroy (this.gameObject);
			gameControl.liveRBCs--;
			SpawnRBC ();
			return;
		}
		
		if (!gameControl.toggleRBC) {
			//Destroy(this.gameObject);
			//Destroy(this);
			this.renderer.enabled = false;
		} else {
			this.renderer.enabled = true;
		}

		if (this.speed != gameControl.rbcSpeed) {
			this.speed = gameControl.rbcSpeed / 250.0f;
		}
		
		if(!oxygenated)
			this.renderer.material.SetColor("_Color", new Color(172.0f / 255.0f,0.0f,0.0f));
		else
			this.renderer.material.SetColor("_Color", new Color(255.0f / 255.0f,0.0f,0.0f));

		if (!currentBlock.notClotted)
			speed = gameControl.rbcSpeed / 1000.0f;
		else
			speed = gameControl.rbcSpeed / 250.0f;
		
		//If we are at current way point or the destination has been changed
		if (Vector2.Distance (destination, this.transform.position) < .03 || destChanged) {
			//If we have arrived at our exit node, our next node should be the next cells entrance node
			//Dest change is to check if it was a destchange request or we reach current node
			if (destBlock && destBlock != currentBlock) {
				foreach (ExitPoint exitPoint in currentBlock.GetExitPoints()) {
					if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, currentBlock) ) {
						destination = exitPoint.gameObject.transform.position;
						currentBlock = exitPoint.nextBlock;
						break;
					}
				}
				if(!leavingCurrBlock) {
					if(origBlock != heartBlock) {
						if(returnToHeart) {
							oxygenated = false;		//set the color to dark red
						}
						else {
							oxygenated = true;
						}
					}
					else {
						if(!returnToHeart) {
							destination = gameControl.body.heart.transform.position;
							oxygenated = false;
							returnToHeart = true;
						}
						else {
							destination = currentBlock.GetRandomPoint();
							oxygenated = true;
							returnToHeart = false;
						}
					}
					leavingCurrBlock = true;
				}
			}
			//Last option is going to a random waypoint
			else if( currentBlock != destBlock ) {
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
					if(!returnToHeart) {
						destination = gameControl.body.heart.transform.position;
						oxygenated = false;
						returnToHeart = true;
					}
					else {
						destination = currentBlock.GetRandomPoint();
						oxygenated = true;
						returnToHeart = false;
					}
				}
			}
			else {
				leavingCurrBlock = false;
				if(origBlock != heartBlock) {
					if(!returnToHeart) {
						returnToHeart = true;
						destBlock = heartBlock;
						destChanged = true;
					}
					else {
						returnToHeart = false;
						destBlock = origBlock;
						destChanged = true;
					}
				}
				else {
					if(!returnToHeart) {
						destination = gameControl.body.heart.transform.position;
						oxygenated = false;
						returnToHeart = true;
					}
					else {
						destination = currentBlock.GetRandomPoint();
						oxygenated = true;
						returnToHeart = false;
					}
				}
				foreach (ExitPoint exitPoint in currentBlock.GetExitPoints()) {
					if( ExitPointLeadsToDestination(exitPoint.gameObject, destBlock, currentBlock) ) {
						destination = exitPoint.gameObject.transform.position;
						currentBlock = exitPoint.nextBlock;
						break;
					}
				}
			}
			if( destChanged ){
				destChanged = false;
			}
		}
		Vector2 directionToDestination = ((Vector2)destination - (Vector2)this.transform.position).normalized;			
		this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
		                                       (directionToDestination.y * speed) + this.transform.position.y,
		                                       this.transform.position.z);
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
