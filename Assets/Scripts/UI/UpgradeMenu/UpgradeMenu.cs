using UnityEngine;
using System.Collections;

using UpgradeTypes;

namespace UpgradeTypes
{
	public enum CellType{ Red, White, Platelet };
	
	public enum UpgradeType{ EneryPerUnit, BuildSpeed, MoveSpeed };

	public struct CellUpgrades
	{
		public int EneryPerUnit;
		public int BuildSpeed;
		public int MoveSpeed;
	}
	
	public struct Cells
	{
		public CellUpgrades RedBloodCell;
		public CellUpgrades WhiteCells;
		public CellUpgrades PlateletCells;
	}
}

public class UpgradeMenu : MonoBehaviour {

	public Cells upgradeValues;
	public GameControl gc;

	// Use this for initialization
	void Start () {
		upgradeValues.RedBloodCell.EneryPerUnit = 0;
		upgradeValues.RedBloodCell.BuildSpeed = 0;
		upgradeValues.RedBloodCell.MoveSpeed = 0;

		upgradeValues.WhiteCells.EneryPerUnit = 0;
		upgradeValues.WhiteCells.BuildSpeed = 0;
		upgradeValues.WhiteCells.MoveSpeed = 0;

		upgradeValues.PlateletCells.EneryPerUnit = 0;
		upgradeValues.PlateletCells.BuildSpeed = 0;
		upgradeValues.PlateletCells.MoveSpeed = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



}
