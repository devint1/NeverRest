using UnityEngine;
using UnityEngine.UI;
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
			case UpgradeType.EneryPerUnit:
				value = menu.upgradeValues.PlateletCells.EneryPerUnit;
				break;
			case UpgradeType.BuildSpeed:
				value = menu.upgradeValues.PlateletCells.BuildSpeed;
				break;
			case UpgradeType.MoveSpeed:
				value = menu.upgradeValues.PlateletCells.MoveSpeed;
				break;
			}
			break;
		case CellType.Red:
			switch (upgradeType) {
			case UpgradeType.EneryPerUnit:
				value = menu.upgradeValues.RedBloodCell.EneryPerUnit;
				break;
			case UpgradeType.BuildSpeed:
				value = menu.upgradeValues.RedBloodCell.BuildSpeed;
				break;
			case UpgradeType.MoveSpeed:
				value = menu.upgradeValues.RedBloodCell.MoveSpeed;
				break;
			}
			break;
		case CellType.White:
			switch (upgradeType) {
			case UpgradeType.EneryPerUnit:
				value = menu.upgradeValues.WhiteCells.EneryPerUnit;
				break;
			case UpgradeType.BuildSpeed:
				value = menu.upgradeValues.WhiteCells.BuildSpeed;
				break;
			case UpgradeType.MoveSpeed:
				value = menu.upgradeValues.WhiteCells.MoveSpeed;
				break;
			}
			break;
		}
		GetComponent<Text> ().text = value.ToString ();
	}
}
