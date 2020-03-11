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
	private int coins = 300;

	public int Coins
	{
		get { return coins; }
	}

	public List<InventoryItem> ChestStorage
	{
		get { return chestStorage; }
	}

	[Inject]
	public void Init(UiController uiController)
	{
		this.uiController = uiController;
		backpack = new Dictionary<int, InventoryItem>();
		chestStorage = new List<InventoryItem>();
		InitBackpack();
		uiController.InitBackpack(backpack);
		uiController.SetCoins(coins);
	}

	private void InitBackpack()
	{
		backpack.Add(0, new InventoryItem(PlayerAction.Plow, ItemName.Pickaxe, 1));
		backpack.Add(1, new InventoryItem(PlayerAction.Water, ItemName.WateringCan, 1));
		backpack.Add(2, new InventoryItem(PlayerAction.Seed, ItemName.Carrot, 2));
		backpack.Add(3, new InventoryItem(PlayerAction.Seed, ItemName.Eggplant, 2));
		backpack.Add(4, new InventoryItem(PlayerAction.Seed, ItemName.Pumpkin, 2));
		//backpack.Add(5, new InventoryItem(PlayerAction.Seed, Item.Tomato, 10));
		//backpack.Add(6, new InventoryItem(PlayerAction.Seed, Item.Turnip, 10));
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

	public InventoryItem GetSelectedItem()
	{
		return backpack.ContainsKey(selectedItem) ? backpack[selectedItem] : null;
	}

	public void AddItem(ItemName itemName, PlayerAction playerAction, int amount)
	{
		bool itemAdded = false;

		foreach (int key in backpack.Keys)
		{
			if (backpack[key].ItemName == itemName && backpack[key].PlayerAction == playerAction)
			{
				backpack[key].Amount += amount;
				uiController.ChangeAmount(key, backpack[key].Amount);
				itemAdded = true;
				break;
			}
		}

		if (!itemAdded)
		{
			InventoryItem inventoryItem = new InventoryItem(playerAction, itemName, amount);
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

	public bool CanCollectItem(ItemName itemName, PlayerAction playerAction)
	{
		if (backpack.Count < backpackSize)
		{
			return true;
		}

		foreach (InventoryItem item in backpack.Values)
		{
			if (item.ItemName == itemName && item.PlayerAction == playerAction)
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

	public void AddCoins(int amount)
	{
		coins += amount;
		uiController.SetCoins(coins);
	}

	public void RemoveCoins(int amount)
	{
		coins -= amount;
		uiController.SetCoins(coins);
	}

	public class InventoryItem
	{
		public PlayerAction PlayerAction;
		public ItemName ItemName;
		public int Amount;

		public InventoryItem(PlayerAction playerAction, ItemName itemName, int amount)
		{
			this.PlayerAction = playerAction;
			this.ItemName = itemName;
			this.Amount = amount;
		} 
	}
}
