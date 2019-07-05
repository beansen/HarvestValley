using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

	private List<InventoryItem> backpack;

	private int selectedItem = 0;
	private int freeSlot;

	// Use this for initialization
	void Start () {
		backpack = new List<InventoryItem>();
		InitBackpack();
	}

	private void InitBackpack()
	{
		backpack.Add(new InventoryItem(PlayerAction.Plow, Seed.None, 1));
		backpack.Add(new InventoryItem(PlayerAction.Water, Seed.None, 1));
		backpack.Add(new InventoryItem(PlayerAction.Seed, Seed.Carrot, 1));
		backpack.Add(new InventoryItem(PlayerAction.Seed, Seed.Eggplant, 1));
		backpack.Add(new InventoryItem(PlayerAction.Seed, Seed.Pumpkin, 1));
		backpack.Add(new InventoryItem(PlayerAction.Seed, Seed.Tomato, 1));
		backpack.Add(new InventoryItem(PlayerAction.Seed, Seed.Turnip, 1));
		freeSlot = backpack.Count;
	}

	public void SelectItem(int slot)
	{
		selectedItem = slot;
	}

	public PlayerAction GetPlayerAction()
	{
		if (selectedItem < backpack.Count && backpack[selectedItem] != null)
		{
			return backpack[selectedItem].PlayerAction;
		}
		else
		{
			return PlayerAction.None;
		}
	}

	public Seed GetSeed()
	{
		if (selectedItem < backpack.Count && backpack[selectedItem] != null)
		{
			return backpack[selectedItem].Seed;
		}
		else
		{
			return Seed.None;
		}
	}

	private class InventoryItem
	{
		public PlayerAction PlayerAction;
		public Seed Seed;
		public int Amount;

		public InventoryItem(PlayerAction playerAction, Seed seed, int amount)
		{
			this.PlayerAction = playerAction;
			this.Seed = seed;
			this.Amount = amount;
		} 
	}
}

public enum ItemType
{
	Hoe,
	WateringCan,
	Seed,
	Sellable
}
