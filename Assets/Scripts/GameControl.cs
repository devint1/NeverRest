using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameControl : MonoBehaviour {

	public const float WHITE_BLOOD_CELL_FOOD_RATE = 0.05f;
	public const float PLATELET_FOOD_RATE = 0.025f;
	private const float MAX_LEVEL_PROGRESS_SPEED = 10.0f;
	private const float MAX_ENERGY = 100.0f;
	private const int MAX_NUM_DEAD_BODY_PARTS = 6;
	private const float ENERGY_RESTORE_PER_SECOND = 4.0f;

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
	public int whiteBloodProduction = 0;
	public int plateletProduction = 0;
	public int liveRBCs;
	public int deadBlocks = 0;
	public float energy = 50f;
	public float rbcSpeed = 5;

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
	public bool firstMouse = false;

	public Tutorial tutorial;
	public RandomEventManager rngManager;
	public Persistence persistence;
	public Image background;
	public List<Sprite> backgroundImages = new List<Sprite>();
	public Slider heartSlider;
	public Texture energyImage;

	public SpriteRenderer levelCompletion;
	public Sprite bg1;
	public Sprite bg2;
	public Sprite bg3;

	private bool isTutorial;
	// int mousePressStart = -1;
	Vector3 mousePositionStart;

	Texture2D text;

	Rect box;

	bool mouseDown = false;
	bool drawText = false;
	bool gameOver = false;
	bool upgradeMenuOpen = false;
	float doubleClickTimer = 0;
	bool click = false;
	bool isPaused = false;
	int clickCount =0;
	float levelProgressSpeed = 1.0f;
	public float levelProgress = 0f;
	public int levelDistance = 2000;

	GameObject upgradeMenu;

	void Start() {
		persistence = GameObject.Find ("Persistence").GetComponent<Persistence>();

		// Get rid of purple/teal WBCs for level 1
		if (persistence.currentLevel <= 1) {
			Destroy(GameObject.Find("whitebloodcell_Button_Purple"));
			Destroy(GameObject.Find("whitebloodcell_Button_Teal"));
			//levelCompletion.sprite = bg1;
		}

		// Get rid of finder WBCs for levels 1 and 2
		if (persistence.currentLevel <= 2) {
			Destroy(GameObject.Find("whitebloodcell_Button_Finder"));
		}

		if (persistence.currentLevel == 2) {
			levelCompletion.sprite = bg2;
		}

		background.sprite = backgroundImages[persistence.currentLevel-1];

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
		levelDistance = 500 + 500 * persistence.currentLevel;

		int i = 0;

		//GameObject actionBar = (GameObject)Instantiate (actionBarPrefab, new Vector3(10, Screen.height - 150, -1), this.transform.rotation);

		doubleClicked = new ArrayList ();
		selected = new ArrayList();
		whiteBloodCells = new ArrayList();
		platelets = new ArrayList ();
		mousePositionStart = new Vector3();
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
		if (Input.GetMouseButtonDown (0)) {
			mouseDown= true;
			mousePositionStart = Event.current.mousePosition;
			if (click && Time.time <= doubleClickTimer +.35) {
				click = false;
				//doubleClickTimer = Time.time;
				//Debug.Log ("double click ");
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero);
				
				if(hit.collider != null){
					//Debug.Log ("hit " + hit.collider.tag);
					if(hit.collider.tag == "WhiteBloodCell"){
						//Debug.Log(" click WBC");
						foreach (WhiteBloodCell wbc in whiteBloodCells){
							if(wbc.currentBlock == current_b){
								wbc.Select();
							}
							else{
								wbc.DeSelect();
							}
						}
						foreach (Platelets plat in platelets){
								plat.DeSelect();

						}
					}
					else if(hit.collider.tag == "Platelet"){
						//Debug.Log(" click platelet");
						foreach (WhiteBloodCell wbc in whiteBloodCells){

								wbc.DeSelect();

						}
						foreach (Platelets plat in platelets){
							if(plat.currentBlock == current_b){
								plat.Select();
							}
							else {
								plat.DeSelect();
							}
						}
					}
					else{
						foreach (Platelets plat in platelets){

								plat.DeSelect();

						}
						foreach (WhiteBloodCell wbc in whiteBloodCells){
							
							wbc.DeSelect();
							
						}
						selected.Clear();
					}

				}

			}  else {
				click = true;
				doubleClickTimer = Time.time;
				//selected.Clear();
				if (!mouseDown && Input.GetMouseButton (0)) {
					
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
			}

		}

	}

	void OnGUI() {
		// Get white blood cell production from slider
		if (isPaused && showMenu) {
			GUI.Box (new Rect (Screen.width / 4, Screen.height / 4, Screen.width / 2, Screen.height / 2), "PAUSED");

			if (GUI.Button (new Rect (Screen.width / 4 + 10, Screen.height / 4 + Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "RESUME")) {
				isPaused = false;
				showMenu = false;
			}
			if (GUI.Button (new Rect (Screen.width / 4 + 10, Screen.height / 4 + 3 * Screen.height / 10 + 10, Screen.width / 2 - 20, Screen.height / 10), "MAIN MENU")) {
				Application.LoadLevel ("MenuScene");

			} 
		} else if (isPaused) {
			GUI.TextArea( new Rect( 400, 400, 100, 50), "PAUSED" ); 
		}

		rbcSpeed = (heartSlider.value);//(int)(heartSlider.value * 9.0f) + 1;
		//Debug.Log ("Slider val = " + rbcSpeed);

		// Display energy bar
		// draw the background:
		//GUI.BeginGroup (new Rect (20, 10, 500, 40));
		//GUI.Box (new Rect (0,0, 200, 40), barEmpty);
		
		// draw the filled-in part:
		//GUI.BeginGroup (new Rect (20, 10, 500f * energy/MAX_ENERGY, 40));
		//GUI.Box (new Rect (0,0, 200, 40),energyBarFull);
		GUI.DrawTexture (new Rect (10, 10, 313.0f * energy/MAX_ENERGY, 30), energyBarFull);
		GUI.DrawTexture (new Rect (0.0f, 0.0f, 400.0f, 50.0f), energyImage);
		GUI.Box (new Rect (325, 9, 70, 25), "" + (energy/MAX_ENERGY*100).ToString("F2") + "%");
		//GUI.EndGroup ();

		// Energ level text
		//GUI.Label(new Rect(70, 10, 100, 20), "Energy: " + (int)energy);

		//GUI.EndGroup ();

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
		if( Input.GetKeyDown( KeyCode.F6 )){
			rngManager.SpawnDiseaseInfection();
		}
		if( Input.GetKeyDown( KeyCode.F8 )){
			rngManager.SpawnDiseaseInfection();
		}
		if( Input.GetKeyDown( KeyCode.F7 )){
			rngManager.SpawnWound();
		}
		if (IsPaused () || showMenu || upgradeMenuOpen || tutorial.StopGameLogic ()) {
				rngManager.isDisabled = true;
		} else {
				rngManager.isDisabled = false;
		}

		if (Input.GetKeyDown(KeyCode.Space)){
			TogglePauseGame();
			showMenu = false;
		}

		// Restore Energy
		if (!IsPaused ()) {
			energy += (ENERGY_RESTORE_PER_SECOND - rbcSpeed/2) * Time.deltaTime;
			if (energy < 0){
				energy = 0;
			}
			else if (energy > MAX_ENERGY){
				energy = MAX_ENERGY;
			}
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
			levelProgress += levelProgressSpeed * Time.deltaTime * rbcSpeed/5;
		}

		if (whiteBloodCells != null) {
			//foreach (WhiteBloodCell cell in whiteBloodCells) {
			for(int i = 0; i < whiteBloodCells.Count; i++) {
				WhiteBloodCell cell = (WhiteBloodCell)(whiteBloodCells[i]);
				
				if (cell.destroyMe) {
					//Debug.Log ("deleting white blood cell...");
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
	}

	public void SpawnWhiteBloodCell(WhiteBloodCellType type) {
		GameObject newWhite = (GameObject)Instantiate (whiteBloodCellPrefab, whiteBloodSpawnPoint.GetRandomPoint(), this.transform.rotation);
		WhiteBloodCell newWhiteScript = newWhite.GetComponent<WhiteBloodCell> ();
		newWhiteScript.type = type;
		newWhiteScript.currentBlock = whiteBloodSpawnPoint;
		newWhiteScript.destination = whiteBloodSpawnPoint.GetRandomPoint ();
		newWhiteScript.gameControl = this;
		
		if (toggleWBC)
			newWhiteScript.GetComponent<Renderer>().enabled = false;
		
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
		GameObject winTextObj = new GameObject("WinText");
		winTextObj.transform.position = new Vector3(0.465f, 0.561f, 1f);
		GUIText winText = (GUIText)winTextObj.AddComponent(typeof(GUIText));
		winText.text = "YOU WIN!!!";
		winText.anchor = TextAnchor.MiddleCenter;
		winText.alignment = TextAlignment.Center;
		winText.fontSize = 100;
		yield return new WaitForSeconds(5);
		persistence.currentLevel++;
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
			if(b.blockType == BlockType.CHEST && b.dead) {
				return true;
			}
			if(b.blockType == BlockType.LIMB && b.dead){
				deadLimbs++;
			}
		}
		
		if (deadBlocks >= MAX_NUM_DEAD_BODY_PARTS)
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
