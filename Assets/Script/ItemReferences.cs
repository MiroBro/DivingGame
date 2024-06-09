using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.Progress;

public class ItemReferences : MonoBehaviour
{
    private static ItemReferences _instance;
    public static ItemReferences Instance { get { return _instance; } }
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public Tile waterTile;

    public static bool IsFish(Item type)
    {
        return type.GetItemCategory() == ItemCategory.Fish;
    }

    public Sprite[] allMineralsSprites;
    public Tile[] allMineralsTiles;
    public Sprite[] allCrystalsSprites;
    public Sprite[] allFishesSprites;
    public Sprite[] allFurnituresSprites;
    public Sprite[] allCatsSprites;
    public Sprite[] allUnderwaterPlantsIcons;
    public Sprite[] allUnderwaterPlantsSprites;
    public Sprite[] allLandPlantsIcons;
    public Sprite[] allLandPlantsSprites;
    public Sprite[] allOthersSprites;
    public Sprite[] allChestSprites;
    public Sprite[] allSeaCreatureSprites;
    public Sprite[] allWallSprites;
    public Sprite[] allRoofSprites;
    public Sprite[] allFloorsSprites;

    public static readonly List<Item> allFurniture = new List<Item>()
    {
        new Item(){itemType = Furniture.Fireplace, value = 10},

    };

    public static readonly List<Item> allOther = new List<Item>()
    {
        new Item(){itemCategory = ItemCategory.Other,itemType = Other.Seaweed,      value = 10, rarity = 0.3f,     spawnDepthStart = 0,    spawnDepthStop = -1000},
        new Item(){itemCategory = ItemCategory.Other,itemType = Other.WaterWood,    value = 10, rarity = 0.4f,     spawnDepthStart = 0,    spawnDepthStop = -5000},
        new Item(){itemCategory = ItemCategory.Other,itemType = Other.WaterRoot,    value = 10, rarity = 0.1f,     spawnDepthStart = -300,    spawnDepthStop = -5000},
        new Item(){itemCategory = ItemCategory.Other,itemType = Other.Sand,         value = 10, rarity = 0.2f,     spawnDepthStart = -300,    spawnDepthStop = -5000},
        new Item(){itemCategory = ItemCategory.Other,itemType = Other.WaterAcorn,   value = 10, rarity = 0.12f,    spawnDepthStart = -300,    spawnDepthStop = -5000},

    };

    public static readonly List<Item> allMineral = new List<Item>()
    {
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Amethyst,      value = 10, rarity = 0.21f,     spawnDepthStart = (int) (-2500 * StaticValues.mineralProgressionSpeed),   spawnDepthStop = (int )(-2900* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Ammolite,      value = 10, rarity = 0.2f,      spawnDepthStart = (int) (-1200* StaticValues.mineralProgressionSpeed),    spawnDepthStop = (int )(-1700* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.AuraQuartz,    value = 10, rarity = 0.22f,     spawnDepthStart = (int) (-2800* StaticValues.mineralProgressionSpeed),    spawnDepthStop = (int )(-3600* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Iron,          value = 10, rarity = 0.35f,     spawnDepthStart = (int) (-1600* StaticValues.mineralProgressionSpeed),    spawnDepthStop = (int )(-2300* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Labradorite,   value = 10, rarity = 0.24f,     spawnDepthStart = (int) (-800* StaticValues.mineralProgressionSpeed),     spawnDepthStop = (int )(-1200* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Moonstone,     value = 10, rarity = 0.18f,     spawnDepthStart = (int) (-2200* StaticValues.mineralProgressionSpeed),    spawnDepthStop = (int )(-2600* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Stone,         value = 10, rarity = 1f,        spawnDepthStart = (int) (Math.Min(-3 * StaticValues.mineralProgressionSpeed,-3)),      spawnDepthStop = (int )(-5000* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.SunsetOpal,    value = 10, rarity = 0.18f,     spawnDepthStart = (int) (-3200* StaticValues.mineralProgressionSpeed),    spawnDepthStop = (int )(-4000* StaticValues.mineralProgressionSpeed)},
        new Item(){itemCategory = ItemCategory.Mineral,itemType = Mineral.Mud,           value = 10, rarity = 0.2f,      spawnDepthStart = (int) (Math.Min(-3 * StaticValues.mineralProgressionSpeed,-3)),      spawnDepthStop = (int )(-1800* StaticValues.mineralProgressionSpeed)},

    };

    /*
    public static Tile GetMineralTile(Mineral mineralType) 
    {
        foreach (var ore in allMineral)
        {
            if (ore.GetMineralType() == mineralType) 
            {
                return ore.GetTile();
            }
        }
        return null;
    }
    */

    public static readonly List<Item> allCrystals = new List<Item>()
    {
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalBerry,    value = 10, rarity = 0.3f,  spawnDepthStart = -1600,    spawnDepthStop = -1900},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalGrass,    value = 10, rarity = 0.21f, spawnDepthStart = -300,     spawnDepthStop = -600},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalLotus,    value = 10, rarity = 0.15f, spawnDepthStart = -2650,    spawnDepthStop = -2900},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalRose,     value = 10, rarity = 0.12f, spawnDepthStart = -2800,    spawnDepthStop = -5000},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalSea,      value = 10, rarity = 0.4f,  spawnDepthStart = -800,     spawnDepthStop = -1500},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalSky,      value = 10, rarity = 0.23f, spawnDepthStart = -500,     spawnDepthStop = -900},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalStarfall, value = 10, rarity = 0.17f, spawnDepthStart = -1000,    spawnDepthStop = -1800},
        new Item() {itemCategory = ItemCategory.Crystal, itemType = Crystal.CrystalSun,      value = 10, rarity = 0.12f, spawnDepthStart = -2000,    spawnDepthStop = -4000}
    };

    public static readonly List<Item> allFish = new List<Item>()
    {
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.ShimmerFish,       value = 10, rarity = 0.3f,  spawnDepthStart = (int )(0 * StaticValues.fishProgressionSpeed),        spawnDepthStop = (int )(-500 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.StarSkyFish,       value = 10, rarity = 0.2f,  spawnDepthStart = (int )(0 * StaticValues.fishProgressionSpeed),        spawnDepthStop = (int )(-600 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.BunFish,           value = 10, rarity = 1f,    spawnDepthStart = (int )(0 * StaticValues.fishProgressionSpeed),        spawnDepthStop = (int )(-300 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.PeachFish,         value = 10, rarity = 0.24f, spawnDepthStart = (int )(0 * StaticValues.fishProgressionSpeed),        spawnDepthStop = (int )(-450 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.SharpFish,         value = 10, rarity = 1f,    spawnDepthStart = (int )(-300 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-600 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.SkyMantaRay,       value = 10, rarity = 0.15f, spawnDepthStart = (int )(-300 * StaticValues.fishProgressionSpeed) ,    spawnDepthStop = (int )(-1000 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.MandrakeFish,      value = 10, rarity = 0.27f, spawnDepthStart = (int )(-300 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-650 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.SprinkleFish,      value = 10, rarity = 0.2f,  spawnDepthStart = (int )(-500 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-950 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.LoafFish,          value = 10, rarity = 0.19f, spawnDepthStart = (int )(-600 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-1000 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.BlueShimmerFish,   value = 10, rarity = 1f,    spawnDepthStart = (int )(-600 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-800 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.NebulaFish,        value = 10, rarity = 0.12f, spawnDepthStart = (int )(-800 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-1100 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.Shrimp,            value = 10, rarity = 0.16f, spawnDepthStart = (int )(-800 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-1500 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.Tuna,              value = 10, rarity = 0.2f,  spawnDepthStart = (int )(-800 * StaticValues.fishProgressionSpeed),     spawnDepthStop = (int )(-1300 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.BlueFish,          value = 10, rarity = 0.22f, spawnDepthStart = (int )(-1200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-1500 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.OinkCatFish,       value = 10, rarity = 0.11f, spawnDepthStart = (int )(-1200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-1400 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.PupFish,           value = 10, rarity = 1f,    spawnDepthStart = (int )(-1200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-1800 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.Axolotl,           value = 10, rarity = 0.08f, spawnDepthStart = (int )(-1600 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-2100 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.FluffPlankton,     value = 10, rarity = 1f,    spawnDepthStart = (int )(-1600 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-2500 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.SoftAnchovi,       value = 10, rarity = 0.31f, spawnDepthStart = (int )(-1600 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-2900 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.Baraccuda,         value = 10, rarity = 0.35f, spawnDepthStart = (int )(-2200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-3000 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.ClementineFish,    value = 10, rarity = 0.25f, spawnDepthStart = (int )(-2200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-2900 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.FloraFish,         value = 10, rarity = 0.15f, spawnDepthStart = (int )(-2200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-2700 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.BlackHoleFish,     value = 10, rarity = 0.12f, spawnDepthStart = (int )(-3200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-4500 * StaticValues.fishProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.Fish, itemType = Fish.Octopus,           value = 10, rarity = 0.2f,  spawnDepthStart = (int )(-3200 * StaticValues.fishProgressionSpeed),    spawnDepthStop = (int )(-4700 * StaticValues.fishProgressionSpeed)},
    };

    public static readonly List<Item> allUnderwaterPlant = new List<Item>()
    {
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.LongSeaweed,       value = 10, rarity = 1f,        spawnDepthStart = (int )(-3 * StaticValues.underwaterPlantProgressionSpeed),    spawnDepthStop = (int )(-400 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.ReefFingers,       value = 10, rarity = 0.3f,      spawnDepthStart = (int )(-3 * StaticValues.underwaterPlantProgressionSpeed),    spawnDepthStop = (int )(-420 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.SeaGrass,          value = 10, rarity = 0.23f,     spawnDepthStart = (int )(-3 * StaticValues.underwaterPlantProgressionSpeed),    spawnDepthStop = (int )(-500 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.CoralTubes,        value = 10, rarity = 0.23f,     spawnDepthStart = (int )(-3 * StaticValues.underwaterPlantProgressionSpeed),    spawnDepthStop = (int )(-400 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.WideSeaweed,       value = 10, rarity = 1f,        spawnDepthStart = (int )(-300 * StaticValues.underwaterPlantProgressionSpeed),  spawnDepthStop = (int )(-1300 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.MushroomAnemone,   value = 10, rarity = 0.2f,      spawnDepthStart = (int )(-300 * StaticValues.underwaterPlantProgressionSpeed),  spawnDepthStop = (int )(-800 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.StarGrass,         value = 10, rarity = 0.1f,      spawnDepthStart = (int )(-500 * StaticValues.underwaterPlantProgressionSpeed),  spawnDepthStop = (int )(-1300 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.CoralDress,        value = 10, rarity = 0.23f,     spawnDepthStart = (int )(-500 * StaticValues.underwaterPlantProgressionSpeed),  spawnDepthStop = (int )(-800 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.BlobSeaGrass,      value = 10, rarity = 1f,        spawnDepthStart = (int )(-1200 * StaticValues.underwaterPlantProgressionSpeed), spawnDepthStop = (int )(-2000 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.BlobStars,         value = 10, rarity = 0.15f,     spawnDepthStart = (int )(-1600 * StaticValues.underwaterPlantProgressionSpeed), spawnDepthStop = (int )(-2350 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.PinkSoftGrass,     value = 10, rarity = 0.33f,     spawnDepthStart = (int )(-2200 * StaticValues.underwaterPlantProgressionSpeed), spawnDepthStop = (int )(-4000 * StaticValues.underwaterPlantProgressionSpeed)},
        new Item() {itemCategory = ItemCategory.UnderwaterPlant, itemType = UnderwaterPlant.PinkSoftSeaweed,   value = 10, rarity = 1f,        spawnDepthStart = (int )(-2500 * StaticValues.underwaterPlantProgressionSpeed), spawnDepthStop = (int )(-5000 * StaticValues.underwaterPlantProgressionSpeed)},
    };

    public Item[] GetAllIInvtems()
    {
        List<Item> concatItems = new List<Item>();
        concatItems.AddRange(allCrystals);
        concatItems.AddRange(allFish);
        concatItems.AddRange(allMineral);
        concatItems.AddRange(allOther);
        return concatItems.ToArray();
    }

    public Sprite GetGeneralSprite(Item item)
    {
        int index = -1;
        switch (item.GetItemCategory())
        {
            case ItemCategory.None:
                return null;
            case ItemCategory.Other:
                index = (int)(Other)item.GetItemType();
                return allOthersSprites[index];
            case ItemCategory.Mineral:
                index = (int)(Mineral)item.GetItemType();
                return allMineralsSprites[index];
            case ItemCategory.Crystal:
                index = (int)(Crystal)item.GetItemType();
                return allCrystalsSprites[index];
            case ItemCategory.Fish:
                return GetFishSprite((Fish)item.GetItemType());
            case ItemCategory.Furniture:
                index = (int)(Furniture)item.GetItemType();
                return allFurnituresSprites[index];
            case ItemCategory.Wall:
                index = (int)(Wall)item.GetItemType();
                return allWallSprites[index];
            case ItemCategory.Roof:
                index = (int)(Roof)item.GetItemType();
                return allRoofSprites[index];
            case ItemCategory.Floor:
                index = (int)(Floor)item.GetItemType();
                return allFloorsSprites[index];
            case ItemCategory.UnderwaterPlant:
                index = (int)(UnderwaterPlant)item.GetItemType();
                return allUnderwaterPlantsSprites[index];
            default:
                break;
        }

        return null;
    }

    
    public Tile GetMineralTile(Mineral mineralType)
    {
        int index = (int)mineralType;
        return allMineralsTiles[index];
    }
    

    public Sprite GetFishSprite(Fish fishType)
    {
        int index = (int)fishType;
        return allFishesSprites[index];
    }

    public Sprite GetUnderwaterSprite(UnderwaterPlant fishType)
    {
        int index = (int)fishType;
        return allUnderwaterPlantsSprites[index];
    }

    public Item GetMineral(Mineral mineral)
    {
        foreach (Item mineralItem in allMineral)
        {
            if (mineral == (Mineral)mineralItem.GetItemType())
                return (Item)mineralItem;
        }
        return null;
    }

    public Item GetFish(Fish fish)
    {
        foreach (Item fishItem in allFish)
        {
            if (fish == (Fish)fishItem.GetItemType())
                return (Item)fishItem;
        }
        return null;
    }

    public Item GetUnderwaterPlant(UnderwaterPlant plant)
    {
        foreach (Item underwaterPlant in allUnderwaterPlant)
        {
            if (plant == (UnderwaterPlant)underwaterPlant.GetItemType())
                return (Item)underwaterPlant;
        }
        return null;
    }
}




/*
foreach (var item in allItems)
{
    itemTypes.Add(item.type, item);
}
*/

/*
[Serializable]
public class MineralSprite
{
    public Mineral itemType;
    public Sprite itemSprite;
    public Tile itemTile;
}

[Serializable]
public class CrystalSprite
{
    public Crystal itemType;
    public Sprite itemSprite;
}

[Serializable]
public class OtherSprite
{
    public Other itemType;
    public Sprite itemSprite;
}

[Serializable]
public class FishSprite
{
    public Fish itemType;
    public Sprite itemSprite;
}

[Serializable]
public class UnderwaterPlantSprite
{
    public UnderwaterPlant itemType;
    public Sprite itemSprite;
}

[Serializable]
public class LandPlantSprite
{
    public LandPlant itemType;
    public Sprite itemSprite;
}

[Serializable]
public class FurnitureSprite
{
    public Furniture itemType;
    public Sprite itemSprite;
}

[Serializable]
public class CatSprite
{
    //public Cat itemType;
}

[Serializable]
public class GeneralItemSprite<T>
{
    public T itemType;
    public Sprite itemSprite;

    public T GetItemType()
    {
        return itemType;
    }

    public Sprite GetSprite()
    {
        return itemSprite;
    }
}
*/

/*
public MineralSprite[] allMineralSprites;
public CrystalSprite[] allCrystalSprites;
public OtherSprite[] allOtherSprites;
public FishSprite[] allFishSprites;
public FurnitureSprite[] allFurnitureSprites;
public CatSprite[] allCatSprites;
public UnderwaterPlantSprite[] allUnderwaterPlantSprites;
public LandPlantSprite[] allLandPlantSprites;
*/




/*
public FishItem GetFish(Fish fishType)
{
    foreach (FishItem fish in allFish)
    {
        if (fish.itemType == fishType)
            return fish;
    }
    Debug.LogError("There was no fish of that type");
    return null;
}

public MineralItem GetMineral(Mineral mineralType)
{
    foreach (MineralItem mineral in allMineral)
    {
        if (mineral.itemType == mineralType)
            return mineral;
    }
    Debug.LogError("There was no fish of that type");
    return null;
}
*/



/*
public IItem GetItem(IItem itemType)
{
    foreach (var item in allOther)
    {
        if (item.GetItemType() == itemType.GetItemType())
        {
            return item;
        }
    }

    foreach (var item in allMineral)
    {
        if (item.GetItemType() == itemType.GetItemType())
        {
            return item;
        }
    }

    foreach (var item in allFish)
    {
        if (item.GetItemType() == itemType.GetItemType())
        {
            return item;
        }
    }

    foreach (var item in allCrystals)
    {
        if (item.GetItemType() == itemType.GetItemType())
        {
            return item;
        }
    }


    Debug.LogError("Couldn't find the item in the database");
    return null;
}

public Sprite GetFishSprite(Fish fishType)
{
    foreach (var fish in allFishSprites)
    {
        if (fish.itemType == fishType)
            return fish.itemSprite;
    }
    Debug.LogError("There was no fish of that type with a belonging Sprite");
    return null;
}

public Sprite GetCrystalSprite(Crystal itemType)
{
    foreach (var item in allCrystalSprites)
    {
        if (item.itemType == itemType)
            return item.itemSprite;
    }
    Debug.LogError("There was no item of that type with a belonging Sprite");
    return null;
}

public Sprite GetOtherSprite(Other itemType)
{
    foreach (var item in allOtherSprites)
    {
        if (item.itemType == itemType)
            return item.itemSprite;
    }
    Debug.LogError("There was no item of that type with a belonging Sprite");
    return null;
}

public Sprite GetMineralSprite(Mineral itemType)
{
    foreach (var item in allMineralSprites)
    {
        if (item.itemType == itemType)
            return item.itemSprite;
    }
    Debug.LogError("There was no item of that type with a belonging Sprite");
    return null;
}

public Tile GetMineralTile(Mineral itemType)
{
    foreach (var item in allMineralSprites)
    {
        if (item.itemType == itemType)
            return item.itemTile;
    }
    Debug.LogError("There was no item of that type with a belonging Tile");
    return null;
}

public Sprite GetFurnitureSprite(Furniture itemType)
{
    foreach (var item in allFurnitureSprites)
    {
        if (item.itemType == itemType)
            return item.itemSprite;
    }
    Debug.LogError("There was no item of that type with a belonging Sprite");
    return null;
}
*/



/*
public enum ItemType
{
    None,
    SmallOre,
    MediumOre,
    LargeOre,
    SmallFish,
    MediumFish,
    LargeFish,
}

public enum ItemCategory
{
    Material,
    Ore,
    Fish,


}
*/

/*
[System.Serializable]
public class Item
{
    public ItemType type;
    public Sprite itemSprite;
    public string itemName;
    public float value;
}
*/

//public Item[] allItems;
//public Dictionary<IItem, Item> itemTypes = new Dictionary<IItem, Item>();

/*
public Tile defaultTile;
public Tile smallOreTile;
public Tile mediumOreTile;
public Tile bigOreTile;
    */