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

	GUIStyle tutorialMessageStyle;

	EventType dialogOpen = EventType.EVENT_TYPE_NONE;
	Rect dialogRect = new Rect(750, 80, 250, 150);
	bool dialogWindowActivated = false;

	int counter = 0;

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
		tutorialMessageStyle = GUIStyle.none;
		Texture2D text = new Texture2D(100,100);
		for (int x = 0; x < 100; x++) {
			for (int y = 0; y < 100; y++) {
				if( x == 0 || x == 99 || y == 0 || y == 99 ){
					text.SetPixel( x,y, Color.white );
				}
				else
				{
					text.SetPixel( x,y, Color.blue );
				}
			}
		}
		text.Apply ();
		tutorialMessageStyle.normal.background = text;
		tutorialMessageStyle.normal.textColor = Color.yellow;
		tutorialMessageStyle.wordWrap = true;
		tutorialMessageStyle.alignment = TextAnchor.MiddleCenter;
		tutorialMessageStyle.border = new RectOffset( 0, 1, 0, 1);
		Font arialFont = (Font)Resources.GetBuiltinResource (typeof(Font), "Arial.ttf");
		tutorialMessageStyle.font = arialFont;
		tutorialMessageStyle.fontSize = 14;
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
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 275, 150), "Multiple cells can be selected at a time by clicking and dragging around them.\nOnce selected, cells can be ordered to move by right clicking the desired location. \nSelect both of the cells in the chest now.", tutorialMessageStyle);
			if (gC.selected.Count >= 2){
				//currentState = TutorialStates.State.Done;
				currentState = TutorialStates.State.Move;
			}
			break;
		case TutorialStates.State.Move:
			GUI.TextArea (new Rect (Screen.width/2 - 87, Screen.height/2 -50, 175, 50), "Right click on a part of the body to move the selected cells.", tutorialMessageStyle);
			if(gC.firstMouse) {
				currentState = TutorialStates.State.Unpause;
			}
			break;
		case TutorialStates.State.Finish:
			GUI.Window(0, new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 150), FinishDialog, "Finished Tutorial", tutorialMessageStyle);
			break;
		case TutorialStates.State.Unpause:
			GUI.TextArea (new Rect (Screen.width/2 - 87, Screen.height/2 -50, 175, 50), "Now unpause the game to continue.\n (space key)", tutorialMessageStyle);
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
			GUI.Window(0, new Rect(200, Screen.height - 325, 250, 150), ProductionDialog, "Production");
			break;
		case TutorialStates.State.PlateProduction:
			//GUI.Window(0, new Rect(220, Screen.height - 160, 250, 150), PlateProductionDialog, "Production");
			//gC.actionBarPrefab.GetComponentInChildren<ActionBarButton>();
			ActionBarButton ab = gC.actionBarPrefab.transform.Find("platelet_Button").GetComponent<ActionBarButton>();
			if(counter < 30) {
				ab.renderer.material.color = Color.yellow;
			}
			else if(counter < 60) {
				ab.renderer.material.color = Color.white;
			}
			else {
				counter = 0;
			}

			//SetAlphaOfBody(0.3f);

			counter++;
			GUI.TextArea (new Rect (125, Screen.height - 200, 250, 150), "Platelets are used to clot wounds as they appear.\nTo create new platelets, either press the platelet button or press the 'W' key.\nCreate a platelet now.", tutorialMessageStyle);
			if (gC.plateletProduction > 0){
				//currentState = TutorialStates.State.Done;
				//SetAlphaOfBody(1.0f);

				ab.renderer.material.color = Color.white;
				currentState = TutorialStates.State.WBCProduction;
				counter = 0;
			}
			break;
		case TutorialStates.State.WBCProduction:
			//GUI.Window(0, new Rect(300, Screen.height - 160, 250, 150), WBCProductionDialog, "Production");
			GUI.TextArea (new Rect (125, Screen.height - 290, 250, 150), "B Cells, a type of white blood cell, are used to combat diseases as they enter the body.\nTo create new B Cells, either press the B Cell button or press the 'Q' key.\n\nCreate a B-Cell now.", tutorialMessageStyle);

			ActionBarButton wb = gC.actionBarPrefab.transform.Find("whitebloodcell_Button").GetComponent<ActionBarButton>();
			if(counter < 30) {
				wb.renderer.material.color = Color.yellow;
			}
			else if(counter < 60) {
				wb.renderer.material.color = Color.white;
			}
			else {
				counter = 0;
			}
			counter++;
			if (gC.whiteBloodProduction > 0){
				//currentState = TutorialStates.State.Done;
				currentState = TutorialStates.State.PlateCombat;
				wb.renderer.material.color = Color.white;
				counter = 0;
			}
			break;
		case TutorialStates.State.PlateCombat:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 150), PlateCombatDialog, "Combat", tutorialMessageStyle);
			break;
		case TutorialStates.State.WBCCombat:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 150), WBCCombatDialog, "Combat", tutorialMessageStyle);
			break;
		case TutorialStates.State.Commence:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 75), CommenceDialog, "Tutorial", tutorialMessageStyle);
			break;
		}
	}

	//In case we want to fade out items that are not what the focus should be on
	void SetAlphaOfBody(float a) {
		Color color1 = gC.body.toplayer.renderer.material.color;
		color1.a = a;
		gC.body.toplayer.renderer.material.color = color1;
		Color color2 = gC.body.middlelayer.renderer.material.color;
		color2.a = a;
		gC.body.middlelayer.renderer.material.color = color2;
		for(int i = 0; i < gC.body.blocks.Count; i++) {
			Color colorc = gC.body.blocks[i].renderer.material.color;
			colorc.a = a;
			gC.body.blocks[i].renderer.material.color = colorc;
		}
	}

	void DoPause(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			currentState = TutorialStates.State.Selection;
		}
		GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 100), "The game can be paused at any time by pressing space.\nThis will alow you more time to make decisions and issue commands.\nDo this now.", tutorialMessageStyle);
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
		GUI.TextArea (new Rect (0, 20, 250, 100), "To increase the speed of the cells in the body, drag the 'Heart Rate' slider to the right.\nTry it out then press the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.EnergyBar;
		}
	}

	void FinishDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "Congratulations! You have completed the tutorial.\nPress the 'OK' button to play the game.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Done;
		}
	}

	void EnergyDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "The green bar represents the amount of energy available to use in production. It builds up over time, and is consumed during production.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Production;
		}
	}

	void ProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "The amount of cells you can produce at a time is limited by the amount of energy available and the multiplication factor based on the number of cells in production.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.PlateProduction;
		}
	}

	void PlateProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "Platelets are used to clot wounds as they appear.\nTo create new platelets, either press the platelet button or press the 'Q' key.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.WBCProduction;
		}
	}

	void WBCProductionDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "B Cells, a type of white blood cell, are used to combat diseases as they enter the body.\nTo create new B Cells, either press the B Cell button or press the 'W' key.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.PlateCombat;
		}
	}

	void PlateCombatDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "To combat wounds, select a platelet and move the wound to the part of the body where the wound is located.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.WBCCombat;
		}
	}
	
	void WBCCombatDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 100), "To combat diseases, move the B cells to the part of the body where the diseases are located.\nPress the 'OK' button to continue.", tutorialMessageStyle);
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			dialogOpen = EventType.EVENT_TYPE_NONE;
			dialogWindowActivated = false;
			currentState = TutorialStates.State.Finish;
		}
	}

	void CommenceDialog(int windowID) {
		dialogWindowActivated = true;
		GUI.TextArea (new Rect (0, 20, 250, 25), "Commence the tutorial?", tutorialMessageStyle);
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