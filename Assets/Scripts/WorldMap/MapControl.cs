using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Much placeholder, but at least gives us a starting point
public class MapControl : MonoBehaviour {
	public List<GameObject> particlesByLevel = new List<GameObject>();
	Persistence p;

	// Use this for initialization
	void Start () {
		p = GameObject.FindGameObjectWithTag ("Persistence").GetComponent<Persistence>();
		for (int i = 0; i<p.currentLevel-2 && i<particlesByLevel.Count; i++) {
			DestroyImmediate(particlesByLevel[i]);
		}

		if (p.currentLevel-1 > 0) {
			turnOffParticlesInLevel(particlesByLevel[p.currentLevel-2]);
		}
	}
	
	void turnOffParticlesInLevel(GameObject levelContainer) {
		foreach (Transform child in levelContainer.transform)
		{
			child.gameObject.GetComponent<ParticleSystem>().loop = false;
		}
	}
}
