using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAndSetToTarget : MonoBehaviour
{
    public float rotationSpeed;
    private Vector2 direction;

    public Transform target;

    private Vector3 previousMousePosition;

    // Start is called before the first frame update
    void Start()
    {
        previousMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 currentMousePosition = target.position;

        // Check if the mouse has moved
        if (currentMousePosition != previousMousePosition)
        {
            direction = currentMousePosition - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        }

        //Vector2 cursorPos = Camera.main.ScreenToWorldPoint(currentMousePosition);
        // Create offset, otherwise rotation will jitter
        Vector2 target2 = target.position - new Vector3(0.01f, 0f);
        transform.position = target2;

        // Update previous mouse position
        previousMousePosition = currentMousePosition;
    }
}
