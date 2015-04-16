using UnityEngine;
using System.Collections;

enum EventType { EVENT_TYPE_NONE, EVENT_TYPE_DISEASE, EVENT_TYPE_WOUND };

public class RandomEventManager : MonoBehaviour {
	public GameControl gameControl;
	public Body body;
	public GameObject diseasePrefab;
	public GameObject woundPrefab;
	public GameObject pingPrefab;
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
				if (gameControl.tutorial.currentState != TutorialStates.State.Off) {
					Debug.Log(" Game state is " + gameControl.tutorial.currentState);
					gameControl.isPause = true;
					GUI.Window(0, dialogRect, SpawnDiseaseDialog, "Infectious Disease!");
				}
			}
			break;
		case EventType.EVENT_TYPE_WOUND:
			if(!woundedWindowActivated) {
				if (gameControl.tutorial.currentState != TutorialStates.State.Off) {
					GUI.Window(0, dialogRect, SpawnWoundDialog, "Wounded!");
					gameControl.isPause = true;
				}
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

		yield return new WaitForSeconds(waitFor);
		StartCoroutine(RandomEventCycle());
	}


	public void SpawnWound() {
		Block randomBodyPart =  body.blocks[getRandomAliveBodyPartIndex()];
		SpawnWound (randomBodyPart);
	}

	public void SpawnWound (Block location){
		Vector3 spawnPoint = location.GetRandomPoint();
		StartCoroutine (PingLocation (spawnPoint));
		GameObject newWound = (GameObject)Instantiate (woundPrefab, spawnPoint, Quaternion.identity);
		Wound newWoundScript = newWound.GetComponent<Wound> ();
		newWoundScript.block = location;
		location.wounds.Add (newWound.GetComponent<Wound>());
		dialogOpen = EventType.EVENT_TYPE_WOUND;
		if(!woundedWindowActivated)
			gameControl.TogglePauseGame ();
	}

	public void SpawnDiseaseInfection() {
		Block randomBodyPart =  body.blocks[getRandomAliveBodyPartIndex()];
		SpawnDiseaseInfection (randomBodyPart, 3);
	}

	public void SpawnDiseaseInfection (Block location, int diseaseNumber){
		Vector3 spawnPoint = location.GetRandomPoint();
		StartCoroutine (PingLocation (spawnPoint));
		for(int i = 0; i<diseaseNumber; i++) {
			GameObject newDisease = (GameObject)Instantiate(diseasePrefab, spawnPoint, Quaternion.identity);
			Disease newDiseaseScript = newDisease.GetComponent<Disease>();
			newDiseaseScript.currentBlock = location;
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
		//Debug.Log (" Pause? " + gameControl.isPause);
			GUI.TextArea (new Rect (0, 20, 250, 100), "Ahhhh! An infectious bacteria has managed to get inside your body! Quick! Orchestrate the proper response of bodily functions to stop the infection before it spreads out of control!");
			gameControl.TogglePauseGame ();
			if (GUI.Button (new Rect (100, 125, 50, 20), "OK")) {
				dialogOpen = EventType.EVENT_TYPE_NONE;
				gameControl.TogglePauseGame ();
				diseaseWindowActivated = true;
				gameControl.isPause = false;
			}

	}

	void SpawnWoundDialog(int windowID) {
		//Debug.Log (" Pause? " + gameControl.isPause);
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ouch! You trip over a rock and injure yourself! Use platelets to clot the wound or you will bleed out!");
		gameControl.TogglePauseGame ();
		if (GUI.Button (new Rect (100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			gameControl.TogglePauseGame ();
			woundedWindowActivated = true;
			gameControl.isPause = false;
		}
	}

	IEnumerator PingLocation(Vector3 position) {
		float scaleFactor = 3f;
		GameObject ping = (GameObject)Instantiate (pingPrefab, position, Quaternion.identity);
		ping.transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1.0f) {
			float newScaleFactor = Mathf.Lerp(scaleFactor, 1f, t);
			ping.transform.localScale = new Vector3	(newScaleFactor, newScaleFactor, newScaleFactor);
			yield return null;
		}
		yield return new WaitForSeconds(2);
		float alpha = ping.renderer.material.color.a;
		for (float t = 0.0f; t < 2.0f; t += Time.deltaTime / 2.0f) {
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
			ping.renderer.material.color = newColor;
			yield return null;
		}
		Destroy(ping);
	}

	int getRandomAliveBodyPartIndex() {
		int randomBodyPartIndex = Random.Range (0, body.blocks.Count);

		while (body.blocks[randomBodyPartIndex].dead) {
			randomBodyPartIndex = Random.Range (0, body.blocks.Count);
		}

		return randomBodyPartIndex;
	}
}
