using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public Block whiteBloodSpawnPoint;
	public GameObject whiteBloodCellPrefab;
	public Texture2D foodBarFull;
	public Texture2D healthBarFull;
	public Texture2D barEmpty;
	public float healthLevel = 1f;
	public int numDiseaseCells = 2;
	public AudioClip backGroundMusic = null;
	public bool toggleRBC = false;
	public int numRBCs = 18;
	public GameObject redBloodCellPrefab;
	public Block redBloodSpawnPoint;
	public Body body;
	public bool isSelected = false;
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
	bool gameOver = false;
	bool isPaused = false;
	bool showMenu = false;
	bool changed = true;

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

	public bool CheckIfPaused(){
		return isPaused;
	}

	void TogglePauseGame(){
		if (isPaused == false) {
			Time.timeScale = 0;
		}
		else{
			Time.timeScale = 1;
		}
		isPaused = !isPaused;

	}

	//Only call this function from on GUI
	void MouseSelection(){
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

	void OnGUI() {
		// Get white blood cell production from slider
		if (isPaused && showMenu){
			GUI.Box(new Rect(Screen.width/4, Screen.height/4, Screen.width/2, Screen.height/2), "PAUSED");

			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+Screen.height/10+10, Screen.width/2-20, Screen.height/10), "RESUME")){
				isPaused = false;
				showMenu = false;
			}
			if (GUI.Button(new Rect(Screen.width/4+10, Screen.height/4+3*Screen.height/10+10, Screen.width/2-20, Screen.height/10), "MAIN MENU")){
				Application.LoadLevel("MenuScene");

			} 
		}
		if (isPaused && showMenu == false){
			GUI.Box(new Rect(Screen.width/3, Screen.height/18, Screen.width/4, Screen.height/8), "PAUSED \n Space Bar to Resume");

		}
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
		if (!CheckIfPaused()){
			MouseSelection ();
		}
		else{
			//Call function to handle pause menu here
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.Escape)){
			TogglePauseGame(); 
			if (isPaused){
				showMenu = true;
			}
			else{
				showMenu = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.Space)){
			TogglePauseGame();
			showMenu = false;
		}

		if (Input.GetKeyDown(KeyCode.Z)){
			//Just a proof of concept atm
			TogglePauseGame();
			showMenu = false;
			Instantiate(Resources.Load("UpgradeMenu"), Vector3.zero, Quaternion.identity);
		}

		if (CheckIfPaused()){
			return;
		}
		
		if (Input.GetKeyDown (KeyCode.B)) {
			toggleRBC = !toggleRBC;
			changed = false;
		}

		if (whiteBloodProduction > 0 && (Time.time - timeOfLastSpawn) > 30 / whiteBloodProduction) {
			SpawnWhiteBloodCell ();
			foodLevel -= WHITE_BLOOD_CELL_FOOD_RATE;
		}

		// Check losing condition
		if (foodLevel <= 0f || healthLevel <= 0f) {
			StartCoroutine (Lose ());
		}

		// Check winning condition
		if (numDiseaseCells <= 0) {
			StartCoroutine (Win ());
		}

		if (whiteBloodCells != null) {
			//foreach (WhiteBloodCell cell in whiteBloodCells) {
			for(int i = 0; i < whiteBloodCells.Count; i++) {
				WhiteBloodCell cell = (WhiteBloodCell)(whiteBloodCells[i]);

				if(toggleRBC && !changed) {
					cell.renderer.enabled = false;
				}
				else if(!toggleRBC && !changed) {
					cell.renderer.enabled = true;
				}
				
				if (cell.destroyMe) {
					Debug.Log ("deleting white blood cell...");
					//whiteBloodCells.Remove (cell);
					Destroy (((WhiteBloodCell)(whiteBloodCells[i])).gameObject, 2);
					whiteBloodCells.RemoveAt(i);
					foodLevel += WHITE_BLOOD_CELL_FOOD_RATE * 0.8f;
					i--;
				}
			}
		}

		if (toggleRBC && !changed) {
			int i = 0;
			for(; i < numRBCs; i++) {
				GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, redBloodSpawnPoint.GetRandomPoint(), this.transform.rotation);
				RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
				newRedScript.currentBlock = redBloodSpawnPoint;
				newRedScript.destination = redBloodSpawnPoint.GetRandomPoint ();
				newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / 3);
				newRedScript.destBlock = newRedScript.origBlock;
				newRedScript.heartBlock = body.GetChest ();
				newRedScript.gameControl = this;
			}
			for(; i > 0; i--) {
				GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, body.GetBodyPart((numRBCs - i) / 3).GetRandomPoint(), this.transform.rotation);
				RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
				newRedScript.currentBlock = body.GetBodyPart((numRBCs - i) / 3);
				newRedScript.destination = body.GetBodyPart((numRBCs - i) / 3).GetRandomPoint ();
				newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / 3);
				newRedScript.heartBlock = body.GetChest ();
				newRedScript.destBlock = newRedScript.heartBlock;
				newRedScript.oxygenated = false;
				newRedScript.gameControl = this;
			}
			
			changed = true;
		}
	}

	void SpawnWhiteBloodCell() {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.GetRandomPoint(), this.transform.rotation);
		WhiteBloodCell newWhiteScript = newWhite.GetComponent<WhiteBloodCell> ();
		newWhiteScript.currentBlock = whiteBloodSpawnPoint;
		newWhiteScript.destination = whiteBloodSpawnPoint.GetRandomPoint ();
		newWhiteScript.gameControl = this;
		
		if (toggleRBC)
			newWhiteScript.renderer.enabled = false;
		
		whiteBloodCells.Add (newWhite.GetComponent<WhiteBloodCell>());
		timeOfLastSpawn = Time.time;
	}

	IEnumerator Wait(float f) {
		yield return new WaitForSeconds (f);
	}

	// Wins the game!
	IEnumerator Win() {
		if (gameOver) {
			yield break;
		}
		gameOver = true;
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

	IEnumerator Lose() {
		if (gameOver) {
			yield break;
		}
		gameOver = true;
		GameObject winTextObj = new GameObject("WinText");
		winTextObj.transform.position = new Vector3(0.465f, 0.561f, 1f);
		GUIText winText = (GUIText)winTextObj.AddComponent(typeof(GUIText));
		winText.text = "YOU LOSE!!!";
		winText.anchor = TextAnchor.MiddleCenter;
		winText.alignment = TextAlignment.Center;
		winText.fontSize = 100;
		yield return new WaitForSeconds(5);
		Application.LoadLevel("MenuScene");
	}
}
