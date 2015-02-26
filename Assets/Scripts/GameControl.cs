using UnityEngine;
using System.Collections;

public class GameControl : MonoBehaviour {
	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public ArrayList platelets;
	public Block whiteBloodSpawnPoint;
	public GameObject whiteBloodCellPrefab;
	public GameObject plateletPrefab;
	public Block plateletSpawnPoint;
	public Texture2D foodBarFull;
	public Texture2D healthBarFull;
	public Texture2D barEmpty;
	public float healthLevel = 1f;
	public int numDiseaseCells = 2;
	public AudioClip backGroundMusic = null;
	public bool toggleRBC = false;
	public bool toggleWBC = true;
	public bool togglePT = false;
	public int numRBCs = 18;
	public GameObject redBloodCellPrefab;
	public Block redBloodSpawnPoint;
	public Body body;
	public bool isSelected = false;
	public int rbcSpeed = 1;
	public float foodLevel = 1f;
	public bool changed = true;
	public bool wbcChanged = true;
	public int whiteBloodProduction = 0;
	public int plateletProduction = 0;
	public UnityEngine.UI.Slider wbcSlider;
	public UnityEngine.UI.Slider rbcSlider;
	public UnityEngine.UI.Slider plateSlider;

	public const float WHITE_BLOOD_CELL_FOOD_RATE = 0.05f;
	public const float PLATELET_FOOD_RATE = 0.025f;

	private const float MAX_LEVEL_PROGRESS_SPEED = 10.0f;

	// int mousePressStart = -1;
	Vector3 mousePositionStart;
	float timeOfLastSpawn;
	float timeOfLastPlateSpawn;
	bool mouseDown = false;
	bool drawText = false;
	Texture2D text;
	Rect box;
	bool gameOver = false;
	bool isPaused = false;
	public bool showMenu = false;
	float levelProgressSpeed = 1.0f;
	int levelDistance = 2000;
	float levelProgress = 0f;

	void Start() {
		if (backGroundMusic) {
			AudioSource temp = gameObject.AddComponent<AudioSource> ();
			temp.clip = backGroundMusic;
			temp.Play();
		}

		if (wbcSlider != null) {
			wbcSlider.onValueChanged.AddListener(changeWBCSliderVal);
		}
		if (plateSlider != null) {
			plateSlider.onValueChanged.AddListener(changePlateSliderVal);
		}
		if (rbcSlider != null) {

		}

		timeOfLastSpawn = Time.time;
		timeOfLastPlateSpawn = Time.time;
		selected = new ArrayList();
		whiteBloodCells = new ArrayList();
		platelets = new ArrayList ();
		mousePositionStart = new Vector3();
		SpawnWhiteBloodCell();
	}

	void changeWBCSliderVal(float f) {
		whiteBloodProduction = (int)(f);
	}

	void changeRBCSliderVal(float f) {

	}

	void changePlateSliderVal(float f) {
		plateletProduction = (int)(f);
	}

	public bool CheckIfPaused(){
		return isPaused;
	}

	public void TogglePauseGame(){
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
			/*if (whiteBloodCells != null) {
				foreach(WhiteBloodCell cell in whiteBloodCells) {
					cell.DeSelect();
				}
			}*/
			if(selected != null) {
				foreach(GameObject obj in selected) {
					if(obj.tag == "WhiteBloodCell") {
						obj.GetComponent<WhiteBloodCell> ().DeSelect();
					}
					else if(obj.tag == "Platelet") {
						obj.GetComponent<Platelets> ().DeSelect();
					}
				}
				selected.Clear();
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
				if(toggleWBC) {
					foreach(WhiteBloodCell cell in whiteBloodCells){
						Vector2 pos = Camera.main.WorldToScreenPoint(cell.transform.position);
						pos.y = Camera.main.pixelHeight - pos.y;
						if(box.Contains(pos, true)){
							cell.Select();
						}
					}
				}
				if(togglePT) {
					foreach(Platelets cell in platelets){
						Vector2 pos = Camera.main.WorldToScreenPoint(cell.transform.position);
						pos.y = Camera.main.pixelHeight - pos.y;
						if(box.Contains(pos, true)){
							cell.Select();
						}
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
			//GUI.Box(new Rect(Screen.width/3, Screen.height/18, Screen.width/4, Screen.height/8), "PAUSED \n Space Bar to Resume");

		}
		//whiteBloodProduction = (int)GUI.HorizontalSlider(new Rect(25, 50, 125, 30), whiteBloodProduction, 0.0F, 10.0F);
		rbcSpeed = (int)GUI.HorizontalSlider(new Rect(25, 50, 125, 30), rbcSpeed, 1.0F, 10.0F);
		
		// Display wihte blood cell production status
		if (whiteBloodProduction > 0) {
			GUI.TextArea (new Rect (25, 133, 125, 20), "1 per " + 30 / whiteBloodProduction + " seconds");
		} else {
			GUI.TextArea (new Rect (25, 133, 125, 20), "Production off");
		}
		
		GUI.TextArea (new Rect (25, 65, 125, 20), "Heart Rate");

		GUI.TextArea (new Rect (25, 90, 125, 38), "Level Completion:\n" + (int)levelProgress + "/" + levelDistance + " ft");

		// Display food bar
		GUI.BeginGroup (new Rect (20, 10, 125, 30));
		GUI.Box (new Rect (0,0, 125, 30),barEmpty);
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, 125 * foodLevel, 30));
		GUI.Box (new Rect (0,0, 125, 30),foodBarFull);
		GUI.EndGroup ();
		GUI.EndGroup ();

		// Display health bar
		/*GUI.BeginGroup (new Rect (20, 50, 125, 30));
		GUI.Box (new Rect (0, 0, 125, 30), barEmpty);
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, 125 * healthLevel, 30));
		GUI.Box (new Rect (0,0, 125, 30), healthBarFull);
		GUI.EndGroup ();
		GUI.EndGroup ();*/

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
			Debug.Log("KeyDown!");
			changed = false;
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			toggleWBC = !toggleWBC;
			wbcChanged = false;
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			togglePT = !togglePT;
		}

		if (whiteBloodProduction > 0 && (Time.time - timeOfLastSpawn) > 30 / whiteBloodProduction) {
			SpawnWhiteBloodCell ();
			foodLevel -= WHITE_BLOOD_CELL_FOOD_RATE;
		}

		if (plateletProduction > 0 && (Time.time - timeOfLastPlateSpawn) > 30 / plateletProduction) {
			SpawnPlatelet ();
			foodLevel -= PLATELET_FOOD_RATE;
		}

		// Check lose condition
		if (checkLoseCondition ()) {
			StartCoroutine (Lose ());
		}

		// Check win condition
		if(levelProgress >= levelDistance) {
			StartCoroutine (Win ());
		}
		else {
			levelProgressSpeed = calcLevelProgressSpeed();
			levelProgress += levelProgressSpeed * Time.deltaTime;
		}

		if (whiteBloodCells != null) {
			//foreach (WhiteBloodCell cell in whiteBloodCells) {
			for(int i = 0; i < whiteBloodCells.Count; i++) {
				WhiteBloodCell cell = (WhiteBloodCell)(whiteBloodCells[i]);
				
				if (cell.destroyMe) {
					Debug.Log ("deleting white blood cell...");
					//whiteBloodCells.Remove (cell);
					Destroy (((WhiteBloodCell)(whiteBloodCells[i])).gameObject, 2);
					whiteBloodCells.RemoveAt(i);
					foodLevel += WHITE_BLOOD_CELL_FOOD_RATE * 0.8f;
					i--;
				}
			}
			wbcChanged = true;
		}

		if (toggleRBC && !changed) {
			changed = true;
			int i = 0;
			for(; i < numRBCs; i++) {
				Vector3 randpt = redBloodSpawnPoint.GetRandomPoint();
				GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, new Vector3(randpt.x, randpt.y, 1.0f) , this.transform.rotation);
				RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
				newRedScript.currentBlock = redBloodSpawnPoint;
				newRedScript.destination = redBloodSpawnPoint.GetRandomPoint ();
				newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
				newRedScript.destBlock = newRedScript.origBlock;
				newRedScript.heartBlock = body.GetChest ();
				newRedScript.gameControl = this;
			}
			for(; i > 0; i--) {
				Vector3 randpt = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count)).GetRandomPoint();
				GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, new Vector3(randpt.x, randpt.y, 1.0f), this.transform.rotation);
				RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
				newRedScript.currentBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
				newRedScript.destination = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count)).GetRandomPoint ();
				newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
				newRedScript.heartBlock = body.GetChest ();
				newRedScript.destBlock = newRedScript.heartBlock;
				newRedScript.oxygenated = false;
				newRedScript.gameControl = this;
			}
			

		}
	}

	void SpawnWhiteBloodCell() {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.GetRandomPoint(), this.transform.rotation);
		WhiteBloodCell newWhiteScript = newWhite.GetComponent<WhiteBloodCell> ();
		newWhiteScript.currentBlock = whiteBloodSpawnPoint;
		newWhiteScript.destination = whiteBloodSpawnPoint.GetRandomPoint ();
		newWhiteScript.gameControl = this;
		
		if (toggleWBC)
			newWhiteScript.renderer.enabled = false;
		
		whiteBloodCells.Add (newWhite.GetComponent<WhiteBloodCell>());
		timeOfLastSpawn = Time.time;
	}

	void SpawnPlatelet() {
		GameObject newPlate = (GameObject)Instantiate (plateletPrefab, plateletSpawnPoint.GetRandomPoint(), this.transform.rotation);
		Platelets newPlateletScript = newPlate.GetComponent<Platelets> ();
		newPlateletScript.currentBlock = plateletSpawnPoint;
		newPlateletScript.destination = plateletSpawnPoint.GetRandomPoint ();
		newPlateletScript.gameControl = this;
		newPlateletScript.spawnTime = Time.time;
		
		if (togglePT)
			newPlateletScript.renderer.enabled = false;
		
		platelets.Add (newPlate.GetComponent<Platelets>());
		timeOfLastPlateSpawn = Time.time;
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
		Application.LoadLevel("MapScene");
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
		Application.LoadLevel("MapScene");
	}

	// Lose if Chest is at 0 health or if all limbs are at 0 health
	bool checkLoseCondition(){
		int deadLimbs = 0;
		foreach (Block b in body.blocks) {
			if(b.blockType == BlockType.CHEST && b.overallHealth <= 0) {
				return true;
			}
			if(b.blockType == BlockType.LIMB && b.overallHealth <= 0){
				deadLimbs++;
			}
		}

		if (deadLimbs >= 4)
			return true;
		else
			return false;
	}

	float calcLevelProgressSpeed() {
		float totalBodyPartHealth = 0f;

		foreach (Block b in body.blocks) {
			totalBodyPartHealth += b.overallHealth;
		}

		float averageHealth = totalBodyPartHealth / body.blocks.Count;

		return MAX_LEVEL_PROGRESS_SPEED * averageHealth;
	}
}
