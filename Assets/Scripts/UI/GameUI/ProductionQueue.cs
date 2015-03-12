using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProductionQueue : MonoBehaviour {
	public GameControl gameControl;
	public List<ProductionSlot> productionSlots = new List<ProductionSlot>();
	
	public void QueueItem(float productionTime, float baseCost, Sprite image, ActionBarButton.ButtonType buttonType) {
		ProductionSlot slot = getFirstEmptyProductionSlot ();

		if(slot && baseCost * slot.costMultiplier > gameControl.energy) {
			Debug.Log ("ERROR! NOT ENOUGH ENERGY OR NOT ENOUGH SLOTS! ADD CODE HERE TO INFORM PLAYER!");
			return;
		}

		gameControl.energy -= baseCost * slot.costMultiplier;
		slot.Produce (productionTime, image, buttonType);
	}

	ProductionSlot getFirstEmptyProductionSlot() {
		foreach (ProductionSlot slot in productionSlots) {
			if(!slot.producing)
				return slot;
		}

		return null;
	}
}
