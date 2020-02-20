using System.Collections.Generic;
using Zenject;

public class Inventory
{

	private UiController uiController;
	private Dictionary<int, InventoryItem> backpack;
	private List<InventoryItem> chestStorage;

	private int selectedItem;
	private int freeSlot;
	private int backpackSize = 10;

	[Inject]
	public void Init(UiController uiController)
	{
		this.uiController = uiController;
		backpack = new Dictionary<int, InventoryItem>();
		chestStorage = new List<InventoryItem>();
		InitBackpack();
		this.uiController.InitBackpack(backpack);
	}

	private void InitBackpack()
	{
		backpack.Add(0, new InventoryItem(PlayerAction.Plow, Seed.None, 1));
		backpack.Add(1, new InventoryItem(PlayerAction.Water, Seed.None, 1));
		backpack.Add(2, new InventoryItem(PlayerAction.Seed, Seed.Carrot, 2));
		backpack.Add(3, new InventoryItem(PlayerAction.Seed, Seed.Eggplant, 2));
		backpack.Add(4, new InventoryItem(PlayerAction.Seed, Seed.Pumpkin, 2));
		//backpack.Add(5, new InventoryItem(PlayerAction.Seed, Seed.Tomato, 10));
		//backpack.Add(6, new InventoryItem(PlayerAction.Seed, Seed.Turnip, 10));
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
		return backpack.ContainsKey(selectedItem) ? backpack[selectedItem].Seed : Seed.None;
	}

	public void AddItem(Seed seed, PlayerAction playerAction, int amount)
	{
		bool itemAdded = false;

		foreach (int key in backpack.Keys)
		{
			if (backpack[key].Seed == seed && backpack[key].PlayerAction == playerAction)
			{
				backpack[key].Amount += amount;
				uiController.ChangeAmount(key, backpack[key].Amount);
				itemAdded = true;
				break;
			}
		}

		if (!itemAdded)
		{
			InventoryItem inventoryItem = new InventoryItem(playerAction, seed, amount);
			backpack.Add(freeSlot, inventoryItem);
			uiController.AddItem(freeSlot, inventoryItem);

			if (backpack.Count == backpackSize)
			{
				freeSlot = backpackSize;
			}
			else
			{
				for (int i = freeSlot + 1; i < backpackSize; i++)
				{
					if (!backpack.ContainsKey(i))
					{
						freeSlot = i;
						break;
					}
				}
			}
		}
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

	public bool CanCollectItem(Seed seed, PlayerAction playerAction)
	{
		if (backpack.Count < backpackSize)
		{
			return true;
		}

		foreach (InventoryItem item in backpack.Values)
		{
			if (item.Seed == seed && item.PlayerAction == playerAction)
			{
				return true;
			}
		}

		return false;
	}

	public bool TransferItemToChest(int index)
	{
		if (backpack.ContainsKey(index))
		{
			if (backpack[index].PlayerAction == PlayerAction.Seed || backpack[index].PlayerAction == PlayerAction.None)
			{
				InventoryItem item = backpack[index];
				backpack.Remove(index);
				if (index < freeSlot)
				{
					freeSlot = index;
				}
				
				chestStorage.Add(item);

				return true;
			}
		}

		return false;
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

/*
 * Selected item - left click puts into chest
 * Right click on chest puts up inventory - left click puts into chest
 */
