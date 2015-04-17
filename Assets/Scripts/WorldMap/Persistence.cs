using UnityEngine;
using System.Collections;

public class Persistence : MonoBehaviour {
	public int currentLevel = 1;

	public static Persistence i;
		
	void Awake () {
		if(!i) {
			i = this;
			DontDestroyOnLoad(gameObject);
		}
		else {
			Destroy(gameObject);
		}
	}
}
