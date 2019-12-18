using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Object = System.Object;
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

	private int currentSelection = 0;

	private Image[] borderImages;
	private Image[] foregroundImages;
	private Text[] itemTexts;
	
	void Update()
	{
		if (collectableCropsList != null)
		{
			foreach (CollectableCrops crops in collectableCropsList)
			{
				crops.Update(Camera.main.WorldToScreenPoint(Player.position));
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
		foregroundImages[index].sprite = GetSprite(item);
		itemTexts[index].text = item.Amount > 1 ? item.Amount.ToString() : String.Empty;
	}

	public void CreateCollectables(Seed seed, Vector3 start)
	{
		CollectableCrops collectable = new CollectableCrops();
		
		for (int i = 0; i < 4; i++)
		{
			Vector3 pos = start;
			pos.y += Random.Range(0, 2f);

			GameObject go = Instantiate(collectablePrefab, MainPanel, true);
			go.transform.position = Camera.main.WorldToScreenPoint(pos);
			collectable.AddItem(go);
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
	
	private Sprite GetCollectableSprite(Inventory.InventoryItem item)
	{
		for (int i = 2; i < ItemSprites.Length; i++)
		{
			if (ItemSprites[i].Seed == item.Seed)
			{
				return ItemSprites[i].Sprite;
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
	
	private class CollectableCrops
	{
		private float timer;
		private List<GameObject> gameObjects;
		private List<ObjectData> objects;
		private bool floatToPlayer;

		public void Update(Vector3 playerPosition)
		{
			if (floatToPlayer)
			{
				
			}
			else
			{
				timer += Time.deltaTime * 4;
				float sin = Mathf.Sin(timer);

				for (int i = 0; i < gameObjects.Count; i++)
				{
					Vector3 newPos = gameObjects[i].transform.position;
					newPos.x += objects[i].Direction;
					newPos.y = objects[i].StartPos + (50 * sin);
					gameObjects[i].transform.position = newPos;
				}

				floatToPlayer = sin <= 0;
			}
		}

		public void AddItem(GameObject gameObject)
		{
			if (objects == null)
			{
				objects = new List<ObjectData>();
				gameObjects = new List<GameObject>();
			}

			int direction = Random.Range(0, 2) == 0 ? -1 : 1;
			objects.Add(new ObjectData(direction, gameObject.transform.position.y));
			gameObjects.Add(gameObject);
		}

		public List<GameObject> GetGameObjects()
		{
			return gameObjects;
		}
		
		private struct ObjectData
		{
			public int Direction;
			public float StartPos;

			public ObjectData(int direction, float startPos)
			{
				this.Direction = direction;
				this.StartPos = startPos;
			}
		}
	}
}
