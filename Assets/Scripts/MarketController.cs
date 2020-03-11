using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MarketController : MonoBehaviour
{

	public GameObject marketScreen, entryPrefab;
	public DataStore dataStore;

	[Inject] private Inventory _inventory;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OpenMarketScreen()
	{
		marketScreen.SetActive(true);
		Transform container = marketScreen.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0);

		int index = 0;
		foreach (KeyValuePair<ItemName,ItemBalancingData> itemBalancingData in dataStore.ItemBalancingData)
		{
			Transform entry;
			
			if (index < container.childCount)
			{
				entry = container.GetChild(index);
			}
			else
			{
				GameObject go = Instantiate(entryPrefab, container);
				entry = go.transform;
			}
			
			entry.GetChild(0).GetComponent<Image>().sprite = dataStore.ItemGraphicsData[itemBalancingData.Key].Sprite;
			entry.GetChild(1).GetComponent<Text>().text = String.Format("{0} Seed", itemBalancingData.Key);
			entry.GetChild(2).GetComponent<Text>().text = itemBalancingData.Value.Price.ToString();

			Button button = entry.GetComponent<Button>();
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(() =>
			{
				if (_inventory.CanCollectItem(itemBalancingData.Key, PlayerAction.Seed) &&
				    _inventory.Coins >= itemBalancingData.Value.Price)
				{
					_inventory.RemoveCoins(itemBalancingData.Value.Price);
					_inventory.AddItem(itemBalancingData.Key, PlayerAction.Seed, 1);
				}
			});

			index++;
		}
	}

	public void CloseMarketScreen()
	{
		marketScreen.SetActive(false);
	}
}
