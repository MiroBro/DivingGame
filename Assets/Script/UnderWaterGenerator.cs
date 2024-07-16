using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class UnderWaterGenerator : MonoBehaviour
{
    public Tilemap tileMap;
    public Tilemap backTileMap;

    // This is used to configure the perlin noise that determines the procedurally generated underwater world
    public int mineralSeed;
    public int underwaterPlantSeed;

    public bool useRandomSeed;
    public float minimumAlphaCutOff = 0.675f;
    public float xOrg;
    public float yOrg;
    public float scale = 1.0F;

    //Used to keep track of which blocks have been mined, and which blocks hold ores
    //public static HashSet<Vector3Int> minedBlocks = new HashSet<Vector3Int>();
    public static Dictionary<ItemCategory, HashSet<Vector3Int>> minedObjects = new Dictionary<ItemCategory, HashSet<Vector3Int>>() { { ItemCategory.Mineral, new HashSet<Vector3Int>() }, { ItemCategory.UnderwaterPlant, new HashSet<Vector3Int>() } };
    //public static Dictionary<Items.ItemCategory, Dictionary<Vector3Int, Items.Mineral>> objPos = new Dictionary<Items.ItemCategory, Dictionary<Vector3Int, Items.Mineral>>();
    public static Dictionary<Vector3Int, Mineral> orePos = new Dictionary<Vector3Int, Mineral>();
    public static Dictionary<Vector3Int, UnderwaterPlant> plantPos = new Dictionary<Vector3Int, UnderwaterPlant>();


    private Dictionary<Vector2, long> posFirstEncountered = new Dictionary<Vector2, long>();
    private Dictionary<Guid, PlantInstance> plantIds = new Dictionary<Guid, PlantInstance>();
    public Dictionary<Vector2, PlantInstance> spawnedPlants = new Dictionary<Vector2, PlantInstance>();

    //Used to calculate if and how wide the tiles around the player should be refreshed
    private Vector3Int prevPos;
    private Vector3Int currentPlayerPos;
    public int radiusOfLoadedTiles = 12;

    //Used in the editor to quickly see the whole scene at once
    public bool showAllAtOnce;

    public GameObject otherItemPrefab;
    public Transform otherItemParent;
    public int spawnDistance = 12;

    public void Start()
    {
        if (useRandomSeed)
        {
            mineralSeed = UnityEngine.Random.Range(0, 1000000);
            underwaterPlantSeed = UnityEngine.Random.Range(0, 1000000);
        }
        SpawnBlocksAroundPlayer(radiusOfLoadedTiles);
    }

    private void Update()
    {
        currentPlayerPos = tileMap.WorldToCell(References.Instance.playerMovingTransform.position);
        if (!prevPos.Equals(currentPlayerPos))
        {
            prevPos = currentPlayerPos;
            SpawnBlocksAroundPlayer(radiusOfLoadedTiles);
            DespawnPlants();
            SpawnPlants(spawnDistance);
        }

        if (showAllAtOnce)
        {
            showAllAtOnce = false;
            orePos = new Dictionary<Vector3Int, Mineral>();
            SpawnBlocksAroundPlayer(100);
        }
    }

    private void SpawnPlants(int radius)
    {
        var currentPlayerPos = tileMap.WorldToCell(References.Instance.playerMovingTransform.position);

        for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
        {
            for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
            {
                if (y < 0)
                {
                    if (!spawnedPlants.ContainsKey(new Vector2(x, y)))
                    {
                        //check if tile above this is empty, and if so, spawn underwaterplant on tile.
                        //if (Mathf.PerlinNoise(GetXCoord(x), GetYCoord(y + 1)) <= minimumAlphaCutOff && Mathf.PerlinNoise(GetXCoord(x), GetYCoord(y)) > minimumAlphaCutOff && orePos.ContainsKey(new Vector3Int(x,y)) && !orePos.ContainsKey(new Vector3Int(x, y+1)))
                        var pos = new Vector3Int(x, y);
                        if (orePos.ContainsKey(pos) && !orePos.ContainsKey(pos + new Vector3Int(0,1)) && !minedObjects[ItemCategory.Mineral].Contains(pos))
                        {

                            SpawnRarestPossiblePlant(x, y);
                            //var inst = Instantiate(otherItemPrefab, new Vector3(x + tileMap.cellSize.x / 2, y + tileMap.cellSize.y, 0), otherItemPrefab.transform.rotation, otherItemParent);
                        }
                    }
                }
            }
        }
    }

    private void SpawnBlocksAroundPlayer(int radius)
    {
        tileMap.ClearAllTiles();
        backTileMap.ClearAllTiles();

        for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
        {
            for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
            {
                if (y < 0)
                {
                    GenerateOreTile(x, y);
                    backTileMap.SetTile(new Vector3Int(x, y, 0), ItemReferences.Instance.waterTile);
                }
            }
        }
    }

    private void GenerateOreTile(int x, int y)
    {
        foreach (Item ore in ItemReferences.allMineral)//var oreEntry in allOres)
        {
            if ((Mineral)ore.itemType == Mineral.Mud)
            {
            }

            //Items.Mineral ore = (Items.Mineral) oreEntry.GetItemType();

            if (minedObjects.ContainsKey(ItemCategory.Mineral) && !minedObjects[ItemCategory.Mineral].Contains(new Vector3Int(x, y, 0)) && orePos.ContainsKey(new Vector3Int(x, y, 0)) && orePos[new Vector3Int(x, y, 0)] == (Mineral)ore.itemType)
            {
                tileMap.SetTile(new Vector3Int(x, y, 0), ore.GetTile());
            }
            else if (!minedObjects[ItemCategory.Mineral].Contains(new Vector3Int(x, y, 0)) &&
                y <= ore.spawnDepthStart &&
                y >= ore.spawnDepthStop)
            {
                float xCoord = GetXCoord(x);
                float yCoord = GetYCoord(y);
                float sample = Mathf.PerlinNoise(xCoord, yCoord);



                if (sample > minimumAlphaCutOff)
                {
                    //System.Random xRand = new System.Random(seed/(Mathf.Abs(x+ (int)xOrg) + (int)yOrg)+1);
                    //System.Random yRand = new System.Random(seed/Mathf.Abs(y + (int)yOrg));

                    System.Random random = new System.Random(mineralSeed + x * 41 + y * 97 + x - y + (int)xOrg + (int)yOrg); //trying to create some randomness in the non random random?
                    //System.Random random = new System.Random(seed + (int)xRand.NextDouble()+ (int)yRand.NextDouble());
                    double oreRoll = random.NextDouble();

                    if (oreRoll < ore.rarity)
                    {
                        if ((Mineral) ore.itemType == Mineral.Mud) 
                        { 
                        }

                        Tile selectedOreTile = null;

                        if (!orePos.ContainsKey(new Vector3Int(x, y)))
                        {
                            orePos.Add(new Vector3Int(x, y), (Mineral)ore.itemType);
                            selectedOreTile = ore.GetTile();
                        }
                        //else if (orePos[new Vector3Int(x, y)] < ore.itemType)
                        else if (ItemReferences.Instance.GetMineral(orePos[new Vector3Int(x, y)]).rarity > ore.rarity
                            && ItemReferences.Instance.GetMineral(orePos[new Vector3Int(x, y)]).spawnDepthStart >= ore.spawnDepthStart)
                        {
                            orePos[new Vector3Int(x, y)] = (Mineral)ore.itemType;
                            selectedOreTile = ore.GetTile();
                        }

                        if (selectedOreTile != null)
                        {
                            tileMap.SetTile(new Vector3Int(x, y, 0), selectedOreTile);
                        }
                    }
                }
            }
        }
    }

    public class PlantInstance
    {
        public Guid id;
        public bool picked = false;
        public GameObject plantObject;
        public Vector2 position;
        public UnderwaterPlant plantType;
    }

    private void SpawnRarestPossiblePlant(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);

        if (!posFirstEncountered.ContainsKey(pos))
            posFirstEncountered.Add(pos, DateTime.Now.Ticks);

        var rarestPlantFirst = ItemReferences.allUnderwaterPlant.OrderBy(plant => ((Item)plant).rarity).ToList();

        foreach (Item plantEntry in rarestPlantFirst)
        {
            if (pos.y <= plantEntry.spawnDepthStart && pos.y >= plantEntry.spawnDepthStop)
            {

                int posAndFishSpecificSeed = string.Concat(
                posFirstEncountered[pos],
                (pos.x * 661),
                (pos.y * 421),
                plantEntry.rarity,
                (int)(Fish)plantEntry.GetItemType())
                .GetHashCode();

                System.Random fishesRandom = new System.Random(posAndFishSpecificSeed);
                double fishRandomRoll = fishesRandom.NextDouble();

                if (fishRandomRoll < plantEntry.rarity)
                {
                    GameObject plant = Instantiate(otherItemPrefab, new Vector3(x + tileMap.cellSize.x / 2, y + tileMap.cellSize.y, 0), otherItemPrefab.transform.rotation, otherItemParent);
                    //GameObject plant = Instantiate(fishPrefab, pos, Quaternion.identity, fishParent);
                    var underwaterInstance = plant.GetComponent<UnderwaterPlantInstance>();
                    Guid plantGuid = Guid.NewGuid();
                    underwaterInstance.id = plantGuid;
                    underwaterInstance.SetUnderwaterPlantTo((UnderwaterPlant)plantEntry.itemType);

                    //var fishInstanceInfo = new FishMoveControl.FishInstance((Items.UnderwaterPlant)plantEntry.itemType, pos, plant);

                    if (plantIds.ContainsKey(plantGuid))
                        plantIds[plantGuid] = new PlantInstance() { plantObject = plant, id = plantGuid, picked = false, position = pos, plantType = (UnderwaterPlant)plantEntry.itemType };
                    else
                        plantIds.Add(plantGuid, new PlantInstance() { plantObject = plant, id = plantGuid, picked = false, position = pos, plantType = (UnderwaterPlant)plantEntry.itemType });

                    if (!plantPos.ContainsKey(new Vector3Int(x, y, 0)))
                        plantPos.Add(new Vector3Int(x, y, 0), (UnderwaterPlant)plantEntry.itemType);
                    spawnedPlants.Add(pos, new PlantInstance() { plantObject = plant, id = plantGuid, picked = false, position = pos, plantType = (UnderwaterPlant)plantEntry.itemType });
                    break;
                }
            }
        }
    }
    public float plantDespawnDistance = 12f; // Distance from the player to despawn fishes

    void DespawnPlants()
    {
        var copiedDictionary = spawnedPlants.ToDictionary(entry => entry.Key, entry => entry.Value);
        foreach (var plant in copiedDictionary)
        {
            if (!plant.Value.picked)
            {
                Vector3 fishPosition = plant.Value.plantObject.transform.position;
                if (Vector3.Distance(fishPosition, References.Instance.playerMovingTransform.position) > plantDespawnDistance)
                {
                    Destroy(plant.Value.plantObject);
                    spawnedPlants.Remove(plant.Value.position);
                }
            }
        }
    }

    private float GetYCoord(int y)
    {
        return (mineralSeed + yOrg) + (y + 500000) / scale;
    }

    private float GetXCoord(int x)
    {
        return (mineralSeed + xOrg) + (x + 500000) / scale;
    }

    public Mineral GetOreAtPosition(Vector3Int position)
    {
        if (orePos.ContainsKey(position))
        {
            return orePos[position];
        }
        else
        {
            return Mineral.None;
        }
    }

    public void SetObjectToMined(ItemCategory itemCategory, Vector3Int atPosition)
    {
        foreach (var item in minedObjects)
        {
            item.Value.Add(atPosition);
        }

        /*
        if (minedObjects.ContainsKey(itemCategory))
        {
            minedObjects[itemCategory].Add(atPosition);
        }
        else 
        {
            minedObjects.Add(itemCategory, new HashSet<Vector3Int> { atPosition } );
        }
        */
    }

    public void PickUpAndDestroyAllItemsAtPos(Vector3Int atPosition)
    {
        if (plantPos.ContainsKey(atPosition) && spawnedPlants.ContainsKey(new Vector2(atPosition.x, atPosition.y)) && !spawnedPlants[new Vector2(atPosition.x, atPosition.y)].picked)
        {
            PlantInstance plant = spawnedPlants[new Vector2(atPosition.x, atPosition.y)];
            Inventory.Instance.AddItemToInventory(ItemReferences.Instance.GetUnderwaterPlant(plant.plantType), 1);
            References.Instance.experienceControl.AddExperience(ItemReferences.Instance.GetUnderwaterPlant(plant.plantType).value);
            References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetUnderwaterPlant(plant.plantType), 1);

            plant.picked = true;
            Destroy(plant.plantObject);
        }
    }
}









/*
private void SpawnBlocksAroundPlayer(int radius)
{
    tileMap.ClearAllTiles();
    backTileMap.ClearAllTiles();

    var start = currentPlayerPos - new Vector3Int(radius, radius, 0);
    var end = currentPlayerPos + new Vector3Int(radius, radius, 0);

    for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
    {
        for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
        {
            if (y < 0 && y > -20) // Depth restriction for small ores
            {
                GenerateOreTile(x, y, allOres[ItemReferences.ItemType.Small].rarity, allOres[ItemReferences.ItemType.Small].oreTile, ItemReferences.ItemType.Small);
            }
            else if (y < 0 && y > -50) // Depth restriction for medium ores
            {
                GenerateOreTile(x, y, allOres[ItemReferences.ItemType.Medium].rarity, allOres[ItemReferences.ItemType.Medium].oreTile, ItemReferences.ItemType.Medium);
            }
            else if (y < 0 && y > -80) // Depth restriction for large ores
            {
                GenerateOreTile(x, y, allOres[ItemReferences.ItemType.Large].rarity, allOres[ItemReferences.ItemType.Large].oreTile, ItemReferences.ItemType.Large);
            }

            // Always set background water tile
            backTileMap.SetTile(new Vector3Int(x, y, 0), ItemReferences.Instance.waterTile);
        }
    }
}

private void GenerateOreTile(int x, int y, float rarity, Tile oreTile, ItemReferences.ItemType itemType)
{
    if (!minedBlocks.Contains(new Vector3Int(x, y, 0)))
    {
        float xCoord = (seed + xOrg) + (x + 500000) / scale;
        float yCoord = (seed + yOrg) + (y + 500000) / scale;
        float sample = Mathf.PerlinNoise(xCoord, yCoord);

        // Determine the tile based on depth and rarity
        Tile selectedOreTile = null;

        // Ensure the depth is within the specified range
        if (sample > minimumAlphaCutOff)
        {
            float depthFactor = Mathf.Clamp01((float)y / -100f);

            System.Random random = new System.Random(seed + x + y + (int)xOrg + (int)yOrg);
            // Generate a deterministic random value based on the position
            double oreRoll = random.NextDouble();

            if (oreRoll < rarity * depthFactor)
            {
                selectedOreTile = oreTile;

                if (!orePos.ContainsKey(new Vector3Int(x, y)))
                {
                    orePos.Add(new Vector3Int(x, y), itemType);
                }
            }
        }

        if (selectedOreTile != null)
        {
            tileMap.SetTile(new Vector3Int(x, y, 0), selectedOreTile);
        }
    }
}
*/