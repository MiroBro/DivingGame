using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishSpawnControl;

public class FishMoveControl : MonoBehaviour
{

    public class FishInstance
    {
        public Fish fishType;
        public Guid fishGuid;
        public Vector2 position;
        public GameObject fishObject;
        public FishInstanceInfo fishInstanceInfo;
        public float repulsionForce;
        public int randomSpawnSeed;
        public Vector3 targetPosInAquarium;


        //Movement variables
        public float directionTimer = 0f;
        public float timeCounter = 0f;
        public bool moveTowardsLure = false;
        public Vector3 startPosition;

        public bool despawned = false;
        public bool caught = false;
        public FishInstance(Fish fishType, Vector2 spawnPosition, GameObject fishObj)
        {
            this.fishType = fishType;

            position = spawnPosition;
            startPosition = spawnPosition;

            fishObject = fishObj;
            
            timeCounter = UnityEngine.Random.Range(0f, 100f);

            fishGuid = new Guid();
            fishInstanceInfo = fishObj.GetComponent<FishInstanceInfo>();
            repulsionForce = UnityEngine.Random.Range(References.Instance.fishMoveControl.startRepulsionForce, References.Instance.fishMoveControl.startRepulsionForce + 1);
            randomSpawnSeed = (int)((System.DateTime.Now.Ticks + ((spawnPosition.x + spawnPosition.y) * (int) fishType) / int.MaxValue));
        }
    }

    public float moveSpeed = 1.0f;
    public float minYValue = int.MinValue;
    private float maxYValue = -1.0f;
    public float noiseScale = 0.1f;
    public float noiseMagnitude = 2.0f;
    public float minFlipTime = 3.0f;
    public float maxFlipTime = 8.0f;
    public float avoidanceDistance = 5.0f;
    public float startRepulsionForce = 2.0f;
    public float repulsionDistance = 2.0f;
    public float minLureDistance = 0.5f;

    public Transform playerTransform;

    private void Start()
    {
        playerTransform = References.Instance.playerMovingTransform;
    }
    public void MoveAllSpawnedFishes()
    {
        foreach (var fish in References.Instance.fishSpawnControl.spawnedFishes)
        {
            var fishInstance = fish.Value;
            fishInstance.directionTimer -= Time.deltaTime;


            if (fishInstance.directionTimer <= 0f)
            {
                ChangeDirection(fishInstance);
            }

            if (fishInstance.moveTowardsLure)
            {
                MoveTowardsLure(fishInstance);
            }
            else
            {
                RegularMovement(fishInstance);
            }
        }
    }

    void MoveTowardsLure(FishInstance fishInstance)
    {
        Vector3 fishingLurePosition = References.Instance.fishingLure.transform.position;

        Vector3 targetPosition = fishingLurePosition;

        fishInstance.fishObject.transform.position = Vector3.Lerp(fishInstance.fishObject.transform.position, targetPosition, Time.deltaTime);

        float distanceToLure = Vector3.Distance(fishInstance.fishObject.transform.position, fishingLurePosition);

        if (distanceToLure < minLureDistance) 
        {
            References.Instance.fishingHandler.StartFishing(fishInstance.fishInstanceInfo.id);
        }

        if (fishInstance.fishObject.transform.position.x <= targetPosition.x)
        {
            fishInstance.fishObject.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            fishInstance.fishObject.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void RegularMovement(FishInstance fishInstance)
    {
        if (fishInstance.fishObject != null)
        {
            fishInstance.timeCounter += Time.deltaTime;

            // Calculate sinusoidal movement along one axis
            float xMovement = Mathf.Sin(fishInstance.timeCounter) * moveSpeed;

            // Use Perlin noise for smooth randomness along the other axis
            float yMovement = Mathf.PerlinNoise(fishInstance.timeCounter * noiseScale, 0) * noiseMagnitude * moveSpeed;

            // Calculate the next position based on sinusoidal and Perlin noise movement
            Vector3 targetPosition = fishInstance.startPosition + new Vector3(xMovement, yMovement, 0);

            // Check if the player is too close, and adjust movement to avoid the player
            if (playerTransform != null)
            {
                float distanceToPlayer = Vector3.Distance(fishInstance.fishObject.transform.position, playerTransform.position);

                // Gradually increase the repulsion force as the player gets closer
                float scaledRepulsionForce = Mathf.Lerp(0f, fishInstance.repulsionForce, Mathf.Clamp01(distanceToPlayer / repulsionDistance));

                // Adjust the target position based on the repulsion force
                Vector3 repulsionDirection = (fishInstance.fishObject.transform.position - playerTransform.position).normalized;
                targetPosition += repulsionDirection * scaledRepulsionForce;
            }

            // Limit the fish's movement within the specified y-value range
            targetPosition.y = Mathf.Clamp(targetPosition.y, minYValue, maxYValue);

            // Move the fish towards the target position
            fishInstance.fishObject.transform.position = Vector3.Lerp(fishInstance.fishObject.transform.position, targetPosition, Time.deltaTime);

            if (fishInstance.fishObject.transform.position.x <= targetPosition.x) 
            {
                fishInstance.fishObject.transform.localScale = new Vector3(-1, 1, 1);
            } else
            {
                fishInstance.fishObject.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    void ChangeDirection(FishInstance fishInstance)
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        fishInstance.directionTimer = UnityEngine.Random.Range( minFlipTime, maxFlipTime);
    }


}
