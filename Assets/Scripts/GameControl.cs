using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public Block whiteBloodSpawnPoint;
	public GameObject whiteBloodCellPrefab;
	public GameObject destMarkPrefab;
	public Texture2D foodBarFull;
	public Texture2D healthBarFull;
	public Texture2D barEmpty;
	public float healthLevel = 1f;
	public int numDiseaseCells = 2;
	public AudioClip backGroundMusic = null;
	public bool paused = false;

	
	const float WHITE_BLOOD_CELL_FOOD_RATE = 0.05f;

	int whiteBloodProduction = 0;
	// int mousePressStart = -1;
	Vector3 mousePositionStart;
	float timeOfLastSpawn;
	float foodLevel = 1f;
	bool mouseDown = false;
	bool drawText = false;
	Texture2D text;
	Rect box;
	bool won = false;


	void Start() {
		if (backGroundMusic) {
			AudioSource temp = gameObject.AddComponent<AudioSource> ();
			temp.clip = backGroundMusic;
			temp.Play();
		}

		timeOfLastSpawn = Time.time;
		selected = new ArrayList();
		whiteBloodCells = new ArrayList();
		mousePositionStart = new Vector3();
		SpawnWhiteBloodCell();
	}

	void OnGUI() {
		if (paused){     
			GUI.Box(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), "PAUSED");
			//GUI.Label(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+10, Screen.width/2-20, Screen.height/10), "YOUR SCORE: "+ ((int)score));
			
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+10, Screen.width/2-20, Screen.height/10), "RESUME")){
				paused = false;
			}
			/*
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+2*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "RESTART")){
				Application.LoadLevel(Application.loadedLevel);
			}
			*/
			 
		}
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
			// Beginning of box selection
			// Draw selection box
			text = new Texture2D (1, 1);
			Color col = Color.green;
			col.a = .15f;
			text.SetPixel (1, 1, col);
			text.Apply ();
			box = new Rect(mousePositionStart.x, mousePositionStart.y,
			                Event.current.mousePosition.x - mousePositionStart.x, 
			                Event.current.mousePosition.y - mousePositionStart.y);
			GUI.DrawTexture (box, text);
			drawText = true;
		} else if(mouseDown && !(Input.GetMouseButton (0))) {
			// End of box selection
			// Select cells in box
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
		if (Input.GetKey(KeyCode.Escape) ) //check if Escape key/Back key is pressed
		{
			if (paused)
				paused = false;  //unpause the game if already paused
			else
				paused = true;  //pause the game if not paused
		}
		
		if(paused)
			Time.timeScale = 0;  //set the timeScale to 0 so that all the procedings are halted
		else
			Time.timeScale = 1;  //set it back to 1 on unpausing the game
		if (paused) {
			if (whiteBloodProduction > 0 && (Time.time - timeOfLastSpawn) > 30 / whiteBloodProduction) {
				SpawnWhiteBloodCell ();
				foodLevel -= WHITE_BLOOD_CELL_FOOD_RATE;
				}
			// Check losing condition
			if (foodLevel <= 0f || healthLevel <= 0f) {
				Application.LoadLevel ("MenuScene");
			}
			// Check winning condition
			if (numDiseaseCells <= 0) {
				StartCoroutine (win ());
				}
			if (whiteBloodCells != null) {
				//foreach (WhiteBloodCell cell in whiteBloodCells) {
				for (int i = 0; i < whiteBloodCells.Count; i++) {
					WhiteBloodCell cell = (WhiteBloodCell)(whiteBloodCells [i]);
					if (cell.destroyMe) {
						Debug.Log ("deleting white blood cell...");
						//whiteBloodCells.Remove (cell);
						Destroy (((WhiteBloodCell)(whiteBloodCells [i])).gameObject, 2);
						whiteBloodCells.RemoveAt (i);
						foodLevel += WHITE_BLOOD_CELL_FOOD_RATE * 0.8f;
						i--;
						}
					}
				}
		}
	}

	void SpawnWhiteBloodCell() {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.GetRandomPoint().transform.position, this.transform.rotation);
		WhiteBloodCell newWhiteScript = newWhite.GetComponent<WhiteBloodCell> ();
		newWhiteScript.currentBlock = whiteBloodSpawnPoint;
		newWhiteScript.headingToward = whiteBloodSpawnPoint.GetRandomPoint ();
		newWhiteScript.gameControl = this;
		whiteBloodCells.Add (newWhite.GetComponent<WhiteBloodCell>());
		timeOfLastSpawn = Time.time;
	}

	// Wins the game!
	IEnumerator win() {
		if (won) {
			yield break;
		}
		won = true;
		GameObject winTextObj = new GameObject("WinText");
		winTextObj.transform.position = new Vector3(0.465f, 0.561f, 1f);
		GUIText winText = (GUIText)winTextObj.AddComponent(typeof(GUIText));
		winText.text = "YOU WIN!!!";
		winText.anchor = TextAnchor.MiddleCenter;
		winText.alignment = TextAlignment.Center;
		winText.fontSize = 100;
		yield return new WaitForSeconds(5);
		Application.LoadLevel("MenuScene");
	}
}
