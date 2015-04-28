using UnityEngine;
using System.Collections;

public class ActionBarButton : MonoBehaviour {
	public enum ButtonType {WhiteBloodCellGreen, WhiteBloodCellPurple, WhiteBloodCellTeal, WhiteBloodCellFinder, Platelet}

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
		    || (Input.GetKeyDown (KeyCode.R) && buttonType == ButtonType.WhiteBloodCellTeal)
		    || (Input.GetKeyDown (KeyCode.T) && buttonType == ButtonType.WhiteBloodCellFinder)) {
			QueueWhiteBloodCell();
		}
		if (showTooltip) {
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(!hit || hit.collider.gameObject != this.gameObject) {
				showTooltip = false;
				GetComponent<Renderer>().material.color = Color.white;
			}
		}
	}

	void OnGUI(){
		if (showTooltip) {
			if (buttonType == ButtonType.Platelet)
				GUI.TextArea(new Rect(Input.mousePosition.x+10,Screen.height-(Input.mousePosition.y+65),170,68), "Platelet:\nHeals wounds and cuts\nHotkey: Q\nBase Cost: " + PLAT_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellGreen)
				GUI.TextArea(new Rect(Input.mousePosition.x+10,Screen.height-(Input.mousePosition.y+65),170,68), "White Blood Cell:\nNeutralizes Green Bacteria\nHotkey: W\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellPurple)
				GUI.TextArea(new Rect(Input.mousePosition.x+10,Screen.height-(Input.mousePosition.y+65),170,68), "White Blood Cell:\nNeutralizes Purple Bacteria\nHotkey: E\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellTeal)
				GUI.TextArea(new Rect(Input.mousePosition.x+10,Screen.height-(Input.mousePosition.y+65),170,68), "White Blood Cell:\nNeutralizes Teal Bacteria\nHotkey: R\nBase Cost: " + WBC_BASE_COST + " Energy");
			else if(buttonType == ButtonType.WhiteBloodCellFinder)
				GUI.TextArea(new Rect(Input.mousePosition.x+10,Screen.height-(Input.mousePosition.y+65),170,68), "Helper T Cell:\nIdentifies Unknown Bacteria\nHotkey: T\nBase Cost: " + WBC_BASE_COST + " Energy");
		}
	}

	void OnMouseDown() {
		if (buttonType == ButtonType.WhiteBloodCellGreen
		    || buttonType == ButtonType.WhiteBloodCellPurple
			|| buttonType == ButtonType.WhiteBloodCellTeal
		    || buttonType == ButtonType.WhiteBloodCellFinder) {
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

	void QueueWhiteBloodCell () {
		productionQueue.QueueItem (WBC_DEFAULT_BUILDTIME, WBC_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}

	void QueuePlatelet () {
		productionQueue.QueueItem (PLAT_DEFAULT_BUILDTIME, PLAT_BASE_COST, PRODUCTION_SPRITE, buttonType);
	}
}
