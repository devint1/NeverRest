using UnityEngine;
using System.Collections;

enum EventType { EVENT_TYPE_NONE, EVENT_TYPE_DISEASE, EVENT_TYPE_WOUND, EVENT_TYPE_RACOON };

public class RandomEventManager : MonoBehaviour {
	public GameControl gameControl;
	public Body body;
	public GameObject diseasePrefab;
	public GameObject woundPrefab;
	public GameObject pingPrefab;
	public GameObject raccoonPrefab;
	public bool isDisabled = true; //Needs to be enabled from start to prevent race conditions

	const float MIN_RANDOM_EVENT_TIME = 10f;
	const float MAX_RANDOM_EVENT_TIME = 30f;

	int numDiseasesSpawn = 3;
	EventType dialogOpen = EventType.EVENT_TYPE_NONE;
	Rect dialogRect = new Rect(750, 80, 250, 150);
	bool diseaseWindowActivated = false;
	bool woundedWindowActivated = false;
	bool randomEventCycleStarted = false;

	void Awake () {
		isDisabled = true;
	}

	void Update() {
		if (Input.GetKeyDown (KeyCode.F11)) {
			SpawnDiseaseInfection();
		}
		if(gameControl.persistence && !randomEventCycleStarted) {
			if(gameControl.persistence.currentLevel == 5) {
				RaccoonAttack();
			}
			StartCoroutine(RandomEventCycle());
			randomEventCycleStarted = true;
		}
	}
	
	void OnGUI() {
		switch(dialogOpen) {
		case EventType.EVENT_TYPE_DISEASE:
			if(!diseaseWindowActivated) {
				if (gameControl.tutorial.currentState != TutorialStates.State.Off) {
					Debug.Log(" Game state is " + gameControl.tutorial.currentState);
					//gameControl.isPause = true;
					GUI.Window(0, dialogRect, SpawnDiseaseDialog, "Infectious Disease!");
				}
			}
			break;
		case EventType.EVENT_TYPE_WOUND:
			if(!woundedWindowActivated) {
				if (gameControl.tutorial.currentState != TutorialStates.State.Off) {
					GUI.Window(0, dialogRect, SpawnWoundDialog, "Wounded!");
					//gameControl.isPause = true;
				}
			}
			break;
		}
	}


	IEnumerator RandomEventCycle() {
		float waitFor = Random.Range (MIN_RANDOM_EVENT_TIME, MAX_RANDOM_EVENT_TIME);
		EventType eventType = (EventType) Random.Range(1, 3);
		yield return new WaitForSeconds(waitFor);
		if (gameControl.persistence.currentLevel == 1 && eventType == EventType.EVENT_TYPE_DISEASE) {
			eventType = EventType.EVENT_TYPE_WOUND;
		}

		// Enable raccoon attack on level 4
		if (gameControl.persistence.currentLevel == 5) {
			eventType += Mathf.RoundToInt(Random.Range(0, 1));
		}

		if (!isDisabled) {
			switch(eventType) {
			case EventType.EVENT_TYPE_DISEASE:
				SpawnDiseaseInfection();
				break;
			case EventType.EVENT_TYPE_WOUND:
				SpawnWound();
				break;
			case EventType.EVENT_TYPE_RACOON:
				RaccoonAttack();
				break;
			}
			// SpawnVirus();
			// FindSupplies();
			// BreakLeg();
		}
		StartCoroutine(RandomEventCycle());
	}


	public void SpawnWound() {
		Block randomBodyPart =  body.blocks[getRandomAliveBodyPartIndex()];
		Vector3 spawnPoint = SpawnWound (randomBodyPart);
		if (gameControl.persistence.currentLevel > 1) {
			SpawnDiseaseInfection (randomBodyPart, Random.Range (3, 5), spawnPoint);
		}
	}

	public Vector3 SpawnWound (Block location){
		Vector3 spawnPoint = location.GetRandomPoint();
		StartCoroutine (PingLocation (spawnPoint));
		GameObject newWound = (GameObject)Instantiate (woundPrefab, spawnPoint, Quaternion.identity);
		Wound newWoundScript = newWound.GetComponent<Wound> ();
		newWoundScript.block = location;
		location.wounds.Add (newWound.GetComponent<Wound>());
		dialogOpen = EventType.EVENT_TYPE_WOUND;

		return spawnPoint;
	}

	public void SpawnDiseaseInfection() {
		Block randomBodyPart =  body.blocks[getRandomAliveBodyPartIndex()];
		SpawnDiseaseInfection (randomBodyPart, Random.Range(3, 5));
	}

	public void SpawnDiseaseInfection (Block location, int diseaseNumber){
		Vector3 spawnPoint = location.GetRandomPoint();
		int type;
		if(gameControl.persistence.currentLevel == 1) {
			type = 1;
		}
		else {
			type = Random.Range (1, 4);
		}

		StartCoroutine (PingLocation (spawnPoint));
		for(int i = 0; i<diseaseNumber; i++) {
			GameObject newDisease = (GameObject)Instantiate(diseasePrefab, spawnPoint, Quaternion.identity);
			Disease newDiseaseScript = newDisease.GetComponent<Disease>();
			newDiseaseScript.currentBlock = location;
			newDiseaseScript.gameControl = gameControl;
			newDiseaseScript.destination = spawnPoint;
			newDiseaseScript.type = (DiseaseType)type;

			++gameControl.numDiseaseCells;
		}
		
		numDiseasesSpawn += 3;
		dialogOpen = EventType.EVENT_TYPE_DISEASE;
	}

	void RaccoonAttack() {
		Block randomBodyPart =  body.blocks[getRandomAliveBodyPartIndex()];
		RaccoonAttack (randomBodyPart);
	}

	void RaccoonAttack (Block location) {
		Vector3 spawnPoint = location.GetRandomPoint();
		GameObject raccoon = (GameObject)Instantiate(raccoonPrefab, spawnPoint, Quaternion.identity);
		int numAttacks = Random.Range(1, 11);;
		StartCoroutine(AnimateRaccoon(raccoon, numAttacks));
	}

	IEnumerator AnimateRaccoon(GameObject raccoon, int numAttacks) {
		float time = 1f;
		for(int i = 0; i < numAttacks; ++i) {
			Block randomBodyPart = body.blocks[getRandomAliveBodyPartIndex()];
			Vector3 start = raccoon.transform.position;
			Vector3 destination = randomBodyPart.GetRandomPoint();
			for (float t = 0.0f; t < time; t += Time.deltaTime / time) {
				Vector3 newPosition = Vector3.Lerp(start, destination, t);
				raccoon.transform.position = newPosition;
				yield return null;
			}
			SpawnWound(randomBodyPart);
			SpawnDiseaseInfection(randomBodyPart, 5);
		}
		float alpha = raccoon.renderer.material.color.a;
		for (float t = 0.0f; t < time; t += Time.deltaTime / time) {
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
			raccoon.renderer.material.color = newColor;
			yield return null;
		}
		Destroy(raccoon);
	}

	public void SpawnDiseaseInfection (Block location, int diseaseNumber, Vector3 spawnPoint) {
		int type;
		if(gameControl.persistence.currentLevel == 1) {
			type = 1;
		}
		else {
			type = Random.Range (1, 4);
		}
		
		StartCoroutine (PingLocation (spawnPoint));
		for(int i = 0; i<diseaseNumber; i++) {
			GameObject newDisease = (GameObject)Instantiate(diseasePrefab, spawnPoint, Quaternion.identity);
			Disease newDiseaseScript = newDisease.GetComponent<Disease>();
			newDiseaseScript.currentBlock = location;
			newDiseaseScript.gameControl = gameControl;
			newDiseaseScript.destination = spawnPoint;
			newDiseaseScript.type = (DiseaseType)type;
			
			++gameControl.numDiseaseCells;
		}
		
		numDiseasesSpawn += 3;
		dialogOpen = EventType.EVENT_TYPE_DISEASE;
	}

	void SpawnDiseaseDialog(int windowID) {
		//Debug.Log (" Pause? " + gameControl.isPause);
			GUI.TextArea (new Rect (0, 20, 250, 100), "Ahhhh! An infectious bacteria has managed to get inside your body! Quick! Orchestrate the proper response of bodily functions to stop the infection before it spreads out of control!");
			//gameControl.TogglePauseGame ();
			if (GUI.Button (new Rect (100, 125, 50, 20), "OK")) {
				dialogOpen = EventType.EVENT_TYPE_NONE;
				//gameControl.TogglePauseGame ();
				diseaseWindowActivated = true;
				//gameControl.isPause = false;
			}

	}

	void SpawnWoundDialog(int windowID) {
		//Debug.Log (" Pause? " + gameControl.isPause);
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ouch! You trip over a rock and injure yourself! Use platelets to clot the wound or you will bleed out!");
		//gameControl.TogglePauseGame ();
		if (GUI.Button (new Rect (100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			//gameControl.TogglePauseGame ();
			woundedWindowActivated = true;
			//gameControl.isPause = false;
		}
	}

	IEnumerator PingLocation(Vector3 position) {
		if (gameControl.IsPaused ()) {
			gameControl.TogglePauseGame();
		}
		float scaleFactor = 3f;
		GameObject ping = (GameObject)Instantiate (pingPrefab, position, Quaternion.identity);
		ping.transform.localScale = new Vector3 (scaleFactor, scaleFactor, scaleFactor);
		for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1.0f) {
			float newScaleFactor = Mathf.Lerp(scaleFactor, 1f, t);
			ping.transform.localScale = new Vector3	(newScaleFactor, newScaleFactor, newScaleFactor);
			yield return null;
		}
		yield return new WaitForSeconds(2);
		float alpha = ping.GetComponent<Renderer>().material.color.a;
		for (float t = 0.0f; t < 2.0f; t += Time.deltaTime / 2.0f) {
			Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, 0, t));
			ping.GetComponent<Renderer>().material.color = newColor;
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
