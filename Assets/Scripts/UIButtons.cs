﻿using UnityEngine;
using System.Collections;

public class UIButtons : UnityEngine.UI.Button {
//public class UIButtons : MonoBehaviour {
	public GameControl gameControl;
	public string cellType;
	public UnityEngine.UI.Button button;

	// Use this for initialization
	new void Start () {
		this.onClick.AddListener(() => {
			if (gameControl != null) {
				if (cellType == "RBC") {
					gameControl.toggleRBC = !gameControl.toggleRBC;
					gameControl.changed = false;
				}
				else if (cellType == "WBC") {
					gameControl.toggleWBC = !gameControl.toggleWBC;
					gameControl.wbcChanged = false;
				}
			}
		});
	}
	
	// Update is called once per frame
	void Update () {

	}
}