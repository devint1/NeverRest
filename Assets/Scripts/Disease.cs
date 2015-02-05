using UnityEngine;
using System.Collections;

public class Disease : MonoBehaviour {
	public GameControl gameControl;
	public GameObject currentBlock;
	public GameObject diseasePrefab;
	public bool captured = false;
	public float speed = 0.07f;
	public float heartHealthDamagePerSec = 0.001f;

	const float MAX_TURN_DEGREES = 90f;
	float turnDegrees = 0f;

	void Start(){
		StartCoroutine(MoveCycle());
		StartCoroutine(DuplicateCycle());
		StartCoroutine(ChangeTurnDegreesCycle());
		StartCoroutine(DamageHeart());

		if(currentBlock != null)
			currentBlock.GetComponent<Block> ().diseases.Add (this);
	}

	// Movement Code
	void Update () {
		if (!currentBlock) {
			Destroy(this.gameObject);
		}

		// Disease has been captured and sucked in. Immobilize and kill ourselves
		if (captured && (currentBlock.transform.position - this.transform.position).magnitude < 0.025) {
			Destroy(gameObject.GetComponent<Rigidbody>());
			Destroy(gameObject.GetComponent<CircleCollider2D>());
			transform.parent = currentBlock.transform;
			Destroy(this);
		}

		if (!currentBlock.GetComponent<Renderer> ().bounds.Contains (this.transform.position) || captured) {
			var direction = currentBlock.transform.position - this.transform.position;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else {
			Vector3 turnRotation = new Vector3( 0f, 0f, turnDegrees * Time.deltaTime);
			this.transform.Rotate(turnRotation);
		}

		this.transform.Translate(this.transform.right * speed * Time.deltaTime, Space.World);
	}

	// Sends to next block every x seconds
	IEnumerator MoveCycle() {
		yield return new WaitForSeconds(30);

		if (!captured && currentBlock.GetComponent<Block> ().nextBlock.GetComponent<Block> ().diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK) {
			currentBlock.GetComponent<Block> ().diseases.Remove (this);
			currentBlock = currentBlock.GetComponent<Block> ().nextBlock;
			currentBlock.GetComponent<Block> ().diseases.Add (this);
			StartCoroutine (MoveCycle ());
		} else if (!captured) {
			StartCoroutine (MoveCycle ());
		}
	}

	// Varries direction were traveling in every x seconds
	IEnumerator ChangeTurnDegreesCycle() {
		yield return new WaitForSeconds(1);

		if (!captured) {
			turnDegrees = Random.Range (-MAX_TURN_DEGREES, MAX_TURN_DEGREES);
			StartCoroutine (ChangeTurnDegreesCycle());
		}
	}

	// Creates new disease every x seconds
	IEnumerator DuplicateCycle() {
		yield return new WaitForSeconds(15);

		if (!captured && currentBlock.GetComponent<Block> ().diseases.Count < Block.MAX_NUM_DISEASE_PER_BLOCK) {
			GameObject newDisease = (GameObject)Instantiate (diseasePrefab, this.transform.position, this.transform.rotation);
			newDisease.GetComponent<Disease> ().currentBlock = currentBlock;
			newDisease.GetComponent<Disease> ().gameControl = gameControl;
			StartCoroutine (DuplicateCycle ());
		} else if (!captured) {
			StartCoroutine (DuplicateCycle ());
		}
	}

	IEnumerator DamageHeart() {
		yield return new WaitForSeconds(1);
		
		if (!captured && currentBlock.GetComponent<Block>().blockType == BlockType.HEART) {
			gameControl.healthLevel -= heartHealthDamagePerSec;
			Debug.Log(gameControl.healthLevel);
		}
		StartCoroutine(DamageHeart());
	}
}
