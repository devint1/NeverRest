using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductionQueue : MonoBehaviour {
	public GameControl gameControl;
	public List<ProductionSlot> productionSlots = new List<ProductionSlot>();

	Color[] colors = { new Color(0.49f, 0.92f, 0.34f), new Color(0.84f, 0.55f, 0.99f), new Color(0.20f, 0.70f, 1.0f), new Color(0.41f, 0.42f, 0.57f), new Color(1.0f, 1.0f, 1.0f) };
	
	public void QueueItem(float productionTime, float baseCost, Sprite image, ActionBarButton.ButtonType buttonType) {
		ProductionSlot slot = getFirstEmptyProductionSlot ();

		if(slot && baseCost * slot.costMultiplier >= gameControl.energy) {
			//Debug.Log ("ERROR! NOT ENOUGH ENERGY OR NOT ENOUGH SLOTS! ADD CODE HERE TO INFORM PLAYER!");
			return;
		}

		if (buttonType == ActionBarButton.ButtonType.WhiteBloodCellGreen
		    || buttonType == ActionBarButton.ButtonType.WhiteBloodCellPurple
		    || buttonType == ActionBarButton.ButtonType.WhiteBloodCellTeal
		    || buttonType == ActionBarButton.ButtonType.WhiteBloodCellFinder) {
				gameControl.whiteBloodProduction++;
		} else if (buttonType == ActionBarButton.ButtonType.Platelet) {
				gameControl.plateletProduction++;
		}
		//Debug.Log ("Energy " + gameControl.energy);
		if (slot && baseCost * slot.costMultiplier < gameControl.energy) {
			gameControl.energy -= baseCost * slot.costMultiplier;
			slot.Produce (productionTime, image, colors[(int)buttonType], buttonType);
		}
	}

	ProductionSlot getFirstEmptyProductionSlot() {
		foreach (ProductionSlot slot in productionSlots) {
			if(!slot.producing)
				return slot;
		}

		return null;
	}
}
