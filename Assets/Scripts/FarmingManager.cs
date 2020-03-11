using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FarmingManager : MonoBehaviour
{

	public GameObject farmPatchPrefab;
	public DataStore DataStore;
	public Sprite[] Sprites;

	[Inject] private Inventory inventory;
	[Inject] private UiController uiController;
	
	private Dictionary<int, FarmingPatch> patches;

	private List<GameObject> farmingPatchesPool;
	private Dictionary<ItemName, List<GameObject>> cropsPool;
	
	// Use this for initialization
	void Start () {
		patches = new Dictionary<int, FarmingPatch>();
		farmingPatchesPool = new List<GameObject>();
		cropsPool = new Dictionary<ItemName, List<GameObject>>();
	}

	public void Action(PlayerAction type, int x, int y, ItemName item)
	{
		if (x < 0 || y < 0 || x > 78 || y > 78)
		{
			return;
		}

		if (x > 52 && y > 65)
		{
			return;
		}
		
		int id = y * 80 + x;

		FarmingPatch patch = null;

		if (patches.ContainsKey(id))
		{
			patch = patches[id];
		}
		else
		{
			if (type == PlayerAction.Plow)
			{
				patch = new FarmingPatch(x, y);
				GameObject go;

				if (farmingPatchesPool.Count > 0)
				{
					go = farmingPatchesPool[0];
					farmingPatchesPool.RemoveAt(0);
				}
				else
				{
					go = Instantiate(farmPatchPrefab);
				}
				
				go.transform.position = new Vector3(x + 20.5f, 0.01f, y + 20.5f);
				patch.SpriteRenderer = go.GetComponent<SpriteRenderer>();
				patch.SpriteRenderer.sprite = Sprites[0];
				patch.SpriteRenderer.gameObject.SetActive(true);

				patches.Add(id, patch);
			}
		}

		if (patch != null)
		{
			if (CanBeHarvested(x, y))
			{
				patch.DryPatch();

				GameObject cropsGameObject = patch.GameObject;
				cropsGameObject.transform.GetChild(0).gameObject.SetActive(false);
				cropsGameObject.transform.GetChild(1).gameObject.SetActive(false);
				AddToCropsPool(cropsGameObject, patch.Seed);
				
				patch.GameObject = null;
				patch.DaysGrowing = -1;
				patch.PatchState = PlayerAction.Plow;

				uiController.CreateCollectables(patch.Seed, cropsGameObject.transform.GetChild(0).position);
				return;
			}
			
			if (type == PlayerAction.Water)
			{
				patch.WaterPatch();
			}

			if (type == PlayerAction.Seed)
			{
				if (patch.PatchState == PlayerAction.Plow)
				{
					patch.Seed = item;
					patch.SpriteRenderer.sprite = Sprites[1];
					patch.DaysGrowing = 0;
					patch.PatchState = PlayerAction.Seed;
					inventory.RemoveItem(1);
				}
			}
		}
	}

	public void UpdateFarmPatches()
	{
		List<int> keys = new List<int>(patches.Keys);

		foreach (int key in keys)
		{
			FarmingPatch patch = patches[key];
			
			if (patch.PatchState == PlayerAction.Seed)
			{
				if (patch.Watered)
				{
					patch.DaysGrowing++;
					patch.DryPatch();
				}

				if (patch.DaysGrowing == 1 && ReferenceEquals(patch.GameObject, null))
				{
					patch.SpriteRenderer.sprite = Sprites[0];
					patch.GameObject = GetCrop(patch.Seed);
					patch.GameObject.transform.position = new Vector3(patch.Coordinates.x + 20.5f, 0, patch.Coordinates.y + 20.5f);
					patch.GameObject.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 180f), 0);
				}

				if (patch.DaysGrowing >= DataStore.ItemBalancingData[patch.Seed].DaysToGrow)
				{
					if (patch.Seed != ItemName.Eggplant && patch.Seed != ItemName.Tomato)
					{
						patch.GameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
					
					patch.GameObject.transform.GetChild(1).gameObject.SetActive(true);
				}
			}
			else
			{
				patch.DryPatch();
				GameObject farmingPatchGameObject = patch.SpriteRenderer.gameObject;
				farmingPatchGameObject.SetActive(false);
				farmingPatchesPool.Add(farmingPatchGameObject);
				patches.Remove(key);
			}
		}
	}

	public bool CanBeHarvested(int x, int y)
	{
		int id = y * 80 + x;
		
		if (patches.ContainsKey(id))
		{
			FarmingPatch patch = patches[id];
			if (patch.PatchState == PlayerAction.Seed)
				return patch.DaysGrowing >= DataStore.ItemBalancingData[patch.Seed].DaysToGrow;
		}

		return false;
	}

	private void AddToCropsPool(GameObject go, ItemName seed)
	{
		if (!cropsPool.ContainsKey(seed))
		{
			cropsPool.Add(seed, new List<GameObject>());
		}
		
		cropsPool[seed].Add(go);
	}

	private GameObject GetCrop(ItemName seed)
	{
		if (cropsPool.ContainsKey(seed))
		{
			if (cropsPool[seed].Count > 0)
			{
				GameObject go = cropsPool[seed][0];
				cropsPool[seed].RemoveAt(0);
				go.transform.GetChild(0).gameObject.SetActive(true);
				go.transform.GetChild(1).gameObject.SetActive(false);
				return go;
			}
		}

		return Instantiate(DataStore.ItemGraphicsData[seed].FarmingPrefab);
	}

	private class FarmingPatch
	{
		private bool watered;
		private Vector2 coordinates;
		
		public SpriteRenderer SpriteRenderer;
		public int DaysGrowing = -1;
		public GameObject GameObject;
		public ItemName Seed;
		public PlayerAction PatchState = PlayerAction.Plow;

		public bool Watered
		{
			get { return watered; }
		}
		
		public Vector2 Coordinates
		{
			get { return coordinates; }
		}

		public FarmingPatch(int x, int y)
		{
			coordinates = new Vector2(x, y);
		}
		
		public void WaterPatch()
		{
			watered = true;
			SpriteRenderer.color = new Color(153 / 255f, 153 / 255f, 153 / 255f, 230 / 255f);
		}

		public void DryPatch()
		{
			watered = false;
			SpriteRenderer.color = new Color(1, 1, 1, 230 / 255f);
		}
	}
}