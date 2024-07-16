using UnityEngine;

public class HairSegment : MonoBehaviour
{
    public int length;
    public LineRenderer lineRend;
    public Vector3[] segmentPoses;
    private Vector3[] segmentV;

    public Transform targetDir;
    public float targetDist;
    public float maxDist;
    public float smoothSpeed;
    public float trailSpeed;

    public float wiggleSpeed;
    public float wiggleMagnitude;
    public Transform wiggleDir;

    public float minValue;  // Add this variable to set the maximum y-value for segments

    private bool hasInit = false;

    private void Start()
    {
        if (!hasInit)
            InitHairStrandDetails();
    }

    private void InitHairStrandDetails()
    {
        hasInit = true;

        lineRend.positionCount = length;
        segmentPoses = new Vector3[length];
        segmentV = new Vector3[length];

        // Initialize segment positions
        for (int i = 0; i < length; i++)
        {
            segmentPoses[i] = transform.position;
        }
    }

    private void Update()
    {
        SetHairToCorrectPositions();
    }

    public void SetHairToCorrectPositions()
    {
        if (!hasInit)
            InitHairStrandDetails();

        float deltaTime = Time.deltaTime;

        // Update wiggle direction
        wiggleDir.localRotation = Quaternion.Euler(0f, 0f, (Mathf.Sin(Time.time * wiggleSpeed) * wiggleMagnitude) - Mathf.Sin(wiggleMagnitude) / 2);

        // Ensure the first segment follows the player's position exactly
        segmentPoses[0] = transform.position;

        // Set the position of the first segment in the LineRenderer immediately
        lineRend.SetPosition(0, segmentPoses[0]);

        // Update the rest of the segments
        for (int i = 1; i < segmentPoses.Length; i++)
        {
            // Calculate the target position for the current segment
            Vector3 targetPosition = segmentPoses[i - 1] + targetDir.right * targetDist;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPosition, ref segmentV[i], smoothSpeed * deltaTime + i / (trailSpeed * deltaTime));

            // Check distance and clamp to maxDist
            float distance = Vector3.Distance(segmentPoses[i], segmentPoses[i - 1]);
            if (distance > maxDist)
            {
                segmentPoses[i] = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * maxDist;
            }

            // Clamp the y-value to maxYValue
            if (segmentPoses[i].y < minValue)
            {
                segmentPoses[i].y = minValue;
            }
        }

        // Update the LineRenderer with the new positions
        lineRend.SetPositions(segmentPoses);
    }

    public void ResetHair()
    {
        // Update the rest of the segments
        for (int i = 0; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = transform.position;
        }

        // Update the LineRenderer with the new positions
        lineRend.SetPositions(segmentPoses);
    }

    public void SetHairColor(Gradient color)
    {
        lineRend.colorGradient = color;
    }
}
