using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Item
{
    public ItemCategory itemCategory;
    public Enum itemType;
    public string name;
    public float value;
    public float rarity;
    public int spawnDepthStart;
    public int spawnDepthStop;

    public Sprite GetIcon()
    {
        return ItemReferences.Instance.GetGeneralSprite(this);
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
    public Tile GetTile()
    {
        if (itemCategory == ItemCategory.Mineral)
        {
            return ItemReferences.Instance.GetMineralTile((Mineral)itemType);
        }
        else
        {
            Debug.LogError("Cannot get tile of non-mineral item");
            return null;
        }
    }

    public Mineral GetMineralType() 
    {
        return (Mineral)itemType;
    }

    public Fish GetFishType()
    {
        return (Fish)itemType;
    }

    public Furniture GetFurnitureType()
    {
        return (Furniture)itemType;
    }

    public UnderwaterPlant GetUnderwaterPlantType() 
    {
        return (UnderwaterPlant)itemType; 
    }
}

public enum SeaCreature
{
    None,
    LakeCreature,
    SoulCreature,
}

public enum Chest
{
    None,
    Pink,
    Gold,
}

public enum Fish
{
    None,
    BunFish,
    PeachFish,
    ShimmerFish,
    StarSkyFish,
    MandrakeFish,
    SharpFish,
    SkyMantaRay,
    BlueShimmerFish,
    LoafFish,
    SprinkleFish,
    NebulaFish,
    Shrimp,
    Tuna,
    BlueFish,
    OinkCatFish,
    PupFish,
    Axolotl,
    FluffPlankton,
    SoftAnchovi,
    Baraccuda,
    ClementineFish,
    FloraFish,
    BlackHoleFish,
    Octopus,
}

public enum Furniture
{
    None,
    Bed,
    Bookshelf,
    Cabinet,
    Carpet,
    Cauldron,
    Chair,
    Chandelier,
    Curtain,
    Fireplace,
    FloorLamp,
    OutsideLampBig,
    OutsideLampSmall,
    Painting,
    PetBad,
    Plant,
    Shelf,
    Sofa,
    Table,
    TableLamp,
    WallLamp,
    Window,
}

public enum Mineral
{
    None,
    Amethyst,
    Ammolite,
    AuraQuartz,
    Iron,
    Labradorite,
    Moonstone,
    Stone,
    SunsetOpal,
    Mud,
}

public enum Crystal
{
    None,
    CrystalBerry,
    CrystalGrass,
    CrystalLotus,
    CrystalRose,
    CrystalSea,
    CrystalSky,
    CrystalStarfall,
    CrystalSun,
}

public enum Other
{
    None,
    Sand,
    Seaweed,
    WaterAcorn,
    WaterRoot,
    WaterWood,
}

public enum ItemCategory
{
    None,
    Other,
    Mineral,
    Crystal,
    Fish,
    Furniture,
    Wall,
    Roof,
    Floor,
    LandPlant,
    UnderwaterPlant,
}

public enum Wall
{
    None,
    WhiteCatPattern,
}

public enum Roof
{
    None,
    PinkTiles,
}

public enum Floor
{
    None,
    PaleWood,
    PalePinkTiles,
}

public enum UnderwaterPlant
{
    None,
    BlobSeaGrass,
    BlobStars,
    CoralDress,
    CoralTubes,
    MushroomAnemone,
    PinkSoftGrass,
    PinkSoftSeaweed,
    ReefFingers,
    SeaGrass,
    LongSeaweed,
    StarGrass,
    WideSeaweed,
}

public enum LandPlant
{
    None,
    BerryBush,
    FluffBush,
    GoldenLeaves,
    Grass,
    HeatFlower,
    PillowFlower,
    Pine,
    SnowDrops,
    SnowBush,
}

public enum Cat
{
    None,
    OrangeCat,
    PrismCat,
    SpaceCat,
    SunriseCat
}
