using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// THIS IS A CODE THAT GPT SUGGESSTED DOESNT WORK THOUGH!
// for this to work turn off fish movement in fish prefabs and turn off fish spawner. But it doesnt work very well
public class FishManager : MonoBehaviour
{
    public GameObject fishPrefab;
    public int numberOfFishes = 10;

    private List<FishMovementController> fishControllers = new List<FishMovementController>();

    void Start()
    {
        // Spawn fishes and create controllers
        for (int i = 0; i < numberOfFishes; i++)
        {
            SpawnFish();
        }
    }

    void Update()
    {
        // Update the movement of all fishes
        foreach (FishMovementController fishController in fishControllers)
        {
            fishController.UpdateMovement();
        }
    }

    void SpawnFish()
    {
        // Instantiate a new fish
        GameObject fishObject = Instantiate(fishPrefab, GetRandomSpawnPosition(), Quaternion.identity);

        // Add FishMovementController to the fish object
        FishMovementController fishController = fishObject.AddComponent<FishMovementController>();

        // Initialize the controller with necessary parameters
        fishController.Initialize(GetRandomMovementParameters());

        // Add the controller to the list
        fishControllers.Add(fishController);
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Replace this with your logic to get a random spawn position
        return new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0f);
    }

    FishMovementParameters GetRandomMovementParameters()
    {
        // Replace this with your logic to generate random movement parameters
        FishMovementParameters parameters = new FishMovementParameters();
        parameters.moveSpeed = Random.Range(1f, 3f);
        parameters.minYValue = -5f;
        parameters.maxYValue = 5f;
        // Add other parameters as needed
        return parameters;
    }
}

public class FishMovementParameters
{
    public float moveSpeed;
    public float minYValue;
    public float maxYValue;
    public float noiseScale;
    public float noiseMagnitude;
    public float minFlipTime;
    public float maxFlipTime;
    public float avoidanceDistance;
    public float repulsionForce;
    public float repulsionDistance;
    public float minLureDistance;
}

public class FishMovementController : MonoBehaviour
{
    private FishMovementParameters parameters;

    public void Initialize(FishMovementParameters initialParameters)
    {
        parameters = initialParameters;
        // Add any additional initialization logic if needed
    }

    public void UpdateMovement()
    {
        // Replace this with your logic to update fish movement using parameters
        // For example, you can modify transform.position based on parameters
        transform.Translate(Vector3.right * parameters.moveSpeed * Time.deltaTime);
    }
}
