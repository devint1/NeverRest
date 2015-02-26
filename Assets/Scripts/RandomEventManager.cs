using UnityEngine;
using System.Collections;

public class RandomEventManager : MonoBehaviour {
	public GameControl gameControl;
	public Body body;
	public GameObject diseasePrefab;

	const float MIN_RANDOM_EVENT_TIME = 10f;
	const float MAX_RANDOM_EVENT_TIME = 30f;

	int numDiseasesSpawn = 3;

	void Start () {
		SpawnDiseaseInfection ();

		StartCoroutine(RandomEventCycle());
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
		//gameControl.TogglePauseGame ();

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
	}
}
