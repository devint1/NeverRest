using UnityEngine;
using System.Collections;

public class MenuGUI : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	void OnGUI() {
		if(GUILayout.Button("Start Game"))
		{
			Application.LoadLevel ("GameScene"); 
		}
		if(GUILayout.Button("How To Play"))
		{

		}
		if(GUILayout.Button("Quit"))
		{
			Application.Quit();
		}
	}
}
