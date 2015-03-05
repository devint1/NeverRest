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
			case UpgradeType.EneryPerUnit:
				menu.upgradeValues.PlateletCells.EneryPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.BuildSpeed:
				menu.upgradeValues.PlateletCells.BuildSpeed += Incrementer ? 1 : -1;
				break;
			case UpgradeType.MoveSpeed:
				menu.upgradeValues.PlateletCells.MoveSpeed += Incrementer ? 1 : -1;
				break;
			}
			break;
		case CellType.Red:
			switch (upgradeType) {
			case UpgradeType.EneryPerUnit:
				menu.upgradeValues.RedBloodCell.EneryPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.BuildSpeed:
				menu.upgradeValues.RedBloodCell.BuildSpeed += Incrementer ? 1 : -1;
				break;
			case UpgradeType.MoveSpeed:
				menu.upgradeValues.RedBloodCell.MoveSpeed += Incrementer ? 1 : -1;
				break;
			}
			break;
		case CellType.White:
			switch (upgradeType) {
			case UpgradeType.EneryPerUnit:
				menu.upgradeValues.WhiteCells.EneryPerUnit += Incrementer ? 1 : -1;
				break;
			case UpgradeType.BuildSpeed:
				menu.upgradeValues.WhiteCells.BuildSpeed += Incrementer ? 1 : -1;
				break;
			case UpgradeType.MoveSpeed:
				menu.upgradeValues.WhiteCells.MoveSpeed += Incrementer ? 1 : -1;
				break;
			}
			break;
		}
	}
}
