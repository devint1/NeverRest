using UnityEngine;
using System.Collections;
using TutorialStates;

namespace TutorialStates
{
	public enum State{ Off, Done, Commence, Pause, Selection, Move, Unpause, PlateProduction, DiseaseBasicSpawn, WBCProduction, Finish, EnemySpawn, WaitForLevelTwo, WaitForLevelThree };
}
public class Tutorial : MonoBehaviour {
	public GameControl gC;
	
	public TutorialStates.State currentState;
	bool bStartedDragCoroutine;
	
	float dragPercent;
	
	Rect box;
	
	GUIStyle tutorialMessageStyle;
	
	int counter = 0;
	
	public bool StopGameLogic(){
		if (!(currentState == State.Done || currentState == State.Off || currentState == State.WaitForLevelTwo || currentState == State.WaitForLevelThree)) {
			return true;
		}
		return false;
	}
	
	void Start(){
		if (gC.persistence.currentLevel == 1) {
			currentState = TutorialStates.State.Commence;
		}
		else {
			currentState = TutorialStates.State.Off;
		}
		
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
		if (currentState == State.WaitForLevelTwo && gC.persistence.currentLevel == 2) {
			currentState = State.DiseaseBasicSpawn;
		} else if (currentState == State.WaitForLevelThree && gC.persistence.currentLevel == 3) {
		}
	}
	void OnGUI(){
		switch (currentState) {
		case TutorialStates.State.Off:
			break;
		case TutorialStates.State.Done:
			break;
		case TutorialStates.State.Pause:
			DoPause();
			break;
		case TutorialStates.State.Selection:
			if( gC.plateletProduction > 0 ){
				return;
			}
			if( !gC.IsPaused() ){
				gC.TogglePauseGame();
			}
			Texture2D text = new Texture2D (1, 1);
			Color col = Color.blue;
			col.a = .25f;
			text.SetPixel (1, 1, col);
			text.Apply ();
			GUI.DrawTexture (box, text);
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 275, 150), "Multiple cells can be selected at a time by clicking and dragging around them.\nSelect the three platelet cells in the chest now.", tutorialMessageStyle);
			if (gC.selected.Count >= 2){
				currentState = TutorialStates.State.Move;
			}
			break;
		case TutorialStates.State.Move:
			GUI.TextArea (new Rect (Screen.width/2.4f, Screen.height/1.8f, Screen.width/6f, Screen.height/6f), "Right click on a part of the body to move the selected cells. Right click the stomach to send the platelet cells there.", tutorialMessageStyle);
			if(gC.firstMouse) {
				currentState = TutorialStates.State.Unpause;
			}
			break;
		case TutorialStates.State.Finish:
			GUI.Window(0, new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 150), FinishDialog, "Finished Tutorial", tutorialMessageStyle);
			break;
		case TutorialStates.State.Unpause:
			GUI.TextArea (new Rect (Screen.width/2.4f, Screen.height/1.8f, Screen.width/6f, Screen.height/6f), "Now unpause the game to continue.\n (space key)", tutorialMessageStyle);
			if ( !gC.IsPaused() ) {
				currentState = TutorialStates.State.WaitForLevelTwo;
			}
			break;
		case TutorialStates.State.PlateProduction:;
			ActionBarButton ab = gC.actionBarPrefab.transform.Find("platelet_Button").GetComponent<ActionBarButton>();
			if(counter < 30) {
				ab.GetComponent<Renderer>().material.color = Color.yellow;
			}
			else if(counter < 60) {
				ab.GetComponent<Renderer>().material.color = Color.white;
			}
			else {
				counter = 0;
			}
			counter++;
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2, 250, 200), "You have a wound on your stomach! The first step to fixing this is creating platelet cells. A platelet cell can be made by clicking the flashing icon in the bottom left or by hitting Q. Notice that each one created costs energy (which is found in the upper left hand corner). This bar will slowly refill over time. Create three platelet cells by hitting Q three times.", tutorialMessageStyle);
			if (gC.plateletProduction > 2){
				ab.GetComponent<Renderer>().material.color = Color.white;
				currentState = TutorialStates.State.Pause;
				counter = 0;
			}
			break;
		case TutorialStates.State.WBCProduction:
			GUI.TextArea (new Rect (125, Screen.height - 290, 250, 150), "B Cells, a type of white blood cell, are used to combat diseases as they enter the body.\nTo create new B Cells, either press the B Cell button or press the 'Q' key.\n\nCreate a B-Cell now.", tutorialMessageStyle);
			ActionBarButton wb = gC.actionBarPrefab.transform.Find("whitebloodcell_Button").GetComponent<ActionBarButton>();
			if(counter < 30) {
				wb.GetComponent<Renderer>().material.color = Color.yellow;
			}
			else if(counter < 60) {
				wb.GetComponent<Renderer>().material.color = Color.white;
			}
			else {
				counter = 0;
			}
			counter++;
			if (gC.whiteBloodProduction > 0){
				currentState = TutorialStates.State.WaitForLevelThree;
				wb.GetComponent<Renderer>().material.color = Color.white;
				counter = 0;
			}
			break;
		case TutorialStates.State.Commence:
			GUI.Window(0, new Rect(Screen.width/2 - 125, Screen.height/2 -50, 250, 75), CommenceDialog, "Tutorial", tutorialMessageStyle);
			break;
		case TutorialStates.State.DiseaseBasicSpawn:
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2, 250, 200), "You have diseases in your stomach!", tutorialMessageStyle);
			break;
		}
	}
	
	//In case we want to fade out items that are not what the focus should be on
	void SetAlphaOfBody(float a) {
		Color color1 = gC.body.toplayer.GetComponent<Renderer>().material.color;
		color1.a = a;
		gC.body.toplayer.GetComponent<Renderer>().material.color = color1;
		Color color2 = gC.body.middlelayer.GetComponent<Renderer>().material.color;
		color2.a = a;
		gC.body.middlelayer.GetComponent<Renderer>().material.color = color2;
		for(int i = 0; i < gC.body.blocks.Count; i++) {
			Color colorc = gC.body.blocks[i].GetComponent<Renderer>().material.color;
			colorc.a = a;
			gC.body.blocks[i].GetComponent<Renderer>().material.color = colorc;
		}
	}
	
	void DoPause(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			currentState = TutorialStates.State.Selection;
		}
		gC.TogglePauseGame ();
		GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 150), "The tutorial starts out with the game paused. Unpause the game by hitting the space key. Once unpaused, the platelet cells will begin construction. Their progress will be displayed in the bottom left corner along the grey bar with x4 at the top of it. Unpaused the game and wait for the three platelet cells to be created.", tutorialMessageStyle);
	}
	
	void DoSelection(){
		if (!bStartedDragCoroutine) {
			bStartedDragCoroutine = true;
			StartCoroutine (DragBox());
		}
	}
	
	IEnumerator DragBox(){
		float baseX = Screen.width * .45f;
		float baseY = Screen.height * .2f;
		float targetX = Screen.width * .55f;	
		float targetY = Screen.height * .38f;
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
	
	void FinishDialog(int windowID) {
		if (GUI.Button(new Rect(100, 125, 50, 20), "OK")) {
			currentState = TutorialStates.State.Done;
		}
	}
	
	void CommenceDialog(int windowID) {
		//gC.TogglePauseGame ();
		GUI.TextArea (new Rect (0, 20, 250, 25), "Commence the tutorial?", tutorialMessageStyle);
		gC.TogglePauseGame ();
		if (GUI.Button(new Rect(150, 50, 50, 20), "Yes")) {
			currentState = TutorialStates.State.PlateProduction;
			gC.rngManager.SpawnWound( GameObject.Find( "/Body/Stomach" ).GetComponent<Block>());
			gC.energy = 100;

		}
		if (GUI.Button(new Rect(50, 50, 50, 20), "No")) {
			currentState = TutorialStates.State.Off;
		}
	}
}