using UnityEngine;
using System.Collections;

public class RandomEventManager : MonoBehaviour {
	public GameControl gameControl;
	public Body body;
	public GameObject diseasePrefab;

	const float MIN_RANDOM_EVENT_TIME = 10f;
	const float MAX_RANDOM_EVENT_TIME = 30f;

	int numDiseasesSpawn = 3;
	bool dialogOpen = false;
	Rect dialogRect = new Rect(750, 80, 250, 150);

	void Start () {
		SpawnDiseaseInfection ();

		StartCoroutine(RandomEventCycle());
	}
	
	void OnGUI() {
		if(dialogOpen) {
			GUI.Window(0, dialogRect, SpawnDiseaseDialog, "Infection Disease!");
		}
	}


	IEnumerator RandomEventCycle() {
		float waitFor = Random.Range (MIN_RANDOM_EVENT_TIME, MAX_RANDOM_EVENT_TIME);
		yield return new WaitForSeconds(waitFor);

		SpawnDiseaseInfection();
		// SpawnWound();   //To be added later and randomly choosen
		// SpawnVirus();
		// RacoonAttack();
		// FindSupplies();
		// BreakLeg();

		StartCoroutine(RandomEventCycle());
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
		dialogOpen = true;
		gameControl.TogglePauseGame();
	}

	void SpawnDiseaseDialog(int windowID) {
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ahhhh! An infectious bacteria has managed to get inside your body! Quick! Orchestrate the proper response of bodily functions to stop the infection before it spreads out of control!");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = false;
			gameControl.TogglePauseGame();
		}
	}
}
