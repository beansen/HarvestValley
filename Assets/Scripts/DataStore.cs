using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DataStorage", menuName = "ScriptableObjects/DataStorageScriptableObject", order = 1)]
public class DataStore : ScriptableObject
{

	public ItemBalancingData[] BalancingData;
	public ItemGraphicsData[] GraphicsData;

	private Dictionary<ItemName, ItemBalancingData> itemBalancingData;
	private Dictionary<ItemName, ItemGraphicsData> itemGraphicsData;

	public Dictionary<ItemName, ItemBalancingData> ItemBalancingData
	{
		get
		{
			if (itemBalancingData == null)
			{
				itemBalancingData = new Dictionary<ItemName, ItemBalancingData>();

				foreach (ItemBalancingData balancingData in BalancingData)
				{
					itemBalancingData[balancingData.Type] = balancingData;
				}
			}
			return itemBalancingData;
		}
	}

	public Dictionary<ItemName, ItemGraphicsData> ItemGraphicsData
	{
		get
		{
			if (itemGraphicsData == null)
			{
				itemGraphicsData = new Dictionary<ItemName, ItemGraphicsData>();
				foreach (ItemGraphicsData graphicsData in GraphicsData)
				{
					itemGraphicsData[graphicsData.Seed] = graphicsData;
				}
			}
			return itemGraphicsData;
		}
	}
}
