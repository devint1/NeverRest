using UnityEngine;
using System.Collections;

public class Disease : MonoBehaviour {
	public GameControl gameControl;
	public Block currentBlock;
	public GameObject diseasePrefab;
	public bool captured = false;
	public bool removedFromCell = false;
	public float speed = 0.005f;
	public float heartHealthDamagePerSec = 0.01f;
	public WhiteBloodCellType type;
	
	public Vector3 destination;

	Block destBlock;
	const int MAX_DISEASE_RESPAWN_TIME = 15;
	const int MIN_DISEASE_RESPAWN_TIME = 40;
	
	void Start() {

		if (currentBlock) {
			destBlock = currentBlock;
			destination = destBlock.GetRandomPoint ();
		}
		
		StartCoroutine(MoveCycle());
		StartCoroutine(DuplicateCycle());
		
		if(currentBlock != null)
			currentBlock.diseases.Add (this);
	}
	
	// Movement Code
	void Update () {
		if( gameControl.IsPaused() ){
			return;
		}
		
		Vector2 directionToDestination = ((Vector2)destination - (Vector2)this.transform.position).normalized;

		if (!gameControl.toggleWBC)
			this.GetComponent<Renderer>().enabled = false;
		else
			this.GetComponent<Renderer>().enabled = true;
		
		if (this.speed != gameControl.rbcSpeed) {
			this.speed = gameControl.rbcSpeed / 250.0f;
		}
		
		if (!currentBlock.notClotted)
			speed = gameControl.rbcSpeed / 1000.0f;
		else
			speed = gameControl.rbcSpeed / 250.0f;

		//If disease has reached is destination
		if ((destination - this.transform.position).magnitude < 0.07) {
			//If were captured then we just reached the inside of White Blood Cell. Immobilze ourselves and attach to White Blood Cell
			if(captured) {
				Destroy(gameObject.GetComponent<Rigidbody>());
				Destroy(gameObject.GetComponent<CircleCollider2D>());
			}
			else {
				// Else we just reached a waypoint. Choose next destination.
				// Roll a random dice with 3% chance of exiting
				float dice = Random.value;
				bool exit = dice >= 0.97f;
				if(exit) {
					ExitPoint[] exitPoints = destBlock.GetExitPoints();
					ExitPoint exitPoint = exitPoints[(Random.Range( 0, exitPoints.Length ))]; //Not -1 since exclusive
					destBlock = exitPoint.nextBlock;
					destination = destBlock.GetRandomPoint();
				}
				else {
					destination = destBlock.GetRandomPoint();
				}
				StartCoroutine(ChangeRotation(directionToDestination));
			}
		}

		if (!captured) {
			this.transform.position = new Vector3 ((directionToDestination.x * speed) + this.transform.position.x,
                               (directionToDestination.y * speed) + this.transform.position.y,
                               this.transform.position.z);
		}
	}

	void OnMouseOver() {
		currentBlock.OnMouseOver ();
	}
	
	void OnMouseExit() {
		currentBlock.OnMouseExit ();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (destBlock && other.gameObject.name == destBlock.name) {
			if(captured) {
				currentBlock = destBlock;
			} else {
				currentBlock.diseases.Remove (this);
				currentBlock = destBlock;
				currentBlock.diseases.Add (this);
			}
		}
	}
	
	// Sends to next block every x seconds
	IEnumerator MoveCycle() {
		yield return new WaitForSeconds(30);
		StartCoroutine (MoveCycle ());
	}
	
	// Creates new disease every x seconds
	IEnumerator DuplicateCycle() {
		int waitFor = Random.Range (MIN_DISEASE_RESPAWN_TIME, MAX_DISEASE_RESPAWN_TIME);
		yield return new  WaitForSeconds(waitFor);

		if(!gameControl.isPause){
			if (!captured && currentBlock.diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK) {
				GameObject newDisease = (GameObject)Instantiate (diseasePrefab, this.transform.position, this.transform.rotation);
				Disease newDiseaseScript = newDisease.GetComponent<Disease>();
				newDiseaseScript.gameControl = gameControl;
				newDiseaseScript.destination = destination;
				++gameControl.numDiseaseCells;
			}
		}
			
		StartCoroutine(DuplicateCycle());
	}

	IEnumerator ChangeRotation(Vector2 directionToDestination) {
		float time = 0.5f;
		float angle = Mathf.Atan2(directionToDestination.y, directionToDestination.x) * Mathf.Rad2Deg;
		Quaternion currentRotation = this.transform.rotation;
		Quaternion destinationRotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
		for (float t = 0.0f; t < time; t += Time.deltaTime / time) {
			this.transform.rotation = Quaternion.Lerp(currentRotation, destinationRotation, (t / time));
			yield return null;
		}
	}
	
	public void BeenCapturedBy(GameObject whiteBloodCell) {
		transform.position = whiteBloodCell.transform.position + new Vector3 (Random.Range (-.07f, .07f), Random.Range (-.07f, .07f), 0);
		transform.parent = whiteBloodCell.transform;
		captured = true;
		speed *= 2.5f;
	}
}
