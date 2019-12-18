using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FarmingManager : MonoBehaviour
{

	public GameObject farmPatchPrefab;
	public List<PlantData> PlantBalancingData;
	public Sprite[] Sprites;
	
	[Inject] private Inventory inventory;
	[Inject] private UiController uiController;
	
	private Dictionary<int, FarmingPatch> patches;

	private List<GameObject> farmingPatchesPool;
	private Dictionary<Seed, List<GameObject>> cropsPool;
	
	// Use this for initialization
	void Start () {
		patches = new Dictionary<int, FarmingPatch>();
		farmingPatchesPool = new List<GameObject>();
		cropsPool = new Dictionary<Seed, List<GameObject>>();
	}

	public void Action(PlayerAction type, int x, int y, Seed seed)
	{
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
				patch.SpriteRenderer.gameObject.SetActive(true);
				patch.Data = new PlantData();

				patches.Add(id, patch);
			}
		}

		if (patch != null)
		{
			if (patch.DaysGrowing >= patch.Data.DaysToGrow)
			{
				patch.GameObject.transform.GetChild(0).gameObject.SetActive(false);
				patch.GameObject.transform.GetChild(1).gameObject.SetActive(false);
				addToCropsPool(patch.GameObject, patch.Data.Type);
				uiController.CreateCollectables(patch.Data.Type, patch.GameObject.transform.GetChild(0).position);
				patch.Data = new PlantData();
				patch.GameObject = null;
				patch.DaysGrowing = -1;
				patch.DryPatch();
				return;
			}
			
			if (type == PlayerAction.Water)
			{
				patch.WaterPatch();
			}

			if (type == PlayerAction.Seed)
			{
				if (patch.Data.Type == Seed.None)
				{
					patch.Data = getPlantData(seed);
					patch.SpriteRenderer.sprite = Sprites[1];
					patch.DaysGrowing = 0;
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
			
			if (patch.Data.Type != Seed.None)
			{
				if (patch.Watered)
				{
					patch.DaysGrowing++;
					patch.DryPatch();
				}
				

				if (patch.DaysGrowing == 1 && ReferenceEquals(patch.GameObject, null))
				{
					patch.SpriteRenderer.sprite = Sprites[0];
					patch.GameObject = getCrop(patch.Data);
					patch.GameObject.transform.position = new Vector3(patch.Coordinates.x + 20.5f, 0, patch.Coordinates.y + 20.5f);
					patch.GameObject.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 180f), 0);
				}

				if (patch.DaysGrowing >= patch.Data.DaysToGrow)
				{
					if (patch.Data.Type != Seed.Eggplant && patch.Data.Type != Seed.Tomato)
					{
						patch.GameObject.transform.GetChild(0).gameObject.SetActive(false);
					}
					
					patch.GameObject.transform.GetChild(1).gameObject.SetActive(true);
				}
			}
			else
			{
				patches.Remove(key);
				patch.SpriteRenderer.sprite = Sprites[0];
				patch.DryPatch();
				patch.SpriteRenderer.gameObject.SetActive(false);
				farmingPatchesPool.Add(patch.SpriteRenderer.gameObject);
			}
		}
	}

	public bool CanBeHarvested(int x, int y)
	{
		int id = y * 80 + x;
		
		if (patches.ContainsKey(id))
		{
			FarmingPatch patch = patches[id];
			return patch.DaysGrowing >= patch.Data.DaysToGrow;
		}

		return false;
	}

	private void addToCropsPool(GameObject go, Seed seed)
	{
		if (!cropsPool.ContainsKey(seed))
		{
			cropsPool.Add(seed, new List<GameObject>());
		}
		
		cropsPool[seed].Add(go);
	}

	private GameObject getCrop(PlantData plantData)
	{
		if (cropsPool.ContainsKey(plantData.Type))
		{
			if (cropsPool[plantData.Type].Count > 0)
			{
				GameObject go = cropsPool[plantData.Type][0];
				cropsPool[plantData.Type].RemoveAt(0);
				go.transform.GetChild(0).gameObject.SetActive(true);
				go.transform.GetChild(1).gameObject.SetActive(false);
				return go;
			}
		}

		return Instantiate(plantData.prefab);
	}

	private PlantData getPlantData(Seed seed)
	{
		for (int i = 0; i < PlantBalancingData.Count; i++)
		{
			if (PlantBalancingData[i].Type == seed)
			{
				return PlantBalancingData[i];
			}
		}
		
		return new PlantData();
	}
	
	private class FarmingPatch
	{
		private bool watered;
		private Vector2 coordinates;
		
		public SpriteRenderer SpriteRenderer;
		public int DaysGrowing = -1;
		public PlantData Data;
		public GameObject GameObject;

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

[Serializable]
public struct PlantData
{
	public int DaysToGrow;
	public Seed Type;
	public int AvgSellingPrice;
	public int Price;
	public GameObject prefab;
}

public enum Seed
{
	None,
	Carrot,
	Eggplant,
	Pumpkin,
	Tomato,
	Turnip
}