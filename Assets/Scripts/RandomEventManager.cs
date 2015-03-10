using UnityEngine;
using System.Collections;

enum EventType { EVENT_TYPE_NONE, EVENT_TYPE_DISEASE, EVENT_TYPE_WOUND };

public class RandomEventManager : MonoBehaviour {
	public GameControl gameControl;
	public Body body;
	public GameObject diseasePrefab;
	public GameObject woundPrefab;
	public bool isDisabled = true; //Needs to be enabled from start to prevent race conditions

	const float MIN_RANDOM_EVENT_TIME = 10f;
	const float MAX_RANDOM_EVENT_TIME = 30f;

	int numDiseasesSpawn = 3;
	EventType dialogOpen = EventType.EVENT_TYPE_NONE;
	Rect dialogRect = new Rect(750, 80, 250, 150);
	bool diseaseWindowActivated = false;
	bool woundedWindowActivated = false;

	void Start () {
		isDisabled = true;
		StartCoroutine(RandomEventCycle());
	}
	
	void OnGUI() {
		switch(dialogOpen) {
		case EventType.EVENT_TYPE_DISEASE:
			if(!diseaseWindowActivated) {
				GUI.Window(0, dialogRect, SpawnDiseaseDialog, "Infectious Disease!");
			}
			break;
		case EventType.EVENT_TYPE_WOUND:
			if(!woundedWindowActivated) {
				GUI.Window(0, dialogRect, SpawnWoundDialog, "Wounded!");
			}
			break;
		}
	}


	IEnumerator RandomEventCycle() {
		float waitFor = Random.Range (MIN_RANDOM_EVENT_TIME, MAX_RANDOM_EVENT_TIME);
		EventType eventType = (EventType) Mathf.RoundToInt(Random.Range(1, 3));

		if (!isDisabled) {
			switch(eventType) {
			case EventType.EVENT_TYPE_DISEASE:
				SpawnDiseaseInfection();
				break;
			case EventType.EVENT_TYPE_WOUND:
				SpawnWound();
				break;
			}
			// SpawnVirus();
			// RacoonAttack();
			// FindSupplies();
			// BreakLeg();
		}
		else{
			waitFor = 0f;
		}
		yield return new WaitForSeconds(waitFor);
		StartCoroutine(RandomEventCycle());
	}


	void SpawnWound() {
		Block randomBodyPart =  body.blocks[Random.Range (0, body.blocks.Count)];
		Vector3 spawnPoint = randomBodyPart.GetRandomPoint();
		GameObject newWound = (GameObject)Instantiate (woundPrefab, spawnPoint, Quaternion.identity);
		Wound newWoundScript = newWound.GetComponent<Wound> ();
		newWoundScript.block = randomBodyPart;
		randomBodyPart.wounds.Add (newWound.GetComponent<Wound>());
		dialogOpen = EventType.EVENT_TYPE_WOUND;
		if(!woundedWindowActivated)
			gameControl.TogglePauseGame ();
	}

	void SpawnDiseaseInfection() {
		int randomBodyPart = Random.Range (0, body.blocks.Count);
		Vector3 spawnPoint = body.blocks[randomBodyPart].GetRandomPoint();

		for(int i = 0; i<numDiseasesSpawn; i++) {
			GameObject newDisease = (GameObject)Instantiate(diseasePrefab, spawnPoint, Quaternion.identity);
			Disease newDiseaseScript = newDisease.GetComponent<Disease>();
			newDiseaseScript.currentBlock = body.blocks[randomBodyPart];
			newDiseaseScript.gameControl = gameControl;
			newDiseaseScript.destination = spawnPoint;
			++gameControl.numDiseaseCells;
		}

		numDiseasesSpawn += 3;
		dialogOpen = EventType.EVENT_TYPE_DISEASE;
		if(!diseaseWindowActivated)
			gameControl.TogglePauseGame();
	}

	void SpawnDiseaseDialog(int windowID) {
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ahhhh! An infectious bacteria has managed to get inside your body! Quick! Orchestrate the proper response of bodily functions to stop the infection before it spreads out of control!");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			gameControl.TogglePauseGame();
			diseaseWindowActivated = true;
		}
	}

	void SpawnWoundDialog(int windowID) {
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ouch! You trip over a rock and injure yourself! Use platelets to clot the wound or you will bleed out!");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			gameControl.TogglePauseGame();
			woundedWindowActivated = true;
		}
	}
}
