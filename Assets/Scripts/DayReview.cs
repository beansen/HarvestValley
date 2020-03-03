using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class DayReview : MonoBehaviour
{

	public GameObject EndDayScreen;
	public PlayerController Player;
	public GameObject EntryPrefab;
	public DataStore DataStore;

	[Inject] private Inventory inventory;
	[Inject] private FarmingManager farmingManager;

	public void CloseScreen()
	{
		Vector3 pos = transform.position;
		pos.z -= 3;
		pos.y = 0;
		Player.transform.position = pos;
		Player.transform.Rotate(0, 180, 0);
		Player.SetInputEnabled(true);
		EndDayScreen.SetActive(false);
		int total = 0;
		foreach (Inventory.InventoryItem inventoryItem in inventory.ChestStorage)
		{
			if (inventoryItem.PlayerAction == PlayerAction.Seed)
			{
				total += inventoryItem.Amount * DataStore.ItemBalancingData[inventoryItem.ItemName].Price;
			}
			else
			{
				total += inventoryItem.Amount * DataStore.ItemBalancingData[inventoryItem.ItemName].AvgSellingPrice;
			}
		}
		inventory.ChestStorage.Clear();
		inventory.AddCoins(total);
		farmingManager.UpdateFarmPatches();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name.Equals("Character"))
		{
			ShowScreen();
			Player.SetInputEnabled(false);
		}
	}

	private void ShowScreen()
	{
		EndDayScreen.SetActive(true);
		Transform entryContainer = EndDayScreen.transform.GetChild(0).GetChild(1);

		int index = 0;
		int total = 0;

		foreach (Inventory.InventoryItem inventoryItem in inventory.ChestStorage)
		{
			Transform entry;

			if (index >= entryContainer.childCount)
			{
				GameObject go = Instantiate(EntryPrefab, entryContainer, false);
				entry = go.transform;
			}
			else
			{
				entry = entryContainer.GetChild(index);
				entry.gameObject.SetActive(true);
			}

			Sprite itemSprite;
			int price;

			if (inventoryItem.PlayerAction == PlayerAction.Seed)
			{
				itemSprite = DataStore.ItemGraphicsData[inventoryItem.ItemName].Sprite;
				price = inventoryItem.Amount * DataStore.ItemBalancingData[inventoryItem.ItemName].Price;
			}
			else
			{
				itemSprite = DataStore.ItemGraphicsData[inventoryItem.ItemName].Collectable;
				price = inventoryItem.Amount * DataStore.ItemBalancingData[inventoryItem.ItemName].AvgSellingPrice;
			}
			
			total += price;
			
			entry.GetChild(0).GetComponent<Image>().sprite = itemSprite;
			entry.GetChild(1).GetComponent<Text>().text = String.Format("{0} x {1}", inventoryItem.Amount, inventoryItem.ItemName);
			entry.GetChild(2).GetComponent<Text>().text = price.ToString();
			

			index++;
		}

		for (int i = index; i < entryContainer.childCount; i++)
		{
			entryContainer.GetChild(i).gameObject.SetActive(false);
		}

		EndDayScreen.transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<Text>().text = total.ToString();
	}
}
