﻿using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public GameObject whiteBloodSpawnPoint;
	public GameObject whiteBloodCellPrefab;
	public Texture2D foodBarFull;
	public Texture2D healthBarFull;
	public Texture2D barEmpty;
	public float healthLevel = 1f;

	int whiteBloodProduction = 0;
	int mousePressStart = -1;
	Vector3 mousePositionStart;
	float timeOfLastSpawn;
	float foodLevel = 1f;
	bool mouseDown = false;
	bool drawText = false;
	Texture2D text;
	Rect box;

	void Start() {
		timeOfLastSpawn = Time.time;
		selected = new ArrayList ();
		whiteBloodCells = new ArrayList ();
		mousePositionStart = new Vector3 ();
		SpawnWhiteBloodCell();
	}

	void OnGUI() {
		// Get white blood cell production from slider
		whiteBloodProduction = (int)GUI.HorizontalSlider(new Rect(25, 90, 125, 30), whiteBloodProduction, 0.0F, 10.0F);
		
		// Display wihte blood cell production status
		if (whiteBloodProduction > 0) {
			GUI.TextArea (new Rect (25, 105, 125, 20), "1 per " + 30 / whiteBloodProduction + " seconds");
		} else {
			GUI.TextArea (new Rect (25, 105, 125, 20), "Production off");
		}

		// Display food bar
		GUI.BeginGroup (new Rect (20, 10, 125, 30));
		GUI.Box (new Rect (0,0, 125, 30),barEmpty);
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, 125 * foodLevel, 30));
		GUI.Box (new Rect (0,0, 125, 30),foodBarFull);
		GUI.EndGroup ();
		GUI.EndGroup ();

		// Display health bar
		GUI.BeginGroup (new Rect (20, 50, 125, 30));
		GUI.Box (new Rect (0, 0, 125, 30), barEmpty);
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, 125 * healthLevel, 30));
		GUI.Box (new Rect (0,0, 125, 30), healthBarFull);
		GUI.EndGroup ();
		GUI.EndGroup ();

		// Draw text if enabled
		if (drawText) {
			GUI.DrawTexture (box, text);
			drawText = false;
		}

		// Handle selection
		if (!mouseDown && Input.GetMouseButton (0)) {
			mousePositionStart = Event.current.mousePosition;
			mouseDown = true;
			if (whiteBloodCells != null) {
				foreach(WhiteBloodCell cell in whiteBloodCells) {
					cell.DeSelect();
				}
			}
		} else if (mouseDown && Input.GetMouseButton (0)) {
			text = new Texture2D (1, 1);
			Color col = Color.green;
			col.a = .15f;
			text.SetPixel (1, 1, col);
			text.Apply ();
			box = new Rect (mousePositionStart.x, mousePositionStart.y,
			                Event.current.mousePosition.x - mousePositionStart.x, 
			                Event.current.mousePosition.y - mousePositionStart.y);
			GUI.DrawTexture (box, text);
			drawText = true;
		} else if(mouseDown && !(Input.GetMouseButton (0))) {
			box = new Rect(mousePositionStart.x, mousePositionStart.y,
			               Event.current.mousePosition.x - mousePositionStart.x, 
			               Event.current.mousePosition.y - mousePositionStart.y);
			if (whiteBloodCells != null) {
				foreach(WhiteBloodCell cell in whiteBloodCells){
					Vector2 pos = Camera.main.WorldToScreenPoint(cell.transform.position);
					pos.y = Camera.main.pixelHeight - pos.y;
					if(box.Contains(pos, true)){
						cell.Select();
					}
					
				}
			}
			mouseDown = false;
			mousePositionStart.x = 0;
			mousePositionStart.y = 0;
		}
	}

	void Update() {
		if (whiteBloodProduction > 0 && (Time.time - timeOfLastSpawn) > 30 / whiteBloodProduction) {
			SpawnWhiteBloodCell ();
			foodLevel -= 0.05f;
		}

		if (foodLevel <= 0f || healthLevel <= 0f) {
			Application.LoadLevel ("MenuScene");
		}

		if (whiteBloodCells != null) {
			foreach (WhiteBloodCell cell in whiteBloodCells) {
				if (cell.destroyMe) {
					whiteBloodCells.Remove (cell);
					Destroy (cell, 2.0f);
				}
			}
		}
	}

	void SpawnWhiteBloodCell() {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.transform.position, this.transform.rotation);
		newWhite.GetComponent<WhiteBloodCell> ().currentBlock = whiteBloodSpawnPoint;
		newWhite.GetComponent<WhiteBloodCell> ().gameControl = this;
		whiteBloodCells.Add (newWhite.GetComponent<WhiteBloodCell>());
		timeOfLastSpawn = Time.time;
	}
}
