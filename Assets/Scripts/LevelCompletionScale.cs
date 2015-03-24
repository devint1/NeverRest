using UnityEngine;
using System.Collections;

public class LevelCompletionScale : MonoBehaviour {

	public GameControl gameControl;
	public Transform start;
	public Transform end;
	public Transform levelCompletionClimber;
	public Transform numbersDisplayPoint;

	// Update is called once per frame
	void Update () {
		Vector3 new_position = new Vector3( start.position.x + (end.position.x - start.position.x) * ( gameControl.levelProgress/gameControl.levelDistance ), levelCompletionClimber.position.y, levelCompletionClimber.position.z );

		levelCompletionClimber.position = new_position;
	}

	void OnGUI() {
		Vector3 position = Camera.main.WorldToScreenPoint (numbersDisplayPoint.position);

		GUI.TextArea (new Rect (position.x, Screen.height-position.y, 125, 38), "Level Completion:\n" + (int)gameControl.levelProgress + "/" + gameControl.levelDistance + " ft");
	}
}
