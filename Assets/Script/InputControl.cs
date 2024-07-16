using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;

public class InputControl : MonoBehaviour
{
    float horizontal;
    float vertical;
    float moveLimiter = 0.7f;

    public float swimSpeed = 20.0f;
    public float runSpeed = 20.0f;
    public float slowSwimSpeed = 2.0f;
    public float jumpSpeed = 5;
    public Tilemap tilemap;
    public GameObject beam;
    public Camera mainCam;
    public float smooth = 10;

    private bool jump;
    private bool swimSlow;
    private bool dive;
    private bool popOutOfWater;

    public Transform landPos;
    public Transform waterPos;

    public Rigidbody2D[] allHairRbs;

    //public InWorldState previousWorldState;
    public PlayerWorldState playerWorldState;

    public GameObject waterLine;

    public float waterToLandY = -1;

    private bool burstDive;
    private float burstDiveForce = -4.5f;
    private float burstDiveForceTime = 0.7f;

    private Rigidbody2D playerRb;

    public GameObject overWaterHair;
    public GameObject underWaterHair;

    public enum PlayerWorldState
    {
        OnLand,
        StandingOnWaterLine,
        Underwater,
    }

    private enum Direction
    {
        Left,
        Right
    }

    private Direction currentDirection;

    private void Start()
    {
        SetPlayerPhysicsToType(PlayerWorldState.OnLand);
    }

    void Update()
    {
        playerRb = References.Instance.playerRigidBody;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        SetMovementVariables();

        RaycastHit2D hit = Physics2D.Raycast(References.Instance.mouseWorldPos, Vector2.zero);


        if (Input.GetMouseButtonDown(1)
            && References.Instance.playerController.playerWorldState == InputControl.PlayerWorldState.Underwater)
        {
            References.Instance.toolsControl.ToggleLureState(true);
        }
        else if (Input.GetMouseButtonUp(1))
        {
            References.Instance.toolsControl.ToggleLureState(false);
            InventorySlot.draggingSlot = null;
        }

        if (Input.GetMouseButton(0)
            && References.Instance.playerController.playerWorldState == InputControl.PlayerWorldState.Underwater)
        {
            References.Instance.toolsControl.BlastBeam();
            //InventorySlot.draggingSlot = null;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            References.Instance.toolsControl.ResetBreakableHit();

            if (InventorySlot.draggingSlot != null && InventorySlot.hoveringOverSlot != null)
            {
                SwapItemSlots();
            }
            else if (hit.collider != null && hit.collider.CompareTag("Aquarium")
                && InventorySlot.draggingSlot != null
                && ItemReferences.IsFish(InventorySlot.draggingSlot.itemType))
            {
                DropFishInAquarium();
            }
            InventorySlot.draggingSlot = null;
            References.Instance.uiControl.HideApplyingItemImage();
        }
        else
        {
            References.Instance.toolsControl.ToggleBeamState(false);
        }
    }

    private void DropFishInAquarium()
    {
        References.Instance.aquariumController.AddFish((Fish)InventorySlot.draggingSlot.itemType.GetItemType());
        Inventory.Instance.RemoveItemFromInventory(InventorySlot.draggingSlot.itemType, 1);
    }

    private void SwapItemSlots()
    {
        Inventory.Instance.SwapSlots(InventorySlot.hoveringOverSlot.inventoryOrder, InventorySlot.draggingSlot.inventoryOrder);
    }

    private GameObject collWithStructure;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Structure"))
        {
            collWithStructure = collision.gameObject;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.CompareTag("Structure"))
        {
            collWithStructure = null;
        }
    }

    private void SetMovementVariables()
    {
        if (GotJumpInput())
            jump = true;

        if (Input.GetKeyDown(KeyCode.LeftShift) && playerWorldState == PlayerWorldState.Underwater)
            swimSlow = true;

        if (Input.GetKeyUp(KeyCode.LeftShift) && playerWorldState == PlayerWorldState.Underwater)
            swimSlow = false;

        if (GotDownInput() && playerWorldState == PlayerWorldState.StandingOnWaterLine)
            dive = true;

        if (GotDownInput() && collWithStructure != null)
        {
            collWithStructure.GetComponent<ToggleCollider>().TurnOffEdgeColliderShortly();
        }

        if (playerWorldState == PlayerWorldState.Underwater && References.Instance.playerMovingTransform.position.y > waterToLandY)
            popOutOfWater = true;

        if (horizontal != 0)
        {
            if (horizontal < 0)
            {
                currentDirection = Direction.Left;
            }
            else
            {
                currentDirection = Direction.Right;
            }
        }
    }

    private bool GotJumpInput()
    {
        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) && (playerWorldState == PlayerWorldState.StandingOnWaterLine || playerWorldState == PlayerWorldState.OnLand);
    }

    private static bool GotDownInput()
    {
        return (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S) || Input.GetKey(KeyCode.S));
    }

    void FixedUpdate()
    {
        playerRb = References.Instance.playerRigidBody;

        if (dive)
        {
            Dive();
        }
        else if (popOutOfWater)
        {
            PopOutOfWater();
        }


        if (playerWorldState == PlayerWorldState.OnLand || playerWorldState == PlayerWorldState.StandingOnWaterLine)
        {
            Vector2 move = References.Instance.playerRigidBody.velocity;

            move.x = horizontal * runSpeed;

            if (jump)
            {
                move.y = (playerWorldState == PlayerWorldState.StandingOnWaterLine ? 1.2f * jumpSpeed : jumpSpeed);
                jump = false;
            }
            playerRb.velocity = move;


            // Project where our velocity will take us by the end of the frame.
            Vector2 positionAtEndOfStep = playerRb.position + playerRb.velocity * Time.deltaTime;

            // Limit that projected position to within our allowed bounds.
            positionAtEndOfStep.y = Mathf.Clamp(positionAtEndOfStep.y, References.Instance.waterTouchControl.transform.position.y, float.MaxValue);

            // Compute a velocity that will take us to this clamped position instead.
            Vector3 neededVelocity = (positionAtEndOfStep - playerRb.position) / Time.deltaTime;

            // You can also calculate this as the needed velocity change/acceleration,
            // and add it as a force instead if you prefer.
            playerRb.velocity = neededVelocity;


            //if (References.Instance.playerMovingTransform.position.y <= References.Instance.waterTouchControl.transform.position.y) 
            //{
            //    playerRb.MovePosition(new Vector2(playerRb.position.x, References.Instance.waterTouchControl.transform.position.y));
            //}
        }
        else if (playerWorldState == PlayerWorldState.Underwater)
        {
            // Check for diagonal movement
            if (horizontal != 0 && vertical != 0)
            {
                horizontal *= moveLimiter;
                vertical *= moveLimiter;
            }

            Vector2 move = playerRb.velocity;
            move.x = horizontal * (swimSlow ? slowSwimSpeed : swimSpeed);

            if (burstDive)
            {
                move.y = divingBoost;
                burstDive = false;
            }

            playerRb.velocity = new Vector3(move.x, divingBoost + vertical * (swimSlow ? slowSwimSpeed : swimSpeed));
        }

        if (currentDirection == Direction.Left)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1,1);
        }
            

    }

    public void SetPlayerPhysicsToType(PlayerWorldState newWorldState)
    {
        playerRb = References.Instance.playerRigidBody;
        switch (newWorldState)
        {
            case PlayerWorldState.OnLand:
            case PlayerWorldState.StandingOnWaterLine:
                jump = false;
                playerRb.gravityScale = 2.75f;

                this.playerWorldState = PlayerWorldState.OnLand;
                break;
            case PlayerWorldState.Underwater:
                jump = false;
                playerRb.gravityScale = 0;

                this.playerWorldState = PlayerWorldState.Underwater;
                break;

        }
    }

    private void Dive()
    {
        dive = false;
        waterLine.SetActive(false);
        overWaterHair.SetActive(false);
        underWaterHair.SetActive(true);

        SetPlayerPhysicsToType(PlayerWorldState.Underwater);
        StartCoroutine(IncreaseSpeed(burstDiveForce, 0, burstDiveForceTime));
        References.Instance.uiControl.TurnOffBuildingOption();
    }

    private void PopOutOfWater()
    {
        StopAllCoroutines();
        popOutOfWater = false;
        waterLine.SetActive(true);
        overWaterHair.SetActive(true);
        underWaterHair.SetActive(false);

        SetPlayerPhysicsToType(PlayerWorldState.OnLand);
        References.Instance.playerRigidBody.velocity = new Vector3(References.Instance.playerRigidBody.velocity.x, 10);
        References.Instance.uiControl.TurnOnBuildingOption();
    }

    private float divingBoost;

    private IEnumerator IncreaseSpeed(float start, float end, float duration)
    {
        float percent = 0;
        float timeFactor = 1 / duration;
        while (percent < 1)
        {
            percent += Time.deltaTime * timeFactor;
            divingBoost = Mathf.Lerp(start, end, Mathf.SmoothStep(0, 1, percent));
            yield return null;
        }
        divingBoost = 0;
    }
}