using UnityEngine;
using System.Collections;

// Much placeholder, but at least gives us a starting point
public class MapControl : MonoBehaviour {
	Rect level1, level2, level3;	

	// Use this for initialization
	void Start () {
		GameObject level1Obj = GameObject.Find ("Level 1");
		GameObject level2Obj = GameObject.Find ("Level 2");
		GameObject level3Obj = GameObject.Find ("Level 3");
		Vector3 level1ScreenPos = Camera.main.WorldToScreenPoint (level1Obj.transform.position);
		Vector3 level2ScreenPos = Camera.main.WorldToScreenPoint (level2Obj.transform.position);
		Vector3 level3ScreenPos = Camera.main.WorldToScreenPoint (level3Obj.transform.position);
		float level1Size = level1Obj.transform.localScale.magnitude * 20;
		float level2Size = level2Obj.transform.localScale.magnitude * 20;
		float level3Size = level3Obj.transform.localScale.magnitude * 20;
		level1 = new Rect (level1ScreenPos.x - level1Size / 2, Screen.height - level1ScreenPos.y - level1Size / 2, level1Size, level1Size);
		level2 = new Rect (level2ScreenPos.x - level2Size / 2, Screen.height - level2ScreenPos.y - level2Size / 2, level2Size, level2Size);
		level3 = new Rect (level3ScreenPos.x - level3Size / 2, Screen.height - level3ScreenPos.y - level3Size / 2, level3Size, level3Size);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {
		// Buttons are gross, need some nice graphics for this
		if (GUI.Button (level1, "Level 1")) {
			Application.LoadLevel("GameScene");
		}
		if (GUI.Button (level2, "Level 2")) {
			Debug.Log("LEVEL 2!!!!");
		}
		if (GUI.Button (level3, "Level 3")) {
			Debug.Log("LEVEL 3!!!!");
		}
	}
}
