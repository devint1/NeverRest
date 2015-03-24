using UnityEngine;
using System.Collections;

public class EventControl : MonoBehaviour {
	public Vector2 pointPos;
	public Map map;
	//public class WinCon{};
	public enum PointType { POINT_TYPE_NONE, POINT_TYPE_EXIT, POINT_TYPE_SHOP, POINT_TYPE_DISEASE, POINT_TYPE_PEACE }
	Rect dialogRect = new Rect(750, 80, 250, 150);
	public PointType dialogOpen = PointType.POINT_TYPE_NONE;


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
	void OnGUI() {
		switch(dialogOpen) {
		case PointType.POINT_TYPE_EXIT:

			break;

		case PointType.POINT_TYPE_DISEASE:

			//GUI.Window(0, dialogRect, SpawnDiseaseDialog, "Infection Disease!");
			break;
		case PointType.POINT_TYPE_PEACE:
			//GUI.Window(0, dialogRect, SpawnPeaceDialog, "All is Calm!");

				;
			break;
		case PointType.POINT_TYPE_SHOP:

				GUI.Window(0, dialogRect, SpawnShopDialog, "Shop!");
				//Debug.Log("Open Shop");
			break;
		}
	}
	void SpawnShopDialog(int windowID){
		dialogRect =new Rect(750, 80, 400, 400);
		//GUI.TextArea (new Rect (0, 20, 250, 100), "Buy Buy Buy");

		//GUI.Box(new Rect(10, 20, 100, 90), "Inventory");

		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = PointType.POINT_TYPE_NONE; 
		}
	}

	void SpawnDiseaseDialog(int windowID) {
		GUI.TextArea (new Rect (0, 20, 250, 100), "Ahhhh! An infectious bacteria has managed to get inside your body! Quick! Orchestrate the proper response of bodily functions to stop the infection before it spreads out of control!");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			//dialogOpen = PointType.POINT_TYPE_NONE; 
		}
	}
	void SpawnPeaceDialog(int windowID) {
		GUI.TextArea (new Rect (0, 20, 250, 100), "Peaceful nothing happens");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			//dialogOpen = PointType.POINT_TYPE_NONE; 
		}
	}
}
