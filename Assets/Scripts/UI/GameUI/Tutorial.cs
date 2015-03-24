using UnityEngine;
using System.Collections;
using TutorialStates;

namespace TutorialStates
{
	public enum State{ Off, Done, Commence, Pause, Selection, Move, Unpause, HeartRate, EnergyBar, Production, PlateProduction, WBCProduction, PlateCombat, WBCCombat, Finish };
}

public class Tutorial : MonoBehaviour {
	public GameControl gC;

	public TutorialStates.State currentState;

	bool bStartedDragCoroutine;

	float dragPercent;

	Rect box;

	EventType dialogOpen = EventType.EVENT_TYPE_NONE;
	Rect dialogRect = new Rect(750, 80, 250, 150);
	bool dialogWindowActivated = false;

	public bool StopGameLogic(){
		if (!(currentState == State.Done || currentState == State.Off)) {
			return true;
		}
		return false;
	}

	void Start(){
		currentState = TutorialStates.State.Commence;
		bStartedDragCoroutine = false;
		dragPercent = 0;
		StartCoroutine (DragBox());
	}

	void Update(){
	}
	void OnGUI(){
		switch (currentState) {
		case TutorialStates.State.Off:
		case TutorialStates.State.Done:
			break;
		case TutorialStates.State.Pause:
			DoPause();
			break;
		case TutorialStates.State.Selection:
			Texture2D text = new Texture2D (1, 1);
			Color col = Color.blue;
			col.a = .25f;
			text.SetPixel (1, 1, col);
			text.Apply ();
			GUI.DrawTexture (box, text);
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 100), "Multiple cells can be selected at a time by clicking and dragging around them.\nOnce selected, cells can be ordered to move by right clicking the desired location. \nSelect both of the cells in the chest now.");
			if (gC.selected.Count >= 2){
				//currentState = TutorialStates.State.Done;
				currentState = TutorialStates.State.Move;
			}
			break;
		case TutorialStates.State.Move:
			GUI.TextArea (new Rect (Screen.width/2 - 87, Screen.height/2 -50, 175, 50), "Right click on a part of the body to move the selected cells.");
			if(gC.firstMouse) {
				currentState = TutorialStates.State.Unpause;
			}
			break;
		case TutorialStates.State.Finish:
			GUI.Window(0, new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 150), FinishDialog, "Finished Tutorial");
			break;
		case TutorialStates.State.Unpause:
			GUI.TextArea (new Rect (Screen.width/2 - 87, Screen.height/2 -50, 175, 50), "Now unpause the game to continue.\n (space key)");
			if ( !gC.IsPaused() ) {
				currentState = TutorialStates.State.HeartRate;
			}
			break;
		case TutorialStates.State.HeartRate:
			GUI.Window(0, new Rect(175, 60, 250, 150), HeartRateDialog, "Heart Rate Slider");
			break;
		case TutorialStates.State.EnergyBar:
			GUI.Window(0, new Rect(250, 10, 250, 150), EnergyDialog, "Energy Bar");
			break;
		case TutorialStates.State.Production:
			GUI.Window(0, new Rect(175, Screen.height - 325, 250, 150), ProductionDialog, "Production");
			break;
		case TutorialStates.State.PlateProduction:
			GUI.Window(0, new Rect(220, Screen.height - 160, 250, 150), PlateProductionDialog, "Production");
			break;
		case TutorialStates.State.WBCProduction:
			GUI.Window(0, new Rect(300, Screen.height - 160, 250, 150), WBCProductionDialog, "Production");
			break;
		case TutorialStates.State.PlateCombat:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 150), PlateCombatDialog, "Combat");
			break;
		case TutorialStates.State.WBCCombat:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 150), WBCCombatDialog, "Combat");
			break;
		case TutorialStates.State.Commence:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 75), CommenceDialog, "Tutorial");
			break;
		}
	}

	void DoPause(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			currentState = TutorialStates.State.Selection;
		}
		GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 100), "The game can be paused at any time by pressing space.\nThis will alow you more time to make decisions and issue commands.\nDo this now.");
	}

	void DoSelection(){
		if (!bStartedDragCoroutine) {
			bStartedDragCoroutine = true;
			StartCoroutine (DragBox());
		}
	}

	IEnumerator DragBox(){
		float baseX = Screen.width * .44f;
		float baseY = Screen.height * .2f;
		float targetX = Screen.width * .53f;	
		float targetY = Screen.height * .37f;
		while (dragPercent < 100) {
			box = new Rect (baseX, baseY,
			                     (targetX - baseX) * dragPercent / 100, 
			                     (targetY - baseY) * dragPercent / 100);
			yield return new WaitForSeconds (1/30f);
			dragPercent += 3;
		}
		dragPercent = 0;
		yield return new WaitForSeconds (1f);
		StartCoroutine (DragBox());
	}

	void HeartRateDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "To increase the speed of the cells in the body, drag the 'Heart Beat' slider to the right.\nTry it out then press the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.EnergyBar;
		}
	}

	void FinishDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "Congratulations! You have completed the tutorial.\nPress the 'OK' button to play the game.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Done;
		}
	}

	void EnergyDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "The green bar represents the amount of energy available to use in production. It builds up over time, and is consumed during production.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Production;
		}
	}

	void ProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "The amount of cells you can produce at a time is limited by the amount of energy available and the multiplication factor based on the number of cells in production.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.PlateProduction;
		}
	}

	void PlateProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "Platelets are used to clot wounds as they appear.\nTo create new platelets, either press the platelet button or press the 'Q' key.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.WBCProduction;
		}
	}

	void WBCProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "B Cells, a type of white blood cell, are used to combat diseases as they enter the body.\nTo create new B Cells, either press the B Cell button or press the 'W' key.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.PlateCombat;
		}
	}

	void PlateCombatDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "To combat wounds, select a platelet and move the wound to the part of the body where the wound is located.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.WBCCombat;
		}
	}
	
	void WBCCombatDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "To combat diseases, move the B cells to the part of the body where the diseases are located.\nPress the 'OK' button to continue.");
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Finish;
		}
	}

	void CommenceDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 25), "Commence the tutorial?");
		if (GUI.Button(new Rect(150, 50, 50, 20), "Yes")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Pause;
		}
		if (GUI.Button(new Rect(50, 50, 50, 20), "No")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Done;
		}
	}
}