using UnityEngine;
using System.Collections;

public class GrowTextScript : MonoBehaviour {

	// Use this for initialization
	void Awake () {
		this.transform.localScale = new Vector3 (0f, 0f, 1f);
	}
	
	// Update is called once per frame
	void Update () {
		if (this.transform.localScale.x < 1.8f) {
			float increment = 1.8f * Time.deltaTime;
			this.transform.localScale = new Vector3 (this.transform.localScale.x + increment, this.transform.localScale.y + increment, 1f);
		}
	}
}
