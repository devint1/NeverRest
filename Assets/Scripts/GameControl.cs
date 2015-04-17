using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	public const float WHITE_BLOOD_CELL_FOOD_RATE = 0.05f;
	public const float PLATELET_FOOD_RATE = 0.025f;
	private const float MAX_LEVEL_PROGRESS_SPEED = 10.0f;
	private const float MAX_ENERGY = 100.0f;
	private const float ENERGY_RESTORE_PER_SECOND = 2.5f;

	public ArrayList selected;
	public ArrayList whiteBloodCells;
	public ArrayList platelets;
	public ArrayList doubleClicked;

	public Block whiteBloodSpawnPoint;
	public Block plateletSpawnPoint;
	public Block redBloodSpawnPoint;

	public GameObject whiteBloodCellPrefab;
	public GameObject plateletPrefab;
	public GameObject redBloodCellPrefab;
	public GameObject actionBarPrefab;

	public Texture2D energyBarFull;
	public Texture2D barEmpty;

	public AudioClip backGroundMusic = null;

	public int numDiseaseCells;
	public int numRBCs = 15;
	public int rbcSpeed = 1;
	public int whiteBloodProduction = 0;
	public int plateletProduction = 0;
	public int liveRBCs;
	public float energy = 50f;

	public Block current_b;
	public Body body;
	public bool wbcSelected =false;
	public bool plateletSelected = false;
	public bool toggleRBC = false;
	public bool toggleWBC = true;
	public bool changed = true;
	public bool wbcChanged = true;
	public bool isSelected = false;
	public bool showMenu = false;
	public bool isPause = false;
	public bool firstMouse = false;
	public Tutorial tutorial;
	public RandomEventManager rngManager;
	public Persistence persistence;
	public Image background;
	public List<Sprite> backgroundImages = new List<Sprite>();

	// int mousePressStart = -1;
	Vector3 mousePositionStart;

	Texture2D text;

	Rect box;

	bool mouseDown = false;
	bool drawText = false;
	bool gameOver = false;
	bool isPaused = false;
	bool upgradeMenuOpen = false;
	float doubleClickTimer = 0;
	bool click = false;

	float levelProgressSpeed = 1.0f;
	public float levelProgress = 0f;
	public int levelDistance = 2000;

	GameObject upgradeMenu;

	void Start() {
		persistence = GameObject.Find ("Persistence").GetComponent<Persistence>();

		//background.sprite = particlesByLevel[persistence.currentLevel-1];

		if (backGroundMusic) {
			AudioSource temp = gameObject.AddComponent<AudioSource> ();
			temp.clip = backGroundMusic;
			temp.Play();
		}

		//GameObject gameUI = (GameObject) Instantiate(Resources.Load("GameUI"), Vector3.zero, Quaternion.identity);
		//gameUI.GetComponent<GameUI>().gC = this;

		numDiseaseCells = 0;

		upgradeMenu = (GameObject) Instantiate(Resources.Load("UpgradeMenu"), Vector3.zero, Quaternion.identity);
		upgradeMenu.SetActive(false);

		tutorial = gameObject.AddComponent<Tutorial> ();
		tutorial.gC = this;

		int i = 0;
		//TODO move all the member assignment stuff into their start functions - I.E. should only be passed game control object and do it itself
		/*
		for(; i < numRBCs; i++) {
			Vector3 randpt = redBloodSpawnPoint.GetRandomPoint();
			GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, new Vector3(randpt.x, randpt.y, 1.0f) , this.transform.rotation);
			RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
			newRBC.renderer.transform.localScale = new Vector3(.6f,.6f,.6f);
			newRedScript.currentBlock = redBloodSpawnPoint;
			newRedScript.prevBlock = redBloodSpawnPoint;
			newRedScript.destination = redBloodSpawnPoint.GetRandomPoint ();
			newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
			newRedScript.destBlock = newRedScript.origBlock;
			newRedScript.heartBlock = body.GetChest ();
			newRedScript.gameControl = this;
			newRedScript.spawnTime = Time.time;
			newRedScript.renderer.enabled = false;
		}
		for(; i > 0; i--) {
			Vector3 randpt = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count)).GetRandomPoint();
			GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, new Vector3(randpt.x, randpt.y, 1.0f), this.transform.rotation);
			RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
			newRBC.renderer.transform.localScale = new Vector3(.6f,.6f,.6f);
			newRedScript.prevBlock = newRedScript.currentBlock;
			newRedScript.currentBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
			newRedScript.destination = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count)).GetRandomPoint ();
			newRedScript.origBlock = body.GetBodyPart((numRBCs - i) / (numRBCs / body.blocks.Count));
			newRedScript.heartBlock = body.GetChest ();
			newRedScript.destBlock = newRedScript.heartBlock;
			newRedScript.oxygenated = false;
			newRedScript.gameControl = this;
			newRedScript.spawnTime = Time.time;
			newRedScript.renderer.enabled = false;
		}
		liveRBCs = 2 * numRBCs;
		*/

		//GameObject actionBar = (GameObject)Instantiate (actionBarPrefab, new Vector3(10, Screen.height - 150, -1), this.transform.rotation);

		doubleClicked = new ArrayList ();
		selected = new ArrayList();
		whiteBloodCells = new ArrayList();
		platelets = new ArrayList ();
		mousePositionStart = new Vector3();
		SpawnWhiteBloodCell(WhiteBloodCellType.GREEN);
		SpawnPlatelet ();
	}

	public bool IsPaused(){
		return isPaused;
	}

	public void TogglePauseGame(){
		if (isPaused == false) {
			//Time.timeScale = 0;
		}
		else{
			//Time.timeScale = 1;
		}
		isPaused = !isPaused;

	}

	//Only call this function from on GUI
	void MouseSelection(){
		if (!mouseDown && Input.GetMouseButton (0)) {
			mousePositionStart = Event.current.mousePosition;
			mouseDown = true;
			if (click && Time.time <= (doubleClickTimer +.35))
			    {
				click =false;
				if (wbcSelected){
					foreach(WhiteBloodCell wbc in whiteBloodCells)
					{
						if (wbc.currentBlock == current_b){
							wbc.Select();
							doubleClicked.Add(wbc);

						}
					}
					Debug.Log("selected " + selected.Count);
					Debug.Log("Double click " + doubleClicked.Count);
				}

				/*
				if(selected != null) {
					Block current_b;

					foreach(GameObject obj in selected) {
						//Debug.Log("print obj "+ obj );
						if(!obj) {
							doubleClicked.Clear();
							click =false;
							continue;
						}
						if(obj.tag == "WhiteBloodCell") {
							obj.GetComponent<WhiteBloodCell> ().DeSelect();
							doubleClicked.Clear();
							current_b=obj.GetComponent<WhiteBloodCell> ().currentBlock;
							Debug.Log("White blood double clicked"); 
							//Debug.Log(obj.GetComponent<WhiteBloodCell> ().currentBlock + "obj");

							foreach(WhiteBloodCell wbc in doubleClicked){
								wbc.DeSelect();
							}
							Debug.Log("Number of WBC " +doubleClicked.Count);
							doubleClicked.Clear();
							click =false;
						}
						else if(obj.tag == "Platelet") {
							doubleClicked.Clear();
							current_b=obj.GetComponent<Platelets> ().currentBlock;
							obj.GetComponent<Platelets> ().DeSelect();
							Debug.Log("Platelet double clicked");
							foreach(Platelets plat in platelets)
							{
								
								if (plat.currentBlock == current_b){
									
									doubleClicked.Add(plat);
									
								}
							}
							foreach(Platelets plat in doubleClicked){
								plat.DeSelect();
							}
							Debug.Log("Number of Platelets " +doubleClicked.Count);
							doubleClicked.Clear();
							click =false;
						}
					}
					//Debug.Log ("clear double clcick");
					selected.Clear();

				}
				Debug.Log ("clear double click");
				click =false;
				*/

			}
			else if (click && Time.time >(doubleClickTimer +.35)){
				click =false;
			}
			else{
				click = true;
				doubleClickTimer= Time.time;
				//Debug.Log("single click ");
			}

			if (whiteBloodCells != null) {
				foreach(WhiteBloodCell cell in whiteBloodCells) {
					// FIXME: Find out why nulls are still in whiteBloodCells
					if(!cell) {
						continue;
					}
					cell.DeSelect();
				}
			}
			if(selected != null) {
				foreach(GameObject obj in selected) {
					// FIXME: Find out why nulls are still in selected
					if(!obj) {
						continue;
					}
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
				foreach(Platelets cell in platelets){
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

		if (Input.GetMouseButton (1) && !firstMouse) {
			firstMouse = true;
		}
		doubleClicked.Clear();
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
		rbcSpeed = (int)GUI.HorizontalSlider(new Rect(25, 55, 125, 30), rbcSpeed, 1.0F, 10.0F);
		
		// Display wihte blood cell production status
		/*if (whiteBloodProduction > 0) {
			GUI.TextArea (new Rect (25, 133, 125, 20), "1 per " + 30 / whiteBloodProduction + " seconds");
		} else {
			GUI.TextArea (new Rect (25, 133, 125, 20), "Production off");
		}*/
		
		GUI.TextArea (new Rect (25, 70, 125, 20), "Heart Rate");

		// Display energy bar
		// draw the background:
		GUI.BeginGroup (new Rect (20, 10, 200, 40));
		GUI.Box (new Rect (0,0, 200, 40), barEmpty);
		
		// draw the filled-in part:
		GUI.BeginGroup (new Rect (0, 0, 200f * energy/MAX_ENERGY, 40));
		GUI.Box (new Rect (0,0, 200, 40),energyBarFull);
		GUI.EndGroup ();

		// Energ level text
		GUI.Label(new Rect(70, 10, 100, 20), "Energy: " + (int)energy);

		GUI.EndGroup ();

		// Draw text if enabled
		if (drawText) {
			GUI.DrawTexture (box, text);
			drawText = false;
		}

		// Handle selection
		if (!showMenu && !upgradeMenuOpen){
			MouseSelection ();
		}
	}

	void Update() {
		if( Input.GetKeyDown( KeyCode.F8 )){
			rngManager.SpawnDiseaseInfection();
		}
		if (IsPaused () || showMenu || upgradeMenuOpen || tutorial.StopGameLogic ()) {
				rngManager.isDisabled = true;
		} else {
				rngManager.isDisabled = false;
		}

		if (Input.GetKeyDown(KeyCode.Space)){
			TogglePauseGame();
			showMenu = false;
			isPause = !isPause;
		}

		if( tutorial.StopGameLogic() ){
			return;
		}

		if (Input.GetKeyDown(KeyCode.Escape)){
			TogglePauseGame(); 
			if (isPaused){
				showMenu = true;
			}
			else{
				showMenu = false;
			}
		}

		if (Input.GetKeyDown(KeyCode.Z)){
			TogglePauseGame();
			showMenu = false;
			if (!upgradeMenuOpen){
				upgradeMenu.SetActive(true);
				upgradeMenuOpen = true;
			}
			else{
				upgradeMenu.SetActive(false);
				upgradeMenuOpen = false;
			}
		}
		if (IsPaused()){
			return;
		}
		
		if (Input.GetKeyDown (KeyCode.B)) {
			toggleRBC = !toggleRBC;
			Debug.Log("KeyDown!");
			changed = false;
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			toggleWBC = !toggleWBC;
			wbcChanged = false;
		}

		// Restore Energy
		energy += (ENERGY_RESTORE_PER_SECOND - rbcSpeed) * Time.deltaTime;
		if (energy < 0) energy = 0;
		else if (energy > MAX_ENERGY) energy = MAX_ENERGY;

		// Check lose condition
		if (checkLoseCondition () || Input.GetKeyDown (KeyCode.F10)) {
			StartCoroutine (Lose ());
		}

		// Check win condition
		if(levelProgress >= levelDistance || Input.GetKeyDown (KeyCode.F9)) {
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
					selected.Remove(cell.gameObject);
					Destroy (((WhiteBloodCell)(whiteBloodCells[i])).gameObject, 2);
					whiteBloodCells.RemoveAt(i);
					//foodLevel += WHITE_BLOOD_CELL_FOOD_RATE * 0.8f;
					i--;
				}
			}
			wbcChanged = true;
		}

		/*
		if (liveRBCs < 2 * numRBCs) {
			int diff = 2 * numRBCs - liveRBCs;
			for (int i = 0; i < diff; i++) {
				Vector3 randpt = redBloodSpawnPoint.GetRandomPoint();
				GameObject newRBC = (GameObject)Instantiate (redBloodCellPrefab, new Vector3(randpt.x, randpt.y, 1.0f) , this.transform.rotation);
				RedBloodScript newRedScript = newRBC.GetComponent<RedBloodScript> ();
				newRBC.renderer.transform.localScale = new Vector3(.1f,.1f,.1f);
				newRedScript.currentBlock = redBloodSpawnPoint;
				newRedScript.destination = redBloodSpawnPoint.GetRandomPoint ();
				newRedScript.origBlock = body.GetBodyPart((diff - i) / (numRBCs / body.blocks.Count));
				newRedScript.destBlock = newRedScript.origBlock;
				newRedScript.heartBlock = body.GetChest ();
				newRedScript.gameControl = this;
				newRedScript.spawnTime = Time.time;
			}
			liveRBCs += diff;
		}
		*/
	}

	public void SpawnWhiteBloodCell(WhiteBloodCellType type) {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.GetRandomPoint(), this.transform.rotation);
		WhiteBloodCell newWhiteScript = newWhite.GetComponent<WhiteBloodCell> ();
		newWhiteScript.type = type;
		newWhiteScript.currentBlock = whiteBloodSpawnPoint;
		newWhiteScript.destination = whiteBloodSpawnPoint.GetRandomPoint ();
		newWhiteScript.gameControl = this;
		
		if (toggleWBC)
			newWhiteScript.renderer.enabled = false;
		
		whiteBloodCells.Add (newWhite.GetComponent<WhiteBloodCell>());

	}

	public void SpawnPlatelet() {
		GameObject newPlate = (GameObject)Instantiate (plateletPrefab, plateletSpawnPoint.GetRandomPoint(), this.transform.rotation);
		Platelets newPlateletScript = newPlate.GetComponent<Platelets> ();
		newPlateletScript.currentBlock = plateletSpawnPoint;
		newPlateletScript.currentBlock.platelets.Add (newPlate);
		newPlateletScript.destination = plateletSpawnPoint.GetRandomPoint ();
		newPlateletScript.gameControl = this;
		newPlateletScript.spawnTime = Time.time;
		platelets.Add (newPlate.GetComponent<Platelets>());
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
		persistence.currentLevel++;
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

	public GameObject GetUpgradeMenu(){
		return upgradeMenu;
	}
}
