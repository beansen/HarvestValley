using System.Collections.Generic;
using Zenject;

public class Inventory
{

	private UiController uiController;
	private Dictionary<int, InventoryItem> backpack;

	private int selectedItem;
	private int freeSlot;

	[Inject]
	public void Init(UiController uiController)
	{
		this.uiController = uiController;
		backpack = new Dictionary<int, InventoryItem>();
		InitBackpack();
		this.uiController.InitBackpack(backpack);
	}

	private void InitBackpack()
	{
		backpack.Add(0, new InventoryItem(PlayerAction.Plow, Seed.None, 1));
		backpack.Add(1, new InventoryItem(PlayerAction.Water, Seed.None, 1));
		backpack.Add(2, new InventoryItem(PlayerAction.Seed, Seed.Carrot, 10));
		backpack.Add(3, new InventoryItem(PlayerAction.Seed, Seed.Eggplant, 10));
		backpack.Add(4, new InventoryItem(PlayerAction.Seed, Seed.Pumpkin, 10));
		backpack.Add(5, new InventoryItem(PlayerAction.Seed, Seed.Tomato, 10));
		backpack.Add(6, new InventoryItem(PlayerAction.Seed, Seed.Turnip, 10));
		freeSlot = backpack.Count;
	}

	public void SelectItem(int slot)
	{
		selectedItem = slot;
	}

	public PlayerAction GetPlayerAction()
	{
		if (backpack.ContainsKey(selectedItem))
		{
			return backpack[selectedItem].PlayerAction;
		}
		
		return PlayerAction.None;
	}

	public Seed GetSeed()
	{
		if (backpack.ContainsKey(selectedItem))
		{
			return backpack[selectedItem].Seed;
		}

		return Seed.None;
	}

	public void RemoveItem(int amount)
	{
		if (backpack.ContainsKey(selectedItem))
		{
			backpack[selectedItem].Amount -= amount;

			if (backpack[selectedItem].Amount <= 0)
			{
				backpack.Remove(selectedItem);
				uiController.RemoveItem(selectedItem);

				if (freeSlot > selectedItem)
				{
					freeSlot = selectedItem;
				}
			}
			else
			{
				uiController.ChangeAmount(selectedItem, backpack[selectedItem].Amount);
			}
		}
	}

	public class InventoryItem
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
