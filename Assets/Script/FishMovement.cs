using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//This script is old, was once placed on each fish prefab
public class FishMovement : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public float minYValue = int.MinValue;
    public float maxYValue = 0.0f;
    public float noiseScale = 0.1f;
    public float noiseMagnitude = 2.0f;
    public float minFlipTime = 3.0f;
    public float maxFlipTime = 8.0f;
    public float avoidanceDistance = 5.0f;
    public float repulsionForce = 2.0f;
    public float repulsionDistance = 2.0f;
    public float minLureDistance = 0.5f;

    private Transform playerTransform;
    private Vector3 startPosition;
    private float timeCounter = 0f;
    private float directionTimer = 0f;

    public bool moveTowardsLure;

    public Collider2D aquariumBounds;

    void Start()
    {
        startPosition = transform.position;
        timeCounter = Random.Range(0f, 100f);
        ChangeDirection();

        GameObject playerObject = References.Instance.playerMovingTransform.gameObject;
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0f)
        {
            ChangeDirection();
        }

        {
            // Continue with regular movement if moveTowardsLure is false
            RegularMovement();
        }
    }


    void RegularMovement()
    {
        // Update the time counter for the sinusoidal movement
        timeCounter += Time.deltaTime;

        // Calculate sinusoidal movement along one axis
        float xMovement = Mathf.Sin(timeCounter) * moveSpeed;

        // Use Perlin noise for smooth randomness along the other axis
        float yMovement = Mathf.PerlinNoise(timeCounter * noiseScale, 0) * noiseMagnitude * moveSpeed;

        // Calculate the next position based on sinusoidal and Perlin noise movement
        Vector3 targetPosition = startPosition + new Vector3(xMovement, yMovement, 0);

        // Check if the player is too close, and adjust movement to avoid the player
        if (playerTransform != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // Gradually increase the repulsion force as the player gets closer
            float scaledRepulsionForce = Mathf.Lerp(0f, repulsionForce, Mathf.Clamp01(distanceToPlayer / repulsionDistance));

            // Adjust the target position based on the repulsion force
            Vector3 repulsionDirection = (transform.position - playerTransform.position).normalized;
            targetPosition += repulsionDirection * scaledRepulsionForce;
        }

        // Limit the fish's movement within the specified y-value range
       // targetPosition.y = Mathf.Clamp(targetPosition.y, minYValue, maxYValue);

        // Correct clamping
        //targetPosition.x = Mathf.Clamp(targetPosition.x, aquariumBounds.bounds.min.x, aquariumBounds.bounds.max.x);
        //targetPosition.y = Mathf.Clamp(targetPosition.y, aquariumBounds.bounds.min.y, aquariumBounds.bounds.max.y);

        // Move the fish towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
    }

    void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 moveDirection = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0).normalized;

        directionTimer = Random.Range(minFlipTime, maxFlipTime);
    }
}


/*
 *
 *
 *
        // If moveTowardsLure is true, move towards the fishing lure position
        if (moveTowardsLure)
        {
            MoveTowardsLure();
        }
    void MoveTowardsLure()
    {
        Vector3 fishingLurePosition = References.Instance.fishingLure.transform.position;

        // Calculate the direction towards the fishing lure
        Vector3 towardsLureDirection = (fishingLurePosition - transform.position).normalized;

        // Calculate the next position towards the fishing lure
        Vector3 targetPosition = fishingLurePosition;//transform.position + towardsLureDirection * moveSpeed * Time.deltaTime;

        // Move the fish towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

        //Debug.Log("towards: " + towardsLureDirection + ", myPos: " + transform.position + ", targetPos: " + targetPosition);

        // Check if the fish is close enough to the fishing lure
        float distanceToLure = Vector3.Distance(transform.position, fishingLurePosition);

        if (distanceToLure < minLureDistance) // Adjust the threshold as needed
        {
            // Call the StartFishing method when close to the fishing lure
            References.Instance.fishingHandler.StartFishing(GetComponent<FishInstanceInfo>().id);
        }
    }
 */