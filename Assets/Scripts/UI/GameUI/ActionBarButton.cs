﻿using UnityEngine;
using System.Collections;

public class ActionBarButton : MonoBehaviour {
	public enum ButtonType {WhiteBloodCellGreen, WhiteBloodCellPurple, WhiteBloodCellTeal, Platelet}

	public ProductionQueue productionQueue;
	public ButtonType buttonType;

	private bool showTooltip = false;

	private const float WBC_BASE_COST = 10f;
	private const float PLAT_BASE_COST = 10f; 
	private const float WBC_DEFAULT_BUILDTIME = 15.0f;
	private const float PLAT_DEFAULT_BUILDTIME = 15.0f;

	public Sprite PRODUCTION_SPRITE;

	void Update() {
		if (Input.GetKeyDown (KeyCode.Q) && buttonType == ButtonType.Platelet) {
			QueuePlatelet();
		}
		if ((Input.GetKeyDown (KeyCode.W) && buttonType == ButtonType.WhiteBloodCellGreen)
		    || (Input.GetKeyDown (KeyCode.E) && buttonType == ButtonType.WhiteBloodCellPurple)
		    || (Input.GetKeyDown (KeyCode.R) && buttonType == ButtonType.WhiteBloodCellTeal)) {
			QueueWhiteBloodCell();
		}
	}

	void OnGUI(){
		if (showTooltip) {
			if (buttonType == ButtonType.Platelet)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create Platelet\nHotkey: Q\nBase Cost: " + PLAT_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellGreen)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create Green White Blood Cell\nHotkey: W\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellPurple)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create Purple White Blood Cell\nHotkey: E\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellTeal)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create Teal White Blood Cell\nHotkey: R\nBase Cost: " + WBC_BASE_COST + " Energy");
		}
	}

	void OnMouseDown() {
		if (buttonType == ButtonType.WhiteBloodCellGreen
		    || buttonType == ButtonType.WhiteBloodCellPurple
			|| buttonType == ButtonType.WhiteBloodCellTeal) {
			QueueWhiteBloodCell();
		}
		else if (buttonType == ButtonType.Platelet) {
			QueuePlatelet();
		}
	}

	void OnMouseOver() {
		showTooltip = true;
		GetComponent<Renderer>().material.color = Color.yellow;
	}
	
	void OnMouseExit() {
		if (showTooltip) {
			showTooltip = false;
			GetComponent<Renderer>().material.color = Color.white;
		}
	}

	void QueueWhiteBloodCell () {
		productionQueue.QueueItem (WBC_DEFAULT_BUILDTIME, WBC_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}

	void QueuePlatelet () {
		productionQueue.QueueItem (PLAT_DEFAULT_BUILDTIME, PLAT_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}
}
