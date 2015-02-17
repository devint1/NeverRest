using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BlockType { HEART, OTHER }

public class Block : MonoBehaviour {
	public GameControl gameControl;
	public BlockType blockType;
	public ArrayList diseases = new ArrayList();
	public GameObject destMarkPrefab;

	private Vector3 mousePos;
	private Tesselator tesselator;

	public static int MAX_NUM_DISEASE_PER_BLOCK = 200;
	
	void Start() {
		var collider = this.GetComponent<PolygonCollider2D> ();
		if (collider) {
			tesselator = new Tesselator (collider.points);
		}
	}

	// Block clicked. Send selected WhiteBloodCell here
	void OnMouseOver() {
		//Quit out if not a right click
		if (!Input.GetMouseButtonDown(1)){
			return;
		}
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		foreach (WhiteBloodCell cell in gameControl.selected) {
			cell.renderer.material.color = Color.white;
			cell.isSelected = false;
			cell.SetDestination (this, mousePos);
		}
		StartCoroutine(FireMouseClick());
		gameControl.selected.Clear();
	}
	
	public Vector3 GetRandomPoint() {
		// Needs some work
		var collider = this.GetComponent<PolygonCollider2D> ();
		Vector2[] triangle = tesselator.GetRandomTriangle ();
		float randomWeight1 = Random.value;
		float randomWeight2 = Random.value;

		Vector2 randomPoint = (1 - Mathf.Sqrt (randomWeight1)) * triangle [0]
		+ (Mathf.Sqrt (randomWeight1) * (1 - randomWeight2)) * triangle [1]
		+ (randomWeight2 * Mathf.Sqrt (randomWeight1)) * triangle [2];
	
		Vector3 randomPoint3D = new Vector3 (randomPoint.x + transform.position.x,
		                                     randomPoint.y + transform.position.y,
		                                     2);

		return randomPoint3D;
	}

	public ExitPoint GetExitPoint() {
		return transform.gameObject.GetComponentInChildren<ExitPoint>();
	}

	public ExitPoint[] GetExitPoints() {
		return transform.gameObject.GetComponentsInChildren<ExitPoint>();
	}

	IEnumerator FireMouseClick()
	{
		if (!destMarkPrefab.activeSelf) {
			destMarkPrefab.SetActive(true);
		}
		GameObject mouseTarget = (GameObject)Instantiate (destMarkPrefab, (Vector2)mousePos, Quaternion.identity);
		Color c = Color.green;
		while (c.a > 0){
			yield return new WaitForSeconds(.1f);
			c.a -= .05f;
			mouseTarget.renderer.material.color = c;
		}
		Destroy (mouseTarget);
	}
}
