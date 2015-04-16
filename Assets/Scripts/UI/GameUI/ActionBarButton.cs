using UnityEngine;
using System.Collections;

public class ActionBarButton : MonoBehaviour {
	public enum ButtonType {WhiteBloodCell, Platelet}

	public ProductionQueue productionQueue;
	public ButtonType buttonType;

	private bool showTooltip = false;

	private const float WBC_BASE_COST = 10f;
	private const float PLAT_BASE_COST = 10f; 
	private const float WBC_DEFAULT_BUILDTIME = 15.0f;
	private const float PLAT_DEFAULT_BUILDTIME = 15.0f;

	public Sprite PRODUCTION_SPRITE;

	void Update() {
		if (Input.GetKeyDown (KeyCode.Q) && buttonType == ButtonType.WhiteBloodCell) {
			QueueWhiteBloodCell();
		}
		if (Input.GetKeyDown (KeyCode.W) && buttonType == ButtonType.Platelet) {
			QueuePlatelet();
		}
	}

	void OnGUI(){
		if (showTooltip) {
			if(buttonType == ButtonType.WhiteBloodCell)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create White Blood Cell\nHotkey: Q\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if (buttonType == ButtonType.Platelet)
				GUI.TextArea(new Rect(Input.mousePosition.x,Screen.height-(Input.mousePosition.y+55),150,50), "Create Platelet\nHotkey: W\nBase Cost: " + PLAT_BASE_COST + " Energy");
		}
	}

	void OnMouseDown() {
		if (buttonType == ButtonType.WhiteBloodCell) {
			QueueWhiteBloodCell();
		}
		else if (buttonType == ButtonType.Platelet) {
			QueuePlatelet();
		}
	}

	void OnMouseOver() {
		showTooltip = true;
		renderer.material.color = Color.yellow;
	}
	
	void OnMouseExit() {
		if (showTooltip) {
			showTooltip = false;
			renderer.material.color = Color.white;
		}
	}

	void QueueWhiteBloodCell () {
		productionQueue.QueueItem (WBC_DEFAULT_BUILDTIME, WBC_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}

	void QueuePlatelet () {
		productionQueue.QueueItem (PLAT_DEFAULT_BUILDTIME, PLAT_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}
}
