using UnityEngine;
using System.Collections;
using TutorialStates;

namespace TutorialStates
{
	public enum State{ Off, Done, Pause, Selection };
}

public class Tutorial : MonoBehaviour {
	public GameControl gC;

	public TutorialStates.State currentState;

	bool bStartedDragCoroutine;

	float dragPercent;

	Rect box;

	public bool StopGameLogic(){
		if (!(currentState == State.Done || currentState == State.Off)) {
			return true;
		}
		return false;
	}

	void Start(){
		currentState = TutorialStates.State.Pause;
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
			GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 100), "Multiple cells can be selected at a time by clicking and dragging around them. \nSelect both of the cells in the chest now.");
			if (gC.selected.Count >= 2){
				currentState = TutorialStates.State.Done;
			}
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
}