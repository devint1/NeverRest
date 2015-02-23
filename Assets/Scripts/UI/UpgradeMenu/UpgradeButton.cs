using UnityEngine;
using System.Collections;

using UpgradeTypes;

public class UpgradeButton : MonoBehaviour {

	public CellType cellType;
	public UpgradeType upgradeType;
	public bool Incrementer = true;
	public UpgradeMenu menu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Clicked() {
		switch (cellType) {
		case CellType.Platelet:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				menu.upgradeValues.PlateletCells.MaxCapacity += Incrementer ? 1 : -1;
				break;
			case UpgradeType.FoodPerUnit:
			menu.upgradeValues.PlateletCells.FoodPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.Speed:
			menu.upgradeValues.PlateletCells.Speed += Incrementer ? 1 : -1;
				break;
			}
			break;
		case CellType.Red:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				menu.upgradeValues.RedBloodCell.MaxCapacity += Incrementer ? 1 : -1;
				break;
			case UpgradeType.FoodPerUnit:
				menu.upgradeValues.RedBloodCell.FoodPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.Speed:
				menu.upgradeValues.RedBloodCell.Speed += Incrementer ? 1 : -1;
				break;
			}
			break;
		case CellType.White:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				menu.upgradeValues.WhiteCells.MaxCapacity += Incrementer ? 1 : -1;
				break;
			case UpgradeType.FoodPerUnit:
				menu.upgradeValues.WhiteCells.FoodPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.Speed:
				menu.upgradeValues.WhiteCells.Speed += Incrementer ? 1 : -1;
				break;
			}
			break;
		}
	}
}
