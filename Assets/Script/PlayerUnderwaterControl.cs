using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.Tilemaps;

public class PlayerUnderwaterControl : MonoBehaviour
{
    /*
    Rigidbody2D body;

    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;
    */

    public float runSpeed = 20.0f;



    public Tilemap tilemap;

    public GameObject beam;

    public Camera mainCam;


    public float movementSpeed = 15;
    public Vector3 rotationSpeed = new Vector3(0, 40, 0);
    private Rigidbody2D rb;
    private Vector2 inputDirection;

    private Vector2 inputs;

    private bool rotate;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputs = new Vector2(-Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        inputDirection = inputs.normalized;
        //Debug.Log(inputs);

        //inputForward = 

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            rotate = true;
        } else
        {

            rotate = false;
           // rb.freezeRotation = true;
           // rb.angularVelocity = 0;
        }
    }

    private void FixedUpdate()
    {
        if (rotate)
        {
           //rb.freezeRotation = false;
            Quaternion deltaRotation = Quaternion.Euler(inputDirection.x * rotationSpeed * Time.deltaTime);
            rb.MoveRotation(rb.transform.rotation * deltaRotation);
            rb.MovePosition(rb.transform.position + transform.up * movementSpeed * inputDirection.y * Time.deltaTime);
        }
    }

}
