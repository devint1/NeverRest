using UnityEngine;
using System.Collections;

public enum BlockType { HEART, OTHER }

public class Block : MonoBehaviour {
	public GameObject nextBlock;	
	public GameControl gameControl;
	public BlockType blockType;

	// Clicked on. Send selected WhiteBloodCell here
	void OnMouseDown() {
		foreach (WhiteBloodCell cell in gameControl.selected) {
			cell.renderer.material.color = Color.white;
			cell.isSelected = false;
			cell.currentBlock = this.gameObject;
		}
		gameControl.selected = new ArrayList ();
	}
}
