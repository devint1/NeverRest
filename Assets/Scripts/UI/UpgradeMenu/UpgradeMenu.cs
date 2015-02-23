using UnityEngine;
using System.Collections;

using UpgradeTypes;

namespace UpgradeTypes
{
	public enum CellType{ Red, White, Platelet };
	
	public enum UpgradeType{ FoodPerUnit, Capacity, Speed };

	public struct CellUpgrades
	{
		public int FoodPerUnit;
		public int MaxCapacity;
		public int Speed;
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

	// Use this for initialization
	void Start () {
		upgradeValues.RedBloodCell.FoodPerUnit = 0;
		upgradeValues.RedBloodCell.MaxCapacity = 0;
		upgradeValues.RedBloodCell.Speed = 0;

		upgradeValues.WhiteCells.FoodPerUnit = 0;
		upgradeValues.WhiteCells.MaxCapacity = 0;
		upgradeValues.WhiteCells.Speed = 0;

		upgradeValues.PlateletCells.FoodPerUnit = 0;
		upgradeValues.PlateletCells.MaxCapacity = 0;
		upgradeValues.PlateletCells.Speed = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}



}
