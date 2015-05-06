using UnityEngine;
using System.Collections;

public class WinScreen : MonoBehaviour {
	public AudioSource winMusic;
	public Transform winText;

	// Use this for initialization
	void Start () {
		if (winMusic) {
			winMusic.Play();
		}

		winText.localScale = new Vector3 (0f, 0f, 1f);
		StartCoroutine (BackToMainMenu ());
	}
	
	// Update is called once per frame
	void Update () {
		if (winText.localScale.x < 1.0f) {
			float increment = 1.2f * Time.deltaTime;
			winText.localScale = new Vector3 (winText.localScale.x + increment, winText.localScale.y + increment, 1f);
		}
	}

	IEnumerator BackToMainMenu() {
		yield return new WaitForSeconds(34);
		Application.LoadLevel("MenuScene");
	}
}
