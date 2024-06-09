using UnityEngine;

public class AquariumFishInstanceController : MonoBehaviour
{
    public float speed = 2f;             // Speed of the fish
    public float turnSpeed = 2f;         // Turning speed when reaching the target
    public float noiseFrequency = 1f;    // Frequency of the noise
    public float minX = -5f;             // Minimum X boundary
    public float maxX = 5f;              // Maximum X boundary
    public float minY = -5f;             // Minimum Y boundary
    public float maxY = 5f;              // Maximum Y boundary
    public float minHoverTime = 1f;      // Minimum time to hover at the target position
    public float maxHoverTime = 3f;      // Maximum time to hover at the target position
    public float smoothStartTime = 1f;   // Time for smooth start when transitioning from hover to move

    public int seed;

    private Vector3 targetPosition;
    private Vector3 currentVelocity;      // Used for SmoothDamp
    private bool isHovering;
    private float hoverTimer;
    private float hoverTime;
    private float smoothStartTimer;
    public BoxCollider2D edges;
    public SpriteRenderer fishSprite;

    void Start()
    {
        seed = UnityEngine.Random.Range(0, 100000);
        SetRandomTargetPosition();
    }

    public void SetColldiderEdges(BoxCollider2D edges)
    {
        this.edges = edges;
    }

    public void SetToFishType(Fish fishType)
    {
        //fishSprite.sprite = ItemReferences.Instance.GetFish(fishType).GetIcon();
        fishSprite.sprite = ItemReferences.Instance.GetFishSprite(fishType);
    }

    void Update()
    {
        if (isHovering)
        {
            HoverAtTarget();
        }
        else
        {
            MoveTowardsTarget();
        }
    }

    void MoveTowardsTarget()
    {
        // If transitioning from hover to move, apply smooth start
        if (smoothStartTimer < smoothStartTime)
        {
            smoothStartTimer += Time.deltaTime;
            float smoothStartFactor = smoothStartTimer / smoothStartTime;
            speed = Mathf.Lerp(0f, 2f, smoothStartFactor); // Adjust 2f based on your desired max speed
        }

        // Calculate the sinusoidal and noise movement
        float noise = Mathf.PerlinNoise(Time.time * noiseFrequency, 0) - 0.5f;
        float sinMovement = Mathf.Sin(Time.time * speed) * noise;

        // Calculate the new position using SmoothDamp
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + new Vector3(sinMovement, noise, 0), ref currentVelocity, 0.3f, speed);

        // Rotate towards the target position
        //Vector3 direction = targetPosition - transform.position;
        // float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);

        // Check if the fish has reached the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isHovering = true;
            hoverTimer = 0f;
            smoothStartTimer = 0f;
        }
    }

    void HoverAtTarget()
    {
        hoverTimer += Time.deltaTime;

        // Calculate the sinusoidal and noise movement during the hover
        float noise = Mathf.PerlinNoise(seed + Time.time * noiseFrequency, 0) - 0.5f;
        float sinMovement = Mathf.Sin(Time.time * speed) * noise;

        // Hover in a noisy sinusoidal movement
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition + new Vector3(sinMovement, noise, 0), ref currentVelocity, 0.3f, speed);

        // Check if the hover time is complete
        if (hoverTimer >= hoverTime)
        {
            isHovering = false;
            SetRandomTargetPosition();
        }
    }

    void SetRandomTargetPosition()
    {
        // Set a new random target position within the specified boundaries
        var maxXX = edges.gameObject.transform.position.x + ((edges.size.x / 2) * edges.gameObject.transform.localScale.x);
        var minXX = edges.gameObject.transform.position.x - ((edges.size.x / 2) * edges.gameObject.transform.localScale.x);

        var maxYY = edges.gameObject.transform.position.y + ((edges.size.y / 2) * edges.gameObject.transform.localScale.y);
        var minYY = edges.gameObject.transform.position.y - ((edges.size.y / 2) * edges.gameObject.transform.localScale.y);

        float randomX = Random.Range(minXX, maxXX);
        float randomY = Random.Range(minYY, maxYY);
        targetPosition = new Vector3(randomX, randomY, 0);

        // Set a new random hover time within the specified range
        hoverTime = Random.Range(minHoverTime, maxHoverTime);
    }
}
