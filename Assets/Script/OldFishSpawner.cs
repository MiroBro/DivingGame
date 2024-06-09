using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class OldFishSpawner : MonoBehaviour
{
    public GameObject fishPrefab;
    public Tilemap tilemap;
    public float spawnDistance = 10f;
    public float despawnDistance = 15f;
    public float perlinScale = 0.1f;
    public float maxSpawnY = 5f;

    private Transform playerTransform;

    void Start()
    {
        playerTransform = Camera.main.transform;
    }

    void Update()
    {
        Vector3 playerPosition = playerTransform.position;

        Vector3 spawnPosition = GetFishSpawnPosition(playerPosition);

        // Get the tilemap bounds in world space
        BoundsInt bounds = tilemap.cellBounds;
        TileBase tile = null;

        // Check if there is a tile at the spawn position
        if (tilemap.GetTile(new Vector3Int((int)spawnPosition.x, (int)spawnPosition.y, 0)) != tile)
        {
            // Check if the spawn position is below the maximum y-location
            if (spawnPosition.y <= maxSpawnY)
            {
                SpawnFish(spawnPosition);
            }
        }

        DespawnFish(playerPosition);
    }

    Vector3 GetFishSpawnPosition(Vector3 playerPosition)
    {
        float perlinX = playerPosition.x * perlinScale;
        float perlinY = playerPosition.y * perlinScale;

        float xOffset = UnityEngine.Random.Range(-spawnDistance, spawnDistance);
        float yOffset = UnityEngine.Random.Range(-spawnDistance, spawnDistance);

        Vector3 spawnPosition = new Vector3(playerPosition.x + xOffset, playerPosition.y + yOffset, 0f);
        spawnPosition += new Vector3(Mathf.PerlinNoise(perlinX, perlinY), Mathf.PerlinNoise(perlinY, perlinX)) * spawnDistance;

        return spawnPosition;
    }

    void SpawnFish(Vector3 position)
    {
        Instantiate(fishPrefab, position, Quaternion.identity);
        // The fish prefab should handle its movement on its own using its existing script.
    }

    void DespawnFish(Vector3 playerPosition)
    {
        GameObject[] fishes = GameObject.FindGameObjectsWithTag("Fish");

        foreach (GameObject fish in fishes)
        {
            float distanceToPlayer = Vector3.Distance(playerPosition, fish.transform.position);

            if (distanceToPlayer > despawnDistance)
            {
                Destroy(fish);
            }
        }
    }
}
