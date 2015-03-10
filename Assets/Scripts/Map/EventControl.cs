using UnityEngine;
using System.Collections;

public class EventControl : MonoBehaviour {
	public PointControl point;
	//public class WinCon{};
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	

	}
	public static void WinCon(){

		GameObject winTextObj = new GameObject("WinText");
		winTextObj.transform.position = new Vector3(0.465f, 0.561f, 1f);
		GUIText winText = (GUIText)winTextObj.AddComponent(typeof(GUIText));
		winText.text = "You finished this part of the mountain!!!";
		winText.anchor = TextAnchor.MiddleCenter;
		winText.alignment = TextAlignment.Center;
		winText.fontSize = 100;
	}
}
