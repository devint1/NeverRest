using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Much placeholder, but at least gives us a starting point
public class MapControl : MonoBehaviour {
	public List<GameObject> particlesByLevel = new List<GameObject>();
	public List<GameObject> bootprintsByLevel = new List<GameObject>();
	Persistence p;

	public AudioSource footstepAudio;

	// Use this for initialization
	void Start () {
		p = GameObject.FindGameObjectWithTag ("Persistence").GetComponent<Persistence>();
		for (int i = 0; i<p.currentLevel-2 && i<particlesByLevel.Count; i++) {
			showBootprintsInLevel(bootprintsByLevel[i]);
			DestroyImmediate(particlesByLevel[i]);
		}

		if (p.currentLevel-1 > 0) {
			if(p.justWon) {
				turnOffParticlesInLevel(particlesByLevel[p.currentLevel-2]);
				slowlyShowBootPrintInLevel(bootprintsByLevel[p.currentLevel-2]);
			}
			else {
				DestroyImmediate(particlesByLevel[p.currentLevel-2]);
				showBootprintsInLevel(bootprintsByLevel[p.currentLevel-2]);
			}
		}
	}
	
	void turnOffParticlesInLevel(GameObject levelContainer) {
		foreach (Transform child in levelContainer.transform)
		{
			child.gameObject.GetComponent<ParticleSystem>().loop = false;
		}
	}

	void showBootprintsInLevel(GameObject bootContainer) {
		foreach (Transform child in bootContainer.transform)
		{
			child.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
		}
	}

	void slowlyShowBootPrintInLevel(GameObject bootContainer) {
		float i = ((p.currentLevel-2) * 2f) + 9f;
		foreach (Transform child in bootContainer.transform)
		{
			i += 1f;
			StartCoroutine (waitAndShowBoot ( i*0.2f, child.gameObject.GetComponent<SpriteRenderer>() ) );
		}
	}

	IEnumerator waitAndShowBoot(float wait, SpriteRenderer boot) {
		yield return new WaitForSeconds(wait);
		boot.color = Color.white;
		footstepAudio.Play ();
	}
}
