using UnityEngine;
using System.Collections;

public class UISliders : UnityEngine.UI.Slider {
//public class UISliders : MonoBehaviour {
	public GameControl gameControl;
	public string cellType;


	// Use this for initialization
	void Start () {
		this.onValueChanged.AddListener (changeSliderVal);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void changeSliderVal(float f) {
		if (gameControl != null) {
			if (cellType == "Plate") {
				gameControl.plateletProduction = (int)(f);
			}
			else if (cellType == "RBC") {
				
			}
			else if (cellType == "WBC") {
				gameControl.whiteBloodProduction = (int)(f);
			}
		}
	}
}
