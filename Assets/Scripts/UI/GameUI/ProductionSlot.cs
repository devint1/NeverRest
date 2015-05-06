using UnityEngine;
using System.Collections;

public class ProductionSlot : MonoBehaviour {
	public GameControl gameControl;
	public Sprite NOT_PRODUCING_IMAGE;
	public float costMultiplier;
	public bool producing = false;

	ActionBarButton.ButtonType productionType;
	float timeRemaining = 0f;
	float totalTime = 0f;

	private SpriteRenderer spriteRenderer;

	void Start(){
		spriteRenderer = this.GetComponent<SpriteRenderer> ();
	}

	void Update () {
		if (!producing)
			return;

		//Decremen Time remaining and adjust transparency to show progression
		if (!gameControl.IsPaused ()) {
			timeRemaining -= Time.deltaTime;
		}

		float percentProgress = (float)(totalTime - timeRemaining) / totalTime * .90f + .1f;
		GetComponent<Renderer>().material.color = Color.Lerp(Color.clear, Color.white, percentProgress);

		//End producin if time is up
		if(timeRemaining <= 0) {
			producing = false;
			spriteRenderer.sprite = NOT_PRODUCING_IMAGE;
			spriteRenderer.color = Color.white;

			if (productionType == ActionBarButton.ButtonType.WhiteBloodCellGreen) {
				gameControl.SpawnWhiteBloodCell(WhiteBloodCellType.GREEN);
				gameControl.whiteBloodProduction--;
			}
			else if (productionType == ActionBarButton.ButtonType.WhiteBloodCellPurple) {
				gameControl.SpawnWhiteBloodCell(WhiteBloodCellType.PURPLE);
				gameControl.whiteBloodProduction--;
			}
			else if (productionType == ActionBarButton.ButtonType.WhiteBloodCellTeal) {
				gameControl.SpawnWhiteBloodCell(WhiteBloodCellType.TEAL);
				gameControl.whiteBloodProduction--;
			}
			else if (productionType == ActionBarButton.ButtonType.WhiteBloodCellFinder) {
				gameControl.SpawnWhiteBloodCell(WhiteBloodCellType.FINDER);
				gameControl.whiteBloodProduction--;
			}
			else if (productionType == ActionBarButton.ButtonType.Platelet) {
				gameControl.SpawnPlatelet();
				gameControl.plateletProduction--;
			}
		}
	}

	public void Produce(float time, Sprite image, Color col, ActionBarButton.ButtonType type) {
		producing = true;
		timeRemaining = time;
		totalTime = time;
		spriteRenderer.sprite = image;
		spriteRenderer.color = col;
		productionType = type;
	}
}
