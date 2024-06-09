using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class MapPlantSpawner : MonoBehaviour
{
    public float plantDespawnDistance = 12f; // Distance from the player to despawn fishes

    public class PlantInstance
    {
        public Guid id;
        public bool picked = false;
        public GameObject plantObject;
        public Vector2 position;
        public UnderwaterPlant plantType;
    }

    private Dictionary<Vector2, long> posFirstEncountered = new Dictionary<Vector2, long>();
    private Dictionary<Guid, PlantInstance> plantIds = new Dictionary<Guid, PlantInstance>();
    public Dictionary<Vector2, PlantInstance> spawnedPlants = new Dictionary<Vector2, PlantInstance>();
    public GameObject plantPrefab;
    public Transform plantParent;
    public static Dictionary<Vector3Int, UnderwaterPlant> plantPos = new Dictionary<Vector3Int, UnderwaterPlant>();

    public void SpawnRarestPossiblePlant(int x, int y, Tilemap landTileMap)
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

                System.Random plantRandom = new System.Random(posAndFishSpecificSeed);
                double plantRandomRoll = plantRandom.NextDouble();

                if (plantRandomRoll < plantEntry.rarity)
                {
                    GameObject plant = Instantiate(plantPrefab, new Vector3(x + landTileMap.cellSize.x / 2, y + landTileMap.cellSize.y, 0), plantPrefab.transform.rotation, plantParent);
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

    public void DespawnPlants()
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

    public void PickUpPlant(Vector3Int atPosition)
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


    //public void SpawnPlantAboveIfProper(int x, int y, Vector3Int pos, Tilemap tilemap)
    public void SpawnPlantAt(Vector3Int pos, Tilemap tilemap)
    {
            if (!spawnedPlants.ContainsKey(new Vector2(pos.x, pos.y)))
                SpawnRarestPossiblePlant(pos.x, pos.y, tilemap);
    }


    public bool DoesTileContainPlant(Vector3Int atPosition)
    {
        return spawnedPlants.ContainsKey(new Vector2(atPosition.x, atPosition.y));
    }
}
