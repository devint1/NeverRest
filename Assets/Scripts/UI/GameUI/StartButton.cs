using UnityEngine;
using System.Collections;

public class StartButton : MonoBehaviour {
	public Sprite HOVER_IMAGE;
	public Sprite REGULAR_IMAGE;

	void OnMouseOver () {
		this.GetComponent<SpriteRenderer> ().sprite = HOVER_IMAGE;
	}

	void OnMouseExit () {
		this.GetComponent<SpriteRenderer> ().sprite = REGULAR_IMAGE;
	}

	void OnMouseDown () {
		Application.LoadLevel ("MapScene"); 
	}
}
