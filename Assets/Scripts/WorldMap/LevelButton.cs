using UnityEngine;
using System.Collections;

public class LevelButton : MonoBehaviour {
	public int levelNumber;

	public Sprite BEATEN_IMAGE;
	public Sprite UNBEATEN_IMAGE;
	public Sprite UNDISCOVERED_IMAGE;
	public Sprite HOVER_OVER_IMAGE;

	public Persistence p;
	private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
		p = GameObject.FindGameObjectWithTag ("Persistence").GetComponent<Persistence>();

		if (levelNumber == p.currentLevel) {
			spriteRenderer.sprite = UNBEATEN_IMAGE;
		}
		else if (levelNumber < p.currentLevel) {
			spriteRenderer.sprite = BEATEN_IMAGE;
		}
		else if (levelNumber > p.currentLevel) {
			spriteRenderer.sprite = UNDISCOVERED_IMAGE;
		}
	}

	void OnMouseDown() {
		if (p.currentLevel == levelNumber) {
			Application.LoadLevel("GameScene");
		}
	}
	
	void OnMouseOver() {
		if (p.currentLevel == levelNumber) {
			spriteRenderer.sprite = HOVER_OVER_IMAGE;
		}
	}
	
	void OnMouseExit() {
		if (p.currentLevel == levelNumber) {
			spriteRenderer.sprite = UNBEATEN_IMAGE;
		}
	}
}
