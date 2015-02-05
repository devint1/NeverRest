using UnityEngine;
using System.Collections;

public class WhiteBloodCell : MonoBehaviour {
	public GameObject currentBlock;
	public GameControl gameControl;
	public bool isSelected = false;
	public bool destroyMe = false;
	public Block destBlock = null; // Block the cell is moving to
	
	const float SPEED = 0.1f;
	const float MAX_TURN_DEGREES = 90f;
	const int MAX_DISEASE_ABSORBED = 8;

	float turnDegrees = 0f;
	int diseasesabsorbed = 0;
	
	void Start(){
		gameControl.StartCoroutine(ChangeTurnDegreesCycle());
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
			diseaseScript.currentBlock.GetComponent<Block> ().diseases.Remove(diseaseScript);
			diseaseScript.currentBlock = this.gameObject;
			diseaseScript.captured = true;
			diseaseScript.speed *= 2;

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
		if (!currentBlock.GetComponent<Renderer>().bounds.Contains (this.transform.position)) {
			var direction = currentBlock.transform.position - this.transform.position;
			var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else {
			Vector3 turnRotation = new Vector3(0f, 0f, turnDegrees * Time.deltaTime);
			this.transform.Rotate(turnRotation);
		}
		this.transform.Translate(this.transform.right * SPEED * Time.deltaTime, Space.World);

		// Check to see if cell has collided with its destination block
		if (destBlock) {
			// Normal collision detection seems to not work for Unity2D
			// Using overlapping bounds detection instead
			if (this.renderer.bounds.Intersects(destBlock.renderer.bounds)) {
				destBlock.decreaseWBCsTargeting();
				destBlock = null;
			}
		}
	}
	
	IEnumerator ChangeTurnDegreesCycle() {
		yield return new WaitForSeconds(1);
		
		turnDegrees = Random.Range (-MAX_TURN_DEGREES, MAX_TURN_DEGREES);
		
		StartCoroutine(ChangeTurnDegreesCycle());
	}

}
