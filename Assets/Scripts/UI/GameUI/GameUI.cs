﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
	
	public Text energyStatus;
	public GameObject WBCImage;
	public GameObject PCImage;
	public GameControl gC;
	public GameObject PauseImage;
	public Text WBCQueue;
	public Text PCQueue;
	public int maxQueueSize;

	static float WBC_DEFAULT_BUILDTIME = 5.0f;
	static float PLAT_DEFAULT_BUILDTIME = 5.0f;
	static float WBC_DEFAULT_COST = 100f;
	static float PLAT_DEFAULT_COST = 100f;
	static float DEFAULT_ENERGY_GAINED_PER_TICK = 1;
	static float DEFAULT_MAX_ENERGY = 2000f;
	static float TIME_SLICE = .125f;

	float currentEnergy;

	int numWBCBuilding;
	int numPCBuilding;
	int numWBCQueue;
	int numPCQueue;

	ArrayList WBCSlots;
	ArrayList PCSlots;

	// Use this for initialization
	void Start () {
		StartCoroutine(IncrementEnergy());
		currentEnergy = 100;
		energyStatus.color = Color.black;
		WBCSlots = new ArrayList ();
		PCSlots = new ArrayList ();

	}
	
	// Update is called once per frame
	void Update () {
		if (gC.IsPaused ()){
			PauseImage.SetActive (true);
		}
		else{
			PauseImage.SetActive (false);
		}

		if (numWBCBuilding < maxQueueSize && numWBCQueue > 0) {
			numWBCBuilding++;
			WBCQueue.text = "" + (--numWBCQueue);
			StartCoroutine(GenerateWBC());
		}
		
		if (numPCBuilding < maxQueueSize && numPCQueue > 0) {
			numPCBuilding++;
			PCQueue.text = "" + (--numPCQueue);
			StartCoroutine(GeneratePC());
		}

		energyStatus.text = (int)currentEnergy + "/" + DEFAULT_MAX_ENERGY;
		if (Input.GetKeyDown (KeyCode.Q)) {
			if(numWBCBuilding < maxQueueSize) {
				numWBCBuilding += 1;
				StartCoroutine(GenerateWBC());
			}
			else {
				WBCQueue.text = "" + (++numWBCQueue);
			}
		}
		else if (Input.GetKeyDown (KeyCode.W)) {
			if(numPCBuilding < maxQueueSize) {
				numPCBuilding += 1;
				StartCoroutine(GeneratePC());
			}
			else {
				PCQueue.text = "" + (++numPCQueue);
			}
		}
	}

	IEnumerator IncrementEnergy() {
		yield return new WaitForSeconds (.05f);
		if (currentEnergy < DEFAULT_MAX_ENERGY && !gC.IsPaused()) {
			currentEnergy += DEFAULT_ENERGY_GAINED_PER_TICK;
		}
		StartCoroutine (IncrementEnergy ());
	}

	IEnumerator GenerateWBC(){
		float cost,
		      costPerTick,
			  payed;

		double progress = 0;

		int ndx;

		float buildTime = WBC_DEFAULT_BUILDTIME;

		for (ndx = 0; ndx <= WBCSlots.Count; ndx++) {
			if (ndx == WBCSlots.Count){
				WBCSlots.Add(1);
				break;
			}
			else if (WBCSlots[ndx].Equals(0)){
				WBCSlots[ndx] = 1;
				break;
			}
		}

		GameObject pic = (GameObject) Instantiate (WBCImage);
		pic.transform.SetParent (transform, false);
		Vector3 pos = pic.transform.position;
		pos.y += Screen.height * ( .175f * (ndx + 1) );
		pic.transform.position = pos;

		cost = WBC_DEFAULT_COST - gC.GetUpgradeMenu ().GetComponent<UpgradeMenu> ().upgradeValues.WhiteCells.EneryPerUnit;
		buildTime = buildTime - .25f * gC.GetUpgradeMenu ().GetComponent<UpgradeMenu> ().upgradeValues.WhiteCells.BuildSpeed;
		costPerTick = cost / (buildTime / TIME_SLICE);
		payed = 0;

		while (progress != 100) {
			Color c = pic.GetComponent<Image>().color;
			c.a = (float) progress * .01f;
			pic.GetComponent<Image>().color = c;
			if( currentEnergy > costPerTick ){
				currentEnergy -= costPerTick;
				payed += costPerTick;
				progress = Mathf.CeilToInt(payed/cost * 100);
			}
			yield return new WaitForSeconds (TIME_SLICE);
		}
		Destroy (pic);
		numWBCBuilding -= 1;
		WBCSlots [ndx] = 0;
		gC.SpawnWhiteBloodCell();
	}

	IEnumerator GeneratePC(){
		float cost,
				costPerTick,
				payed;
		
		double progress = 0;
		
		int ndx;
		
		float buildTime = PLAT_DEFAULT_BUILDTIME;

		for (ndx = 0; ndx <= PCSlots.Count; ndx++) {
			if (ndx == PCSlots.Count){
				PCSlots.Add(1);
				break;
			}
			else if (PCSlots[ndx].Equals(0)){
				PCSlots[ndx] = 1;
				break;
			}
		}
		
		GameObject pic = (GameObject) Instantiate (PCImage);
		pic.transform.SetParent (transform, false);
		Vector3 pos = pic.transform.position;
		pos.y += Screen.height * ( .175f * (ndx + 1) );
		pic.transform.position = pos;
		
		cost = PLAT_DEFAULT_COST - gC.GetUpgradeMenu ().GetComponent<UpgradeMenu> ().upgradeValues.PlateletCells.EneryPerUnit;
		buildTime = buildTime - .25f * gC.GetUpgradeMenu ().GetComponent<UpgradeMenu> ().upgradeValues.PlateletCells.BuildSpeed;
		costPerTick = cost / (buildTime / TIME_SLICE);
		payed = 0;
		
		while (progress != 100) {
			Color c = pic.GetComponent<Image>().color;
			c.a = (float) progress * .01f;
			pic.GetComponent<Image>().color = c;
			if( currentEnergy > costPerTick ){
				currentEnergy -= costPerTick;
				payed += costPerTick;
				progress = Mathf.CeilToInt(payed/cost *100);
			}
			yield return new WaitForSeconds (TIME_SLICE);
		}
		Destroy (pic);
		numPCBuilding -= 1;
		PCSlots [ndx] = 0;
		gC.SpawnPlatelet();
	}

	public void SetPauseAlphaHigh (Image i){
		i.CrossFadeAlpha (.5f, .5f, true);
	}

	public void SetPauseAlphaLow (Image i){
		i.CrossFadeAlpha (.05f, .5f, true);
	}

	public void incrWBCQueue() {
		WBCQueue.text = "" + (++numWBCQueue);
	}
	public void decrWBCQueue() {
		if(numWBCQueue > 0)
			WBCQueue.text = "" + (--numWBCQueue);
	}

	public void incrPCQueue() {
		PCQueue.text = "" + (++numPCQueue);
	}
	public void decrPCQueue() {
		if(numPCQueue > 0)
			PCQueue.text = "" + (--numPCQueue);
	}
}
