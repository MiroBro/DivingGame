using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapSpawner : MonoBehaviour
{
    public SpriteRenderer spriteR;
    public Color colorGotten;
    public Tilemap landTileMap;
    public Tilemap waterTileMap;
    public Tile[] correspondingTile;

    public int mineralSeed;
    public float scale = 1.15F;
    public float minimumAlphaCutOff = 0.675f;

    public GameObject mineralItemPrefab;
    public Transform mineralParent;

    public Color[] colorCodes;

    //Used to calculate if and how wide the tiles around the player should be refreshed
    private Vector3Int prevPos;
    private Vector3Int currentPlayerPos;
    public int radiusOfLoadedTiles = 15;

    public bool showEverything = false;
    public int showEverythingRadius = 100;

    public bool showEverythingAtStart = false;
    public int showEverythingAtStartRadius = 150;

    public float mineralDespawnDistance = 12f; // Distance from the player to despawn fishes

    public static Dictionary<ItemCategory, HashSet<Vector3Int>> minedObjects = new Dictionary<ItemCategory, HashSet<Vector3Int>>() { { ItemCategory.Mineral, new HashSet<Vector3Int>() }, { ItemCategory.UnderwaterPlant, new HashSet<Vector3Int>() } };

    public GameObject orePrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (showEverythingAtStart)
        {
            SpawnBlocksAroundPlayer(showEverythingAtStartRadius);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!showEverythingAtStart)
        {
            currentPlayerPos = waterTileMap.WorldToCell(References.Instance.playerMovingTransform.position);
            if (!prevPos.Equals(currentPlayerPos))
            {
                prevPos = currentPlayerPos;
                SpawnBlocksAroundPlayer(radiusOfLoadedTiles);
            }

            if (showEverything)
            {
                showEverything = false;
                SpawnBlocksAroundPlayer(showEverythingRadius);
            }
        }
    }

    private void SpawnBlocksAroundPlayer(int radius)
    {
        landTileMap.ClearAllTiles();
        waterTileMap.ClearAllTiles();
        References.Instance.mapPlantSpawner.DespawnPlants();
        DespawnMinerals();

        for (int x = currentPlayerPos.x - radius; x < currentPlayerPos.x + radius; x++)
        {
            for (int y = currentPlayerPos.y - radius; y < currentPlayerPos.y + radius; y++)
            {
                var pos = new Vector3Int(x, y);
                //if (y < 0)
                {
                    Color myColor = Color.white;
                    SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, pos, out myColor, References.Instance.mainCam, this.transform);


                    if (myColor == colorCodes[1])
                    {
                        landTileMap.SetTile(pos, correspondingTile[1]);

                        SpawnPlantIfEmptyAbove(pos);
                        SpawnMineralIfProper(pos);
                    }
                    else if (myColor == colorCodes[2])
                    {
                        landTileMap.SetTile(pos, correspondingTile[2]);

                        SpawnPlantIfEmptyAbove(pos);
                        SpawnMineralIfProper(pos);
                    }
                    else 
                    {
                        if (y < 0)
                            waterTileMap.SetTile(pos, correspondingTile[5]);
                    }
                }
            }
        }
    }

    private void DespawnMinerals()
    {
        var copiedDictionary = spawnedMinerals.ToDictionary(entry => entry.Key, entry => entry.Value);
        foreach (var mineral in copiedDictionary)
        {
            if (mineral.Value.IsSpawned())
            {
                if (mineral.Value.mineralObj != null && Vector3.Distance(mineral.Value.mineralObj.transform.position, References.Instance.playerMovingTransform.position) > mineralDespawnDistance)
                {
                    var holdsObjTemp = mineral.Value.mineralObj;
                    mineral.Value.spawned = false;
                    Destroy(holdsObjTemp);
                }
            }
        }
    }

    private void SpawnPlantIfEmptyAbove(Vector3Int pos) 
    {
        if (!minedObjects[ItemCategory.Mineral].Contains(pos) && IsSpaceAboveEmpty(pos))
        {
            References.Instance.mapPlantSpawner.SpawnPlantAt(pos, landTileMap);
        } 
    }

    private Dictionary<Vector3Int, MineralData> spawnedMinerals = new Dictionary<Vector3Int, MineralData>();
    private HashSet<Vector3Int> minedOres = new HashSet<Vector3Int>();
    
    public class MineralData
    {
        public Vector3Int tilePos;
        public Mineral mineralType;
        public bool spawned;
        public GameObject mineralObj;
        public bool hasBeenMined = false;

        public bool IsSpawned()
        {
            return spawned;
        }
    }

    private void SpawnMineralIfProper(Vector3Int pos)
    {
        if (IsAnEdgeTile(pos) && !HasOreBeenMined(pos))// && !OreAlreadySpawned(pos)) 
        {
            SpawnRarestOre(pos);
        }
    }


    private void SpawnRarestOre(Vector3Int pos) 
    {
        foreach (Item ore in ItemReferences.allMineral)
        {
            if (pos.y <= ore.spawnDepthStart && pos.y >= ore.spawnDepthStop)
            {
                float xCoord = GetXCoord(pos.x);
                float yCoord = GetYCoord(pos.y);
                float sample = Mathf.PerlinNoise(xCoord * 1.15f, yCoord * 1.15f);

                if (sample > minimumAlphaCutOff)
                {
                    System.Random random = new System.Random(mineralSeed + pos.x * 41 + pos.y * 97 + pos.x - pos.y);
                    double oreRoll = random.NextDouble();

                    if (oreRoll < ore.rarity)
                    {
                        Mineral chosenMineral = Mineral.None;

                        //it has never been spawned before
                        if (!spawnedMinerals.ContainsKey(pos))
                        {
                            chosenMineral = ore.GetMineralType();
                            MineralData mineralData = new MineralData() { tilePos = pos, hasBeenMined = false, mineralType = chosenMineral};

                            spawnedMinerals.Add(pos, mineralData);
                        }
                        //else if it has been spawned BUT this ores rarity/depth is higher
                        else if (ItemReferences.Instance.GetMineral(spawnedMinerals[pos].mineralType).rarity >= ore.rarity
                            && ItemReferences.Instance.GetMineral(spawnedMinerals[pos].mineralType).spawnDepthStart >= ore.spawnDepthStart)
                        {
                            chosenMineral = ore.GetMineralType();

                            //MineralData mineralData = new MineralData() { tilePos = pos, hasBeenMined = false, mineralType = chosenMineral };
                            //spawnedMinerals[pos] = mineralData;
                        }

                        if (chosenMineral != Mineral.None)
                        {
                            if (spawnedMinerals.ContainsKey(pos) && spawnedMinerals[pos].mineralObj == null)
                            {
                                GameObject mineralInstance = Instantiate(orePrefab, GetMineralSpawnPos(pos, currentEdgeType), mineralItemPrefab.transform.rotation, mineralParent);
                                spawnedMinerals[pos].mineralObj = mineralInstance;
                            }

                            MineralInstanceInfo mineralInfo = spawnedMinerals[pos].mineralObj.GetComponent<MineralInstanceInfo>();
                            spawnedMinerals[pos].mineralType = chosenMineral;
                            mineralInfo.SetItemToMineral(chosenMineral);
                            spawnedMinerals[pos].spawned = true;
                            //spawnedMinerals[pos].mineralObj = mineralInstance;
                        }
                    }
                }
            }
        }
    }

    private float GetYCoord(int y)
    {
        return (mineralSeed) + (y + 500000) / scale;
    }

    private float GetXCoord(int x)
    {
        return (mineralSeed) + (x + 500000) / scale;
    }

    public bool ShouldThisOreBeSpawnedInPos(Mineral mineralType, Vector3Int pos)
    {
        if (spawnedMinerals.ContainsKey(pos) && !spawnedMinerals[pos].IsSpawned() && spawnedMinerals[pos].mineralType == mineralType)
        {
            return true;
        }
        return false;
    }

    private bool HasOreBeenMined(Vector3Int pos)
    {
        if (spawnedMinerals.ContainsKey(pos) && spawnedMinerals[pos].hasBeenMined)
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    public enum EdgeType
    {
        Left,
        Right,
        Top, 
        Bottom
    }

    private EdgeType currentEdgeType;

    private Vector3 GetMineralSpawnPos(Vector3Int pos, EdgeType edgeType)
    {
        var width = landTileMap.cellSize.x;
        switch (edgeType)
        {
            case EdgeType.Left:
                return landTileMap.CellToWorld(pos) + new Vector3(0, width / 2, 0);
            case EdgeType.Right:
                return landTileMap.CellToWorld(pos) + new Vector3(width, width / 2, 0);
            case EdgeType.Top:
                return landTileMap.CellToWorld(pos) + new Vector3( width/2, width, 0);
            case EdgeType.Bottom:
                return landTileMap.CellToWorld(pos) + new Vector3(width/2, 0, 0);
            default:
                return landTileMap.CellToWorld(pos) + new Vector3(width/2, width / 2, 0);
        }
    }

    public bool IsAnEdgeTile(Vector3Int pos) 
    {
        var posRight = pos + new Vector3Int(1, 0, 0);
        var posLeft = pos + new Vector3Int(-1, 0, 0);
        var posBelow = pos + new Vector3Int(0, -1, 0);
        var posAbove = pos + new Vector3Int(0, 1, 0);

        Color colorRight = new Color();
        Color colorLeft = new Color();
        Color colorBelow = new Color();
        Color colorAbove = new Color();
        SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, posRight, out colorRight, References.Instance.mainCam, this.transform);
        SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, posLeft, out colorLeft, References.Instance.mainCam, this.transform);
        SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, posBelow, out colorBelow, References.Instance.mainCam, this.transform);
        SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, posAbove, out colorAbove, References.Instance.mainCam, this.transform);

        if (IsAnEmptySpace(colorRight)) currentEdgeType = EdgeType.Right;
        else if (IsAnEmptySpace(colorLeft)) currentEdgeType = EdgeType.Left;
        else if (IsAnEmptySpace(colorBelow)) currentEdgeType = EdgeType.Bottom;
        else if (IsAnEmptySpace(colorAbove)) currentEdgeType = EdgeType.Top;

        if (IsAnEmptySpace(colorRight) || IsAnEmptySpace(colorLeft) || IsAnEmptySpace(colorBelow) || IsAnEmptySpace(colorAbove)) 
        {
            return true;
        }
        return false;
    }

    private bool IsAnEmptySpace(Color color) 
    {
        if (color != colorCodes[1] && color != colorCodes[2])
        {
            return true;
        }
        return false;
    }

    public bool IsSpaceAboveEmpty(Vector3Int pos) 
    {
        var posAbove = pos + new Vector3Int(0, 1, 0);
        Color colorAbove = new Color();
        SpritePixelColor.GetSpritePixelColorUnderMousePointer(spriteR, posAbove, out colorAbove,References.Instance.mainCam,this.transform);
        if (colorAbove != colorCodes[1] && colorAbove != colorCodes[2]) 
        { 
            return true;
        }

        return false;
    }
  

    public bool IsTileMined(Vector3Int atPosition) 
    {
        return minedObjects[ItemCategory.Mineral].Contains(atPosition);
    }

    public bool DoesTileContainMinableObject(Vector3Int atPosition) 
    {
        return  References.Instance.mapPlantSpawner.DoesTileContainPlant(atPosition) || spawnedMinerals.ContainsKey(atPosition);
    }


    public void SetObjectToMined(Vector3Int atPosition)
    {
        foreach (var item in minedObjects)
        {
            item.Value.Add(atPosition);
        }
    }

    public void PickUpAndDestroyAllItemsAtPos(Vector3Int atPosition)
    {
        References.Instance.mapPlantSpawner.PickUpPlant(atPosition);

        if (spawnedMinerals.ContainsKey(atPosition))
        {
            minedOres.Add(atPosition);
            References.Instance.experienceControl.AddExperience(ItemReferences.Instance.GetMineral(Mineral.Iron).value);
            References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetMineral(spawnedMinerals[atPosition].mineralType), 1);
            //Debug.Log(spawnedMinerals[atPosition].mineralObj.name);
            Destroy(spawnedMinerals[atPosition].mineralObj);
            spawnedMinerals[atPosition].mineralObj = null;
            //spawnedMinerals[atPosition].mineralObj = null;
            spawnedMinerals[atPosition].spawned = false;
            spawnedMinerals[atPosition].hasBeenMined = true;
        }
    }
}
