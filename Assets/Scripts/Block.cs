using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	public GameObject next_Block;	
	public GameControl game_Control;

	// Clicked on. Send selected WhiteBloodCell here
	void OnMouseDown() {
		foreach (WhiteBloodCell cell in game_Control.selected) {
			cell.renderer.material.color = Color.white;
			cell.bIsSelected = false;
			cell.current_Block = this.gameObject;
		}
		game_Control.selected = new ArrayList ();
	}
}
