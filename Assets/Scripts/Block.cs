﻿using UnityEngine;
using System.Collections;

public class Block : MonoBehaviour {
	public GameObject next_Block;	
	public GameControl game_Control;

	private int whiteCellsTargeting = 0; // Number of WhiteBloodCells moving to this block
	GameObject destTarget = null;

	// Block clicked. Send selected WhiteBloodCell here
	void OnMouseDown() {
		foreach (WhiteBloodCell cell in game_Control.selected) {
			cell.renderer.material.color = Color.white;
			cell.bIsSelected = false;
			cell.current_Block = this.gameObject;
			cell.destBlock = this;
			this.increaseWBCsTargeting();
		}
		game_Control.selected.Clear();
//		game_Control.selected = new ArrayList ();
	}

	public void increaseWBCsTargeting() {
		whiteCellsTargeting++;
		if (whiteCellsTargeting > 0) {
			destTarget = (GameObject)Instantiate(this.game_Control.destMarkPrefab,
			                                       this.renderer.bounds.center,
			                                       this.transform.rotation);
		}
	}

	public void decreaseWBCsTargeting() {
		whiteCellsTargeting--;
		if (whiteCellsTargeting == 0) {
			Destroy(destTarget);	
		}
	}
}
