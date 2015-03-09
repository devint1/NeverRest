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

	public bool StopGameLogic(){
		if (!(currentState == State.Done || currentState == State.Off)) {
			return true;
		}
		return false;
	}

	void Start(){
		currentState = TutorialStates.State.Pause;
	}

	void OnGUI(){
		switch (currentState) {
		case TutorialStates.State.Off:
		case TutorialStates.State.Done:
			return;
		case TutorialStates.State.Pause:
			DoPause();
			return;
		}
	}

	void DoPause(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			currentState = TutorialStates.State.Done;
		}
		GUI.TextArea (new Rect (Screen.width/2 - 125, Screen.height/2 -50, 250, 100), "The game can be paused at any time by pressing space.\nThis will alow you more time to make decisions and issue commands.\nDo this now.");
	}
	
}