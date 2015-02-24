using UnityEngine;
//using UnityEngine.UI;
using System.Collections;

using UpgradeTypes;

public class UpgradeText : MonoBehaviour {

	public UpgradeMenu menu;
	public CellType cellType;
	public UpgradeType upgradeType;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		int value = 0;

		switch (cellType) {
		case CellType.Platelet:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				value = menu.upgradeValues.PlateletCells.MaxCapacity;
				break;
			case UpgradeType.FoodPerUnit:
				value = menu.upgradeValues.PlateletCells.FoodPerUnit;
				break;
			case UpgradeType.Speed:
				value = menu.upgradeValues.PlateletCells.Speed;
				break;
			}
			break;
		case CellType.Red:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				value = menu.upgradeValues.RedBloodCell.MaxCapacity;
				break;
			case UpgradeType.FoodPerUnit:
				value = menu.upgradeValues.RedBloodCell.FoodPerUnit;
				break;
			case UpgradeType.Speed:
				value = menu.upgradeValues.RedBloodCell.Speed;
				break;
			}
			break;
		case CellType.White:
			switch (upgradeType) {
			case UpgradeType.Capacity:
				value = menu.upgradeValues.WhiteCells.MaxCapacity;
				break;
			case UpgradeType.FoodPerUnit:
				value = menu.upgradeValues.WhiteCells.FoodPerUnit;
				break;
			case UpgradeType.Speed:
				value = menu.upgradeValues.WhiteCells.Speed;
				break;
			}
			break;
		}
		//GetComponent<Text> ().text = value.ToString ();
	}
}
