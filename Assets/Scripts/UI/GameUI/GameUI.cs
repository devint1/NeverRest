using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameUI : MonoBehaviour {
	
	public Text energyStatus;
	public float maxEnergy;
	public GameObject WBCImage;
	public GameObject PCImage;
	public GameControl gC;

	float currentEnergy;
	GameControl control;

	int numWBCBuilding;
	int numPCBuilding;

	ArrayList WBCSlots;
	ArrayList PCSlots;

	// Use this for initialization
	void Start () {
		StartCoroutine(IncrementEnergy());
		currentEnergy = 100;
		WBCSlots = new ArrayList ();
		PCSlots = new ArrayList ();
	}
	
	// Update is called once per frame
	void Update () {
		energyStatus.text = (int)currentEnergy + "/" + maxEnergy;
		if (Input.GetKeyDown (KeyCode.Q)) {
			numWBCBuilding += 1;
			StartCoroutine(GenerateWBC());
		}
		else if (Input.GetKeyDown (KeyCode.W)) {
			numPCBuilding += 1;
			StartCoroutine(GeneratePC());
		}
	}

	IEnumerator IncrementEnergy() {
		yield return new WaitForSeconds (.05f);
		if (currentEnergy < maxEnergy) {
			currentEnergy += 1;
		}
		StartCoroutine (IncrementEnergy ());
	}

	IEnumerator GenerateWBC(){
		float cost = numWBCBuilding * 2;
		double progress = 0;
		int ndx;
		for (ndx = 0; ndx <= WBCSlots.Count; ndx++) {
			if (ndx == WBCSlots.Count){
				WBCSlots.Add(1);
				break;
			}
			else if (WBCSlots[ndx].Equals(0)){
				WBCSlots[ndx] = 1;
				break;
			}
		}

		GameObject pic = (GameObject) Instantiate (WBCImage);
		pic.transform.parent = transform;
		Vector3 pos = pic.transform.position;
		pos.y += Screen.height * ( .175f * (ndx + 1) );
		pic.transform.position = pos;

		while (progress != 100) {
			Color c = pic.GetComponent<Image>().color;
			c.a = (float) progress * .01f;
			pic.GetComponent<Image>().color = c;
			if (currentEnergy > cost){
				currentEnergy -= cost;
				progress+=2.5;
			}
			yield return new WaitForSeconds (.125f);
		}
		Destroy (pic);
		numWBCBuilding -= 1;
		WBCSlots [ndx] = 0;
		gC.SpawnWhiteBloodCell();
	}

	IEnumerator GeneratePC(){
		float cost = numPCBuilding * 2;
		double progress = 0;
		int ndx;
		for (ndx = 0; ndx <= PCSlots.Count; ndx++) {
			if (ndx == PCSlots.Count){
				PCSlots.Add(1);
				break;
			}
			else if (PCSlots[ndx].Equals(0)){
				PCSlots[ndx] = 1;
				break;
			}
		}

		GameObject pic = (GameObject) Instantiate (PCImage);
		pic.transform.parent = transform;
		Vector3 pos = pic.transform.position;
		pos.y += Screen.height * ( .175f * (ndx + 1) );
		pic.transform.position = pos;

		while (progress != 100) {
			Color c = pic.GetComponent<Image>().color;
			c.a = (float) progress * .01f;
			pic.GetComponent<Image>().color = c;
			if (currentEnergy > cost){
				currentEnergy -= cost;
				progress+=2.5;
			}
			yield return new WaitForSeconds (.125f);
		}
		Destroy (pic);
		numPCBuilding -= 1;
		PCSlots [ndx] = 0;
		gC.SpawnPlatelet();
	}
}
