using UnityEngine;
using System.Collections;

public class UIButtons : UnityEngine.UI.Button {
//public class UIButtons : MonoBehaviour {
	public GameControl gameControl;
	public string cellType;
	public UnityEngine.UI.Button button;

	bool change = false;

	// Use this for initialization
	void Start () {
		//if (button != null) {
			this.onClick.AddListener(() => {
				if (gameControl != null) {
					if (cellType == "Plate") {
						gameControl.togglePT = !gameControl.togglePT;
					}
					else if (cellType == "RBC") {
						gameControl.toggleRBC = !gameControl.toggleRBC;
						gameControl.changed = false;
					}
					else if (cellType == "WBC") {
						gameControl.toggleWBC = !gameControl.toggleWBC;
						gameControl.wbcChanged = false;
					}
				}
				change = true;
			});
		//}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameControl != null) {
			if (gameControl.toggleWBC && change) {
				//this.renderer.material.color = Color.blue;
				//this.image.renderer.material.color = Color.blue;
				GUI.color = Color.blue;
			}
			else if (!gameControl.toggleWBC && change) {
				//this.image.renderer.material.color = Color.gray;
				GUI.color = Color.gray;
			}
		}
	}
}
