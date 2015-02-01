using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	public GameObject next_Block;	
	public GameControl game_Control;

	// Clicked on. Send selected WhiteBloodCell here
	void OnMouseDown() {
		if (!game_Control.selected)
			return;

		WhiteBloodCell selected_white = game_Control.selected.GetComponent<WhiteBloodCell> ();

		if (selected_white)
			selected_white.current_Block = this.gameObject;
	}
}
