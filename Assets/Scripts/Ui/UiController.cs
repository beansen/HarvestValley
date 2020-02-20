using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Random = UnityEngine.Random;

public class UiController : MonoBehaviour
{

	public Sprite[] Borders;
	public Transform ItemsPanel, MainPanel;
	public ItemSprite[] ItemSprites;
	public GameObject collectablePrefab;
	public Transform Player;

	private List<CollectableCrops> collectableCropsList;

	[Inject]
	private Inventory inventory;

	private int currentSelection;

	private Image[] borderImages;
	private Image[] foregroundImages;
	private Text[] itemTexts;

	public bool ChestMode { get; set; }
	
	void Update()
	{
		if (collectableCropsList != null)
		{
			for (int i = collectableCropsList.Count - 1; i >= 0; i--)
			{
				collectableCropsList[i].Update(Player.position);

				if (collectableCropsList[i].Collectable)
				{
					if (inventory.CanCollectItem(collectableCropsList[i].Seed, PlayerAction.None)
					    && Vector3.Distance(collectableCropsList[i].WorldPosition, Player.position) <= 2)
					{
						collectableCropsList[i].SetCollected();
						inventory.AddItem(collectableCropsList[i].Seed, PlayerAction.None, 4);
					}
				}

				if (collectableCropsList[i].Deactivated)
				{
					collectableCropsList.RemoveAt(i);
				}
			}
		}
	}

	public void SwitchSelection(int index)
	{
		if (index != currentSelection)
		{
			borderImages[currentSelection].sprite = Borders[0];
			borderImages[index].sprite = Borders[1];
			currentSelection = index;
		}
	}

	public void ChangeAmount(int index, int newAmount)
	{
		itemTexts[index].text = newAmount.ToString();
	}

	public void RemoveItem(int index)
	{
		foregroundImages[index].gameObject.SetActive(false);
		itemTexts[index].text = String.Empty;
	}

	public void AddItem(int index, Inventory.InventoryItem item)
	{
		foregroundImages[index].gameObject.SetActive(true);
		foregroundImages[index].sprite = item.PlayerAction == PlayerAction.None ? GetCollectableSprite(item.Seed) : GetSprite(item);
		itemTexts[index].text = item.Amount > 1 ? item.Amount.ToString() : String.Empty;
	}

	public void CreateCollectables(Seed seed, Vector3 start)
	{
		CollectableCrops collectable = new CollectableCrops(seed, start);
		
		for (int i = 0; i < 4; i++)
		{
			Vector3 pos = start;
			pos.z += Random.Range(-1f, 1f);

			GameObject go = Instantiate(collectablePrefab, MainPanel);
			go.GetComponent<Image>().sprite = GetCollectableSprite(seed);
			collectable.AddItem(go, pos);
		}
		
		collectableCropsList.Add(collectable);
	}

	public void InitBackpack(Dictionary<int, Inventory.InventoryItem> backpack)
	{
		collectableCropsList = new List<CollectableCrops>();
		int childCount = ItemsPanel.childCount;
		borderImages = new Image[childCount];
		foregroundImages = new Image[childCount];
		itemTexts = new Text[childCount];

		for (int i = 0; i < childCount; i++)
		{
			borderImages[i] = ItemsPanel.GetChild(i).GetComponent<Image>();
			foregroundImages[i] = ItemsPanel.GetChild(i).GetChild(0).GetComponent<Image>();
			itemTexts[i] = ItemsPanel.GetChild(i).GetChild(1).GetComponent<Text>();
			int index = i;
			
			ItemsPanel.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
			{
				inventory.SelectItem(index);
				SwitchSelection(index);

				if (ChestMode)
				{
					bool transferred = inventory.TransferItemToChest(index);
					if (transferred)
						RemoveItem(index);
				}
			});

			if (i != currentSelection)
			{
				borderImages[i].sprite = Borders[0];
			}

			if (backpack.ContainsKey(i))
			{
				AddItem(i, backpack[i]);
			}
			else
			{
				RemoveItem(i);
			}
		}
	}

	private Sprite GetSprite(Inventory.InventoryItem item)
	{
		if (item.PlayerAction == PlayerAction.Plow)
		{
			return ItemSprites[0].Sprite;
		}
		
		if (item.PlayerAction == PlayerAction.Water)
		{
			return ItemSprites[1].Sprite;
		}

		for (int i = 2; i < ItemSprites.Length; i++)
		{
			if (ItemSprites[i].Seed == item.Seed)
			{
				return ItemSprites[i].Sprite;
			}
		}
		

		return null;
	}
	
	private Sprite GetCollectableSprite(Seed seed)
	{
		for (int i = 2; i < ItemSprites.Length; i++)
		{
			if (ItemSprites[i].Seed == seed)
			{
				return ItemSprites[i].Collectable;
			}
		}
		

		return null;
	}
	
	[Serializable]
	public struct ItemSprite
	{
		public PlayerAction PlayerAction;
		public Seed Seed;
		public Sprite Sprite;
		public Sprite Collectable;
	}
}
