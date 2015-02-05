using UnityEngine;
using System.Collections;

public enum BlockType { HEART, OTHER }

public class Block : MonoBehaviour {
	public GameObject nextBlock;	
	public GameControl gameControl;
	public BlockType blockType;
	public ArrayList diseases = new ArrayList();

	public static int MAX_NUM_DISEASE_PER_BLOCK = 8;

	private int whiteCellsTargeting = 0; // Number of WhiteBloodCells moving to this block
	GameObject destTarget = null;

	void Start() {
		//diseases = new ArrayList ();
	}


	// Block clicked. Send selected WhiteBloodCell here
	void OnMouseDown() {
		foreach (WhiteBloodCell cell in gameControl.selected) {
			cell.renderer.material.color = Color.white;
			cell.isSelected = false;
			cell.currentBlock = this.gameObject;
			cell.destBlock = this;
			this.increaseWBCsTargeting();
		}
		gameControl.selected.Clear();
//		game_Control.selected = new ArrayList ();
	}

	public void increaseWBCsTargeting() {
		if (whiteCellsTargeting == 0) {
			destTarget = (GameObject)Instantiate(this.gameControl.destMarkPrefab,
			                                       this.renderer.bounds.center,
			                                       this.transform.rotation);
		}
		whiteCellsTargeting++;
	}

	public void decreaseWBCsTargeting() {
		whiteCellsTargeting--;
		if (whiteCellsTargeting == 0) {
			Destroy(destTarget);	
		}
	}
}
