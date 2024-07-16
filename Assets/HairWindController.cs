using UnityEngine;

public class HairWindController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    public Transform target; // Player or character transform
    public bool inWind = false;
    public float windRotationSpeed = 2f;

    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = target.position;
    }

    void Update()
    {
        Vector3 currentPosition = target.position;

        // Calculate direction based on movement or wind
        Vector3 direction = inWind ? Vector3.forward : currentPosition - previousPosition;
        Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                              (inWind ? windRotationSpeed : rotationSpeed) * Time.deltaTime);

        // Set position relative to the character
        transform.position = target.position;

        previousPosition = currentPosition;
    }
}
