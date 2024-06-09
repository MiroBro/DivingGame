using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTouchControl : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector2(References.Instance.playerMovingTransform.position.x, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //References.Instance.playerController.previousWorldState = References.Instance.playerController.inWorldState;
            References.Instance.playerController.playerWorldState = InputControl.PlayerWorldState.StandingOnWaterLine;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //var holdPrev = References.Instance.playerController.previousWorldState;

            //References.Instance.playerController.previousWorldState = References.Instance.playerController.inWorldState;
            References.Instance.playerController.playerWorldState = InputControl.PlayerWorldState.OnLand;
        }
    }

    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            References.Instance.playerController.previousWorldState = References.Instance.playerController.inWorldState;
            References.Instance.playerController.inWorldState = PlayerController.InWorldState.StandingOnWaterLine;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var holdPrev = References.Instance.playerController.previousWorldState;

            References.Instance.playerController.previousWorldState = References.Instance.playerController.inWorldState;
            References.Instance.playerController.inWorldState = PlayerController.InWorldState.OnLand;
        }
    }
    */
}
