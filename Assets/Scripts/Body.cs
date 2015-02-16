using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Body : MonoBehaviour {
	public List<Block> blocks = new List<Block>();
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public Block GetBodyPart(int i) {
		if(i < blocks.Count)
			return blocks[i];
		return null;
	}
	
	public Block GetChest() {
		for (int i = 0; i < blocks.Count; i++) {
			if(blocks[i].blockType == BlockType.HEART) {
				return blocks[i];
			}
		}
		return null;
	}
}
