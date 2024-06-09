using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class Items 
{

}











/*
public interface IItem
{
    //string Name {get;set;}
    public string GetName();
    public ItemCategory GetItemCategory();
    public Enum GetItemType();

    public Sprite GetIcon();
}
*/

/*




public class WallItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Wall;
    public Wall itemType;
    public string name;
    public float value;
    public Sprite itemIcon;

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }

    public Sprite GetIcon()
    {
        return null;
    }
}

public class RoofItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Roof;
    public Roof itemType;
    public string name;
    public float value;
    public Sprite itemIcon;

    public Sprite GetIcon()
    {
        return null;
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class FloorItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Floor;
    public Floor itemType;
    public string name;
    public float value;
    public Sprite itemIcon;

    public Sprite GetIcon()
    {
        return null;
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class FishItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Fish;
    public Fish itemType;
    public string name;
    public float value;
    public Sprite itemIcon;
    public float rarity;
    public int spawnDepthStart;
    public int spawnDepthStop;
    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetFishSprite(itemType);
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class OtherItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Other;
    public Other itemType;
    public string name;
    public float value;
    public Sprite itemIcon;
    public float rarity;
    public int spawnDepthStart;
    public int spawnDepthStop;

    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetOtherSprite(itemType);
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class FurnitureItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Furniture;
    public Furniture itemType;
    public string name;
    public float value;
    public Sprite itemIcon;

    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetFurnitureSprite(itemType);
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class MineralItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Mineral;
    public Mineral itemType;
    public string name;
    public float value;
    public Sprite itemIcon;
    public float rarity;
    public int spawnDepthStart;
    public int spawnDepthStop;

    public Tile GetTile()
    {
        return ItemReferences.Instance.GetMineralTile(itemType);
    }

    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetMineralSprite(itemType);
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}

public class CrystalItem : IItem
{
    public readonly ItemCategory itemCategory = ItemCategory.Crystal;
    public Crystal itemType;
    public string name;
    public float value;
    public Sprite itemIcon;
    public float rarity;
    public int spawnDepthStart;
    public int spawnDepthStop;

    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetCrystalSprite(itemType);
    }

    public ItemCategory GetItemCategory()
    {
        return itemCategory;
    }

    public Enum GetItemType()
    {
        return itemType;
    }

    public string GetName()
    {
        return itemType.ToString();
    }
}
*/