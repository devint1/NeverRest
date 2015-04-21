using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ClipperLib;

public enum BlockType { CHEST, LIMB, OTHER }

public class Block : MonoBehaviour {
	public GameControl gameControl;
	public Transform StatsPoint;
	public BlockType blockType;
	public ArrayList diseases = new ArrayList();
	public ArrayList wounds = new ArrayList();
	public ArrayList platelets = new ArrayList();
	public GameObject destMarkPrefab;
	public bool notClotted = true;

	public float oxygenLevel = 1.0f; //Should range from 1(good) to 0(very bad)
	public float temperaturePercent = 1.0f; //Should range from 1(good, 98.6 degrees) to 0(very bad, 70 degrees, aka. dead)
	public float overallHealth = 1.0f;
	public bool dead = false;

	private Vector3 mousePos;
	private Tesselator tesselator;
	private Animator animator;
	private bool showStats = false;

	public const int MAX_NUM_DISEASE_PER_BLOCK = 50;
	private const float HEALH_REGENERATION = 0.03f;
	private const float DAMAGE_PER_DISEASE = 0.001f;
	private const float COLD_DAMAGE = 0.03f;
	private const float NO_OXYGEN_DAMAGE = 0.03f;
	private const float DAMAGE_PER_WOUND = 0.01f;
	private const float WOUND_HEAL_PER_PLATELET = 0.15f;


	void ShrinkPolygon(double scale, ref PolygonCollider2D poly){
		List<IntPoint> subj;
		List<List<IntPoint>> solution = new List<List<IntPoint>>();
		ClipperOffset co = new ClipperOffset();
		Vector2[] path;
		Vector2 tempPoint;
		int pathNdx = 0;
		int pointNdx = 0;

		for (int ndx = 0; ndx < poly.pathCount; ndx++) {
			subj = new List<IntPoint>();
			foreach (Vector2 point in poly.GetPath(0)){
				subj.Add( new IntPoint( point.x * 1000, point.y * 1000 ));
			}
			co.AddPath (subj, JoinType.jtMiter, EndType.etClosedPolygon);
		}
		co.Execute (ref solution, scale);

		foreach (List<IntPoint> list in solution) {
			path = new Vector2[solution[pathNdx].Count];
			pointNdx = 0;
			foreach (IntPoint point in list){
				tempPoint = new Vector2();
				tempPoint.x = (float) point.X / 1000;
				tempPoint.y = (float) point.Y / 1000;
				path.SetValue( tempPoint, pointNdx );
				pointNdx++;
			}
			poly.SetPath( pathNdx, path );
			pathNdx++;
		}
	}

	void Start() {
		Vector2[] points;
		var collider = this.GetComponent<PolygonCollider2D> ();
		if (collider) {
			points = collider.points;
			ShrinkPolygon (-200, ref collider);
			tesselator = new Tesselator (collider.points);
			collider.points = points;
		}
		animator = GetComponent<Animator>();
	}

	void Update() {
		//Change color based on healh
		Color tmpCol = GetComponent<Renderer>().material.color;
		GetComponent<Renderer>().material.color = Color.Lerp(Color.white, Color.black, 1.0f-overallHealth);
		tmpCol.r = GetComponent<Renderer>().material.color.r;
		tmpCol.g = GetComponent<Renderer>().material.color.g;
		tmpCol.b = GetComponent<Renderer>().material.color.b;
		GetComponent<Renderer>().material.color = tmpCol;

		if(dead) {
			DestroyAllDiseases();
			return;
		}

		// Handle animation states
		if (animator != null) {
			if(diseases.Count == 0 && wounds.Count == 0) {
				animator.Play("Still");
			} else {
				animator.Play("Flashing");
			}
		}
		if (!gameControl.IsPaused()) {
			//If vitals are mostly good, slowly increase health. Else, take damage
			if (oxygenLevel >= 0.75f && temperaturePercent >= 0.75f && diseases.Count == 0 && wounds.Count == 0 && overallHealth <= 1.0) {
				overallHealth += HEALH_REGENERATION * Time.deltaTime;
			} else {
				if (oxygenLevel < 0.75f) {
					overallHealth -= (NO_OXYGEN_DAMAGE * (1.0f - oxygenLevel)) * Time.deltaTime;
				}
				if (temperaturePercent < 0.75f) {
					overallHealth -= (COLD_DAMAGE * (1.0f - temperaturePercent)) * Time.deltaTime;
				}
				if( !gameControl.IsPaused()){
				overallHealth -= DAMAGE_PER_DISEASE * diseases.Count * Time.deltaTime;
				overallHealth -= DAMAGE_PER_WOUND * wounds.Count * Time.deltaTime;
				}
			}

			if (overallHealth <= 0) {
				Die();
			}

			foreach (Wound wound in wounds) {
				 
					wound.health -= WOUND_HEAL_PER_PLATELET * wound.plateletsCount * Time.deltaTime;
					//Debug.Log(wound.health);
			 

				/*
			if (Time.time - wound.plateletArrivalTime > Wound.PLATELETE_INEFFECTIVENES_TIME) {
				wound.plateletsCount = wound.plateletsCount > 0 ? wound.plateletsCount - 1 : 0;
			}
			for(int i = 0; i < platelets.Count; i++) {
				wound.plateletsCount++;
				wound.plateletArrivalTime = Time.time;
				/*
				Platelets plate = (Platelets)(platelets[i]);
				gameControl.foodLevel += 0.8f * GameControl.PLATELET_FOOD_RATE;
				gameControl.selected.Remove (plate.gameObject);
				gameControl.platelets.Remove (plate);
				platelets.Remove(plate);
				Destroy (plate.gameObject);
				/
			}

			if(wound.health <= 0) {
				for(int i = 0; i < Mathf.Min(Wound.NUM_PLATELETS_CONSUMED, platelets.Count); i++) {
					Platelets plate = (Platelets)(platelets[i]);
					gameControl.foodLevel += 0.8f * GameControl.PLATELET_FOOD_RATE;
					gameControl.selected.Remove (plate.gameObject);
					gameControl.platelets.Remove (plate);
					platelets.Remove(plate);
					Destroy (plate.gameObject);
				}
			}
			*/
			}
		}
	}

	// Block clicked. Send selected WhiteBloodCell here
	public void OnMouseOver() {
		showStats = true;

		if(dead) {
			return;
		}

		//Quit out if not a right click
		if (!Input.GetMouseButtonDown(1)){
			return;
		}
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		foreach (GameObject obj in gameControl.selected) {
			if(obj.tag == "WhiteBloodCell") {
				WhiteBloodCell cell = obj.GetComponent<WhiteBloodCell> ();
				//cell.renderer.material.color = Color.white;
				//cell.isSelected = false;
				cell.SetDestination (this, mousePos);
				gameControl.isSelected = true;
			}
			else if(obj.tag == "Platelet") {
				Platelets plate = obj.GetComponent<Platelets> ();
				//plate.renderer.material.color = Color.yellow;
				//plate.isSelected = false;
				plate.SetDestination (this, mousePos);
				gameControl.isSelected = true;
			}
		}

		StartCoroutine(FireMouseClick());
		//gameControl.selected.Clear();
	}

	public void OnMouseExit() {
		showStats = false;
	}

	void OnGUI(){
		if (showStats) {
			Vector3 position = Camera.main.WorldToScreenPoint (StatsPoint.position);
			if (gameControl.toggleRBC) {
				GUI.TextArea(new Rect(position.x,Screen.height-position.y,98,65), "Health:    " +(int)(overallHealth*100) + "\nOxygen:  " + oxygenLevel*100 + "%" + "\nDiseases:" + diseases.Count + "\nClotted: " + !notClotted);
			} else {
				GUI.TextArea(new Rect(position.x,Screen.height-position.y,98,55), "Health:    " +(int)(overallHealth*100) + "\nOxygen:  " + oxygenLevel*100 + "%" + "\nDiseases:" + diseases.Count);
			}
		}
	}
	
	public Vector3 GetRandomPoint() {
		if (tesselator == null) {
			return GetExitPoint().transform.position;
		}
		// Needs some work
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

	public IEnumerator FireMouseClick()
	{
		if(dead) {
			//return false;
		}

		if (!destMarkPrefab.activeSelf) {
			destMarkPrefab.SetActive (true);
		}
		if (gameControl.isSelected) {
			GameObject mouseTarget = (GameObject)Instantiate (destMarkPrefab, (Vector2)mousePos, Quaternion.identity);
			mouseTarget.transform.localScale += new Vector3 (1f, 1f, 1f);
			Color c = Color.green;
			c.a = 1f;
			while (c.a > 0) {
				yield return new WaitForSeconds (.1f);
				c.a -= .1f;
				mouseTarget.GetComponent<Renderer>().material.color = c;
			}
			gameControl.isSelected = false;
			//gameControl.selected.Clear ();
			Destroy (mouseTarget);
		}
	}

	void OnMouseDown() {
		if(dead) {
			return;
		}

		if (gameControl.toggleRBC) {
			notClotted = !notClotted;
		}

	}

	void DestroyAllDiseases() {
		for (int i = diseases.Count-1; i>=0; i--) {
			Disease toDestroy = (Disease)diseases[i];
			diseases.Remove(toDestroy);
			GameObject.Destroy( toDestroy.gameObject );
		}
	}

	public void Die(){
		overallHealth = 0.0f;
		oxygenLevel = 0.0f; 
		temperaturePercent = 0.0f;
		dead = true;
		gameControl.deadBlocks++;

		// Kill off all "child" body parts
		foreach (ExitPoint e in GetExitPoints()) {
			if(!e.isExitToHeart) {
				e.nextBlock.Die();
			}
		}
	}
}
