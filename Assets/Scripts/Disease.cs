using UnityEngine;
using System.Collections;

public class Disease : MonoBehaviour {
	public GameControl gameControl;
	public GameObject current_Block;
	public GameObject disease_Prefab;
	public bool captured = false;
	public float speed = 0.07f;
	public float heartHealthDamagePerSec = 0.001f;

	float MAX_TURN_DEGREES = 90f;
	float turn_Degrees = 0f;

	void Start(){
		StartCoroutine(Move_Cycle());
		StartCoroutine(Duplicate_Cycle());
		StartCoroutine(Change_Turn_Degrees_Cycle());
		StartCoroutine (damageHeart ());
	}

	// Movement Code
	void Update () {
		if (!current_Block) {
			Destroy(this.gameObject);
		}

		// Disease has been captured and sucked in. Immobilize and kill ourselves
		if (captured && (current_Block.transform.position - this.transform.position).magnitude < 0.025) {
			Destroy(gameObject.GetComponent<Rigidbody>());
			Destroy(gameObject.GetComponent<CircleCollider2D>());
			transform.parent = current_Block.transform;
			Destroy(this);
		}

		if (!current_Block.GetComponent<Renderer> ().bounds.Contains (this.transform.position) || captured) {
			var direction = current_Block.transform.position - this.transform.position;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else {
			Vector3 turn_rotation = new Vector3( 0f, 0f, turn_Degrees * Time.deltaTime);
			this.transform.Rotate(turn_rotation);
		}

		this.transform.Translate(this.transform.right * speed * Time.deltaTime, Space.World);
	}

	// Sends to next block every x seconds
	IEnumerator Move_Cycle() {
		yield return new WaitForSeconds(30);

		if (!captured) {
			current_Block = current_Block.GetComponent<Block> ().next_Block;
			StartCoroutine (Move_Cycle ());
		}
	}

	// Varries direction were traveling in every x seconds
	IEnumerator Change_Turn_Degrees_Cycle() {
		yield return new WaitForSeconds(1);

		if (!captured) {
			turn_Degrees = Random.Range (-MAX_TURN_DEGREES, MAX_TURN_DEGREES);
			StartCoroutine (Change_Turn_Degrees_Cycle ());
		}
	}

	// Creates new disease every x seconds
	IEnumerator Duplicate_Cycle() {
		yield return new WaitForSeconds(15);

		if(!captured) {
			GameObject new_Disease = (GameObject)Instantiate (disease_Prefab, this.transform.position, this.transform.rotation);
			new_Disease.GetComponent<Disease> ().current_Block = current_Block;
			StartCoroutine(Duplicate_Cycle());
		}
	}

	IEnumerator damageHeart() {
		yield return new WaitForSeconds(1);
		
		if (!captured && current_Block.GetComponent<Block>().blockType == BlockType.HEART) {
			gameControl.healthLevel -= heartHealthDamagePerSec;
			Debug.Log(gameControl.healthLevel);
		}
		StartCoroutine(damageHeart());
	}
}
