﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
	private bool showStats = false;

	public const int MAX_NUM_DISEASE_PER_BLOCK = 200;
	private const float HEALH_REGENERATION = 0.03f;
	private const float DAMAGE_PER_DISEASE = 0.001f;
	private const float COLD_DAMAGE = 0.03f;
	private const float NO_OXYGEN_DAMAGE = 0.03f;
	private const float DAMAGE_PER_WOUND = 0.05f;
	private const float WOUND_HEAL_PER_PLATELET = 0.15f;
	
	void Start() {
		var collider = this.GetComponent<PolygonCollider2D> ();
		if (collider) {
			tesselator = new Tesselator (collider.points);
		}
	}

	void Update() {
		//Change color based on healh
		renderer.material.color = Color.Lerp(Color.white, Color.black, 1.0f-overallHealth);

		if(dead) {
			DestroyAllDiseases();
			return;
		}

		//If vitals are mostly good, slowly increase health. Else, take damage
		if(oxygenLevel >= 0.75f && temperaturePercent >= 0.75f && diseases.Count == 0 && wounds.Count == 0 && overallHealth <= 1.0) {
			overallHealth += HEALH_REGENERATION * Time.deltaTime;
		}
		else {
			if(oxygenLevel < 0.75f){
				overallHealth -= (NO_OXYGEN_DAMAGE*(1.0f-oxygenLevel)) * Time.deltaTime;
			}
			if(temperaturePercent < 0.75f){
				overallHealth -= (COLD_DAMAGE*(1.0f-temperaturePercent)) * Time.deltaTime;
			}
			overallHealth -= DAMAGE_PER_DISEASE * diseases.Count * Time.deltaTime;
			overallHealth -= DAMAGE_PER_WOUND * wounds.Count * Time.deltaTime;
		}

		if(overallHealth <= 0) {
			overallHealth = 0.0f;
			oxygenLevel = 0.0f; 
			temperaturePercent = 0.0f;
			dead = true;
		}

		foreach(Wound wound in wounds) {
			wound.health -= WOUND_HEAL_PER_PLATELET * platelets.Count * Time.deltaTime;
		}
	}

	// Block clicked. Send selected WhiteBloodCell here
	void OnMouseOver() {
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

	void OnMouseExit() {
		showStats = false;
	}

	void OnGUI(){
		if (showStats) {
			Vector3 position = Camera.main.WorldToScreenPoint (StatsPoint.position);

			GUI.TextArea(new Rect(position.x,Screen.height-position.y,98,55), "Health:    " +(int)(overallHealth*100) + "\nOxygen:  " + oxygenLevel*100 + "%" + "\nDiseases:" + diseases.Count);
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

	IEnumerator FireMouseClick()
	{
		if(dead) {
			return false;
		}

		if (!destMarkPrefab.activeSelf) {
			destMarkPrefab.SetActive (true);
		}
		if (gameControl.isSelected) {
			GameObject mouseTarget = (GameObject)Instantiate (destMarkPrefab, (Vector2)mousePos, Quaternion.identity);
			Color c = Color.green;
			while (c.a > 0) {
				yield return new WaitForSeconds (.1f);
				c.a -= .05f;
				mouseTarget.renderer.material.color = c;
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
}
