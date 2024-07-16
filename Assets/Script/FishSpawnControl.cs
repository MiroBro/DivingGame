using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishSpawnControl : MonoBehaviour
{

    public GameObject fishPrefab; // Reference to your Fish prefab
    public Transform fishParent;
    public int fishSeed = 0; // Seed for Perlin noise
    public float fishDespawnDistance = 20f; // Distance from the player to despawn fishes
    public int spawnDistance = 12;
    public Tilemap tileMap;
    public float xOrg;
    public float yOrg;
    public int seed;
    public bool useRandomSeed;
    public float fishSpawnThreshold = 0.68f;
    public float scale = 5F;

    public Dictionary<Vector2, FishMoveControl.FishInstance> spawnedFishes = new Dictionary<Vector2, FishMoveControl.FishInstance>();
    private Dictionary<Guid, FishMoveControl.FishInstance> fishIds = new Dictionary<Guid, FishMoveControl.FishInstance>();
    public static Dictionary<Vector3Int, Fish> fishPos = new Dictionary<Vector3Int, Fish>();

    //private static Dictionary<ItemReferences.ItemType, FishData> allFishesData = new Dictionary<ItemReferences.ItemType, FishData>();

    /*
    public class FishData
    {
        public ItemReferences.ItemType type;
        public Sprite fishSprite;
        public float rarity;
        public int rarityScore;
        public float startsSpawningAtYDepth;
        public float stopsSpawningAtYDepth;
    }
    */

    void Start()
    {
        /*
        allFishesData = new Dictionary<ItemReferences.ItemType, FishData>()
        {
        {ItemReferences.ItemType.SmallFish, new FishData{type = ItemReferences.ItemType.SmallFish, fishSprite = ItemReferences.Instance.GetItem(ItemReferences.ItemType.SmallFish).itemSprite, rarity = 1f, rarityScore = 0, startsSpawningAtYDepth = -5, stopsSpawningAtYDepth = -100000}},
        {ItemReferences.ItemType.MediumFish, new FishData{type = ItemReferences.ItemType.MediumFish, fishSprite = ItemReferences.Instance.GetItem(ItemReferences.ItemType.MediumFish).itemSprite, rarity = 0.2f, rarityScore = 10, startsSpawningAtYDepth = -100, stopsSpawningAtYDepth = -400}},
        {ItemReferences.ItemType.LargeFish, new FishData{type = ItemReferences.ItemType.LargeFish, fishSprite = ItemReferences.Instance.GetItem(ItemReferences.ItemType.LargeFish).itemSprite, rarity = 0.1f, rarityScore = 30, startsSpawningAtYDepth = -300, stopsSpawningAtYDepth = -700}},
        };
        */

        if (useRandomSeed)
        {
            seed = UnityEngine.Random.Range(0, 1000000);
        }
    }

    void Update()
    {
        // Check player's position and despawn fishes if they are far away
        DespawnFishes();

        // Check if the player has moved into a new location and spawn fishes accordingly
        SpawnFishes(spawnDistance);

        References.Instance.fishMoveControl.MoveAllSpawnedFishes();
    }

    void DespawnFishes()
    {
        var copiedDictionary = spawnedFishes.ToDictionary(entry => entry.Key, entry => entry.Value);
        foreach (var fish in copiedDictionary)
        {
            if (!fish.Value.caught)
            {
                Vector3 fishPosition = fish.Value.fishObject.transform.position;
                if (Vector3.Distance(fishPosition, References.Instance.playerMovingTransform.position) > fishDespawnDistance)
                {
                    Destroy(fish.Value.fishObject);
                    spawnedFishes.Remove(fish.Value.position);
                }
            }
        }
    }

    private void SpawnFishes(int radius)
    {
        var currentPlayerPos = tileMap.WorldToCell(References.Instance.playerMovingTransform.position);

        for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
        {
            for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
            {
                if (y < 0)
                {
                    if (!spawnedFishes.ContainsKey(new Vector2(x, y)))
                    {
                        float xCoord = (seed + xOrg) + (x + 500000) / scale;
                        float yCoord = (seed + yOrg) + (y + 500000) / scale;
                        float sample = Mathf.PerlinNoise(xCoord, yCoord);
                        if (sample > fishSpawnThreshold && !FishHasBeenCaught(new Vector3(x, y, 0)))
                        {
                            SpawnRarestPossibleFish(new Vector2(x, y));
                        }
                    }
                }
            }
        }
    }

    private Dictionary<Vector2, long> posFirstEncountered = new Dictionary<Vector2, long>();

    private void SpawnRarestPossibleFish(Vector2 pos)
    {
        if (!posFirstEncountered.ContainsKey(pos))
            posFirstEncountered.Add(pos, DateTime.Now.Ticks);

        //var rarestFishFirstDict = from entry in allFishesData orderby entry.Value.rarityScore descending select entry;
        var rarestFishFirstDict = ItemReferences.allFish.OrderBy(fish => ((Item)fish).rarity).ToList();//entry.Value.rarityScore descending select entry;
        //var rarestFishFirstDict = from fish in ItemReferences.allFish orderby ((Items.FishItem) fish).rarity descending select fish;//entry.Value.rarityScore descending select entry;

        foreach (Item fishTypeEntry in rarestFishFirstDict)
        {
            if (pos.y <= fishTypeEntry.spawnDepthStart && pos.y >= fishTypeEntry.spawnDepthStop)
            {

                int posAndFishSpecificSeed = string.Concat(
                posFirstEncountered[pos],
                (pos.x * 661),
                (pos.y * 421),
                fishTypeEntry.rarity,
                (int)(Fish)fishTypeEntry.GetItemType())
                .GetHashCode();

                System.Random fishesRandom = new System.Random(posAndFishSpecificSeed);
                double fishRandomRoll = fishesRandom.NextDouble();

                if (fishRandomRoll < fishTypeEntry.rarity)
                {
                    GameObject fish = Instantiate(fishPrefab, pos, Quaternion.identity, fishParent);
                    var fishId = fish.GetComponent<FishInstanceInfo>();
                    Guid fishGuid = Guid.NewGuid();
                    fishId.id = fishGuid;
                    fishId.SetToFishType((Fish)fishTypeEntry.itemType);

                    var fishInstanceInfo = new FishMoveControl.FishInstance((Fish)fishTypeEntry.itemType, pos, fish);

                    if (fishIds.ContainsKey(fishGuid))
                        fishIds[fishGuid] = fishInstanceInfo;
                    else
                        fishIds.Add(fishGuid, fishInstanceInfo);

                    spawnedFishes.Add(pos, fishInstanceInfo);
                    break;
                }
            }
        }
    }

    public Fish GetFishAtPosition(Vector3Int position)
    {
        if (fishPos.ContainsKey(position))
        {
            return fishPos[position];
        }
        else
        {
            return Fish.None;
        }
    }

    private bool FishHasBeenCaught(Vector3 pos)
    {
        if (spawnedFishes.ContainsKey(pos))
            return spawnedFishes[pos].caught;
        else return false;
    }


    public FishMoveControl.FishInstance GetFishInstance(Guid id)
    {
        return fishIds[id];
    }

    public void CatchFish(Guid fishId)
    {
        fishIds[fishId].caught = true;
        References.Instance.experienceControl.AddExperience(ItemReferences.Instance.GetFish(fishIds[fishId].fishType).value);//GetItem(fishIds[fishId].fishType).value);

        Destroy(fishIds[fishId].fishObject);

        Inventory.Instance.AddItemToInventory(ItemReferences.Instance.GetFish(GetFishInstance(fishId).fishType), 1);
        //Inventory.Instance.AddItemToInventory(GetFishInstance(fishId).fishType,1);
        References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetFish(GetFishInstance(fishId).fishType), 1);
        //References.Instance.uiControl.ShowPickUpItem(GetFishInstance(fishId).fishType, 1);


    }
}

/*
     private void GenerateFishes(int x, int y)
    {
        Vector2 pos = new Vector2(x, y);
        Vector3Int intPos = new Vector3Int(x, y, 0);

        var sortedDict = from entry in allFishesData orderby entry.Value.rarityScore ascending select entry;

        foreach (var kvp in allFishesData.OrderByDescending(x => x.Value.rarityScore))
        {
            Debug.Log(kvp.Key + " " + kvp.Value);
        }
        Debug.Log("done!");

        foreach (var fishDataEntry in sortedDict)
        {

            if (!spawnedFishes.ContainsKey(pos) || spawnedFishes[pos].fishObject == null)
            {
                FishData fishData = fishDataEntry.Value;

                if (!FishHasBeenCaught(intPos) && fishPos.ContainsKey(intPos) && fishPos[intPos] == fishData.type)
                {
                    Guid fishGuid = Guid.NewGuid();

                    GameObject fish = Instantiate(fishPrefab, pos, Quaternion.identity, fishParent);
                    var fishId = fish.GetComponent<FishInstanceInfo>();
                    fishId.id = fishGuid;
                    fishId.SetToFishType(fishData.type);

                    var f = new FishMoveControl.FishInstance(fishData.type, pos, fish);
                    if (spawnedFishes.ContainsKey(pos))
                    {
                        spawnedFishes[pos] = f;
                    }
                    else
                    {
                        spawnedFishes.Add(pos, f);
                    }
                }
                else if (!FishHasBeenCaught(pos) &&
                    y <= fishData.startsSpawningAtYDepth &&
                    y >= fishData.stopsSpawningAtYDepth)
                {
                    float xCoord = (seed + xOrg) + (x + 500000) / scale;
                    float yCoord = (seed + yOrg) + (y + 500000) / scale;
                    float sample = Mathf.PerlinNoise(xCoord, yCoord);

                    if (sample > fishSpawnThreshold)
                    {
                        System.Random random = new System.Random((int)((System.DateTime.Now.Ticks + seed + ((pos.x + pos.y) * (int)fishData.type)) / int.MaxValue));
                        double fishRoll = random.NextDouble();

                        if (fishRoll < fishData.rarity)
                        {
                            ItemReferences.ItemType fishItem = ItemReferences.ItemType.None;

                            if (!fishPos.ContainsKey(intPos))
                            {
                                fishPos.Add(intPos, fishData.type);
                                fishItem = fishData.type;
                            }
                            else if (fishPos[intPos] < fishData.type)
                            {
                                fishPos[intPos] = fishData.type;
                                fishItem = fishData.type;
                            }

                            if (fishItem != ItemReferences.ItemType.None)
                            {
                                Guid fishGuid = Guid.NewGuid();

                                GameObject fish = Instantiate(fishPrefab, pos, Quaternion.identity, fishParent);
                                var fishId = fish.GetComponent<FishInstanceInfo>();
                                fishId.id = fishGuid;
                                fishId.SetToFishType(fishData.type);

                                if (!spawnedFishes.ContainsKey(pos))
                                {
                                    var f = new FishMoveControl.FishInstance(fishData.type, pos, fish);
                                    spawnedFishes.Add(pos, f);
                                }
                                else
                                {
                                    var f = new FishMoveControl.FishInstance(fishData.type, pos, fish);
                                    spawnedFishes[pos] = f;
                                }
                            }
                        }
                    }
                }
            }
            //else if (spawnedFishes.ContainsKey(pos) && spawnedFishes[pos].fishObject != null && fishDataEntry.Key > spawnedFishes[pos].fishType)
            //{
            //    //System.Random random = new System.Random(dateTime + seed + ((pos.x + pos.y) * (int)fishDataEntry.Key)) / int.MaxValue));
            //    System.Random random = new System.Random((int)((System.DateTime.Now.Ticks + seed + ((pos.x + pos.y) * (int)fishDataEntry.Key)) / int.MaxValue));
            //    double fishRoll = random.NextDouble();

            //    if (fishRoll < fishDataEntry.Value.rarity 
            //        && pos.y <= fishDataEntry.Value.startsSpawningAtYDepth 
            //        && pos.y >= fishDataEntry.Value.stopsSpawningAtYDepth)
            //    {
            //        spawnedFishes[pos].fishInstanceInfo.SetToFishType(fishDataEntry.Key);
            //    }
            //}
        }


    }

 */

/*
int posAndFishSpecificSeed =
    (int)((posFirstEncountered[pos]
    + (pos.x * 661)
    + (pos.y * 421)
    + fishTypeEntry.Value.rarityScore
    + (int)fishTypeEntry.Value.type)
    / int.MaxValue);

System.Random fishesRandom = new System.Random(posAndFishSpecificSeed);
*/

/*
 void SpawnFishes(int radius)
 {
     var currentPlayerPos = tileMap.WorldToCell(References.Instance.playerMovingTransform.position);

     var start = currentPlayerPos - new Vector3Int(radius, radius, 0);
     var end = currentPlayerPos + new Vector3Int(radius, radius, 0);

     for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
         for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
         {
             if (y < 0)
             {
                 float xCoord = (seed + xOrg) + (x + 500000) / scale;
                 float yCoord = (seed + yOrg) + (y + 500000) / scale;
                 float sample = Mathf.PerlinNoise(xCoord, yCoord);
                 var pos = tileMap.CellToWorld(new Vector3Int(x, y, 0));
                 if (sample > fishSpawnThreshold && !FishHasBeenCaught(pos) && !FishAlreadySpawned(pos))
                 {
                     foreach (var fish in allFishesData)
                     {
                         SpawnFish(pos, fish.Value);
                     }
                 }
             }
         }
 }
 */
/*
void SpawnFish(Vector3 pos, FishData fishData)
{
    if (!spawnedFishes.ContainsKey(pos))
    {
        var fishRepulsionForce = UnityEngine.Random.Range(References.Instance.fishMoveControl.startRepulsionForce, References.Instance.fishMoveControl.startRepulsionForce + 1);
        int randomSpawnSeed = (int)((System.DateTime.Now.Ticks + ((pos.x + pos.y) * (int)fishData.type)) / int.MaxValue);
        System.Random random = new System.Random(randomSpawnSeed);
        double fishRoll = random.NextDouble();

        if (fishRoll <= fishData.rarity) // && fishData.type <= spawnedFishes[pos].fishIDClass.type
        {
            if (fishData.type == ItemReferences.ItemType.MediumFish) { Debug.Log("AAAH medium fish spawn"); }
            GameObject fish = Instantiate(fishPrefab, pos, Quaternion.identity, fishParent);
            Guid fishId = Guid.NewGuid();
            fish.GetComponent<FishID>().id = fishId;
            fish.GetComponent<FishID>().SetToFishType(fishData.type);
            FishID fishIdClass = fish.GetComponent<FishID>();


            var newFish = new FishMoveControl.FishInstance(fishID: fishId, position: pos, fishObject: fish, fishIDClass: fishIdClass, repulsionForce: fishRepulsionForce, randomSpawnSeed: randomSpawnSeed, fishType: fishData.type);

            spawnedFishes.Add(pos, newFish);
            fishIds.Add(fishId, newFish);
        }
    }
}
*/
/*
private void GenerateFishes(int x, int y)
{
    foreach (var fishData in allFishesData)
    {

        FishData thisFishData = fishData.Value;

        if (!FishHasBeenCaught(new Vector3Int(x, y, 0)) && spawnedFishes.ContainsKey(new Vector2(x, y)) && spawnedFishes[new Vector2(x, y)].fishType == thisFishData.type)
        {
            //Tile selectedOreTile = null;
            //selectedOreTile = fish.oreTile;
            //tileMap.SetTile(new Vector3Int(x, y, 0), selectedOreTile);

            GameObject fish = Instantiate(fishPrefab, new Vector2(x, y), Quaternion.identity, fishParent);
            Guid fishId = Guid.NewGuid();
            fish.GetComponent<FishID>().id = fishId;
            fish.GetComponent<FishID>().SetToFishType(fishData.Value.type);
            FishID fishIdClass = fish.GetComponent<FishID>();
            var newFish = new FishMoveControl.FishInstance(fishID: fishId, position: pos, fishObject: fish, fishIDClass: fishIdClass, repulsionForce: fishRepulsionForce, randomSpawnSeed: randomSpawnSeed, fishType: fishData.type);
            spawnedFishes.Add(new Vector2(x, y), newFish);
            fishIds.Add(fishId, newFish);
        }
        else if (!FishHasBeenCaught(new Vector3Int(x, y, 0)) &&
            y <= thisFishData.startsSpawningAtYDepth &&
            y >= thisFishData.stopsSpawningAtYDepth)
        {
            float xCoord = (seed + xOrg) + (x + 500000) / scale;
            float yCoord = (seed + yOrg) + (y + 500000) / scale;
            float sample = Mathf.PerlinNoise(xCoord, yCoord);

            if (sample > thisFishData.rarity)
            {
                int randomSpawnSeed = (int)((System.DateTime.Now.Ticks + seed + ((x + y) * (int)thisFishData.type)) / int.MaxValue);
                System.Random random = new System.Random(randomSpawnSeed);
                double oreRoll = random.NextDouble();

                if (oreRoll < thisFishData.rarity)
                {
                    Tile selectedOreTile = null;

                    if (!orePos.ContainsKey(new Vector3Int(x, y)))
                    {
                        orePos.Add(new Vector3Int(x, y), thisFishData.type);
                        selectedOreTile = thisFishData.oreTile;
                    }
                    else if (orePos[new Vector3Int(x, y)] < thisFishData.type)
                    {
                        orePos[new Vector3Int(x, y)] = thisFishData.type;
                        selectedOreTile = thisFishData.oreTile;
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
*/
