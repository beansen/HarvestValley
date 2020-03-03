
using System;
using UnityEngine;

public enum ItemName
{
    Pickaxe,
    WateringCan,
    Carrot,
    Eggplant,
    Pumpkin,
    Tomato,
    Turnip
}

public enum PlayerAction
{
    Plow,
    Water,
    Seed,
    None
}


[Serializable]
public struct ItemBalancingData
{
    public int DaysToGrow;
    public ItemName Type;
    public int AvgSellingPrice;
    public int Price;
}

[Serializable]
public struct ItemGraphicsData
{
    public ItemName Seed;
    public Sprite Sprite;
    public Sprite Collectable;
    public GameObject FarmingPrefab;
}
