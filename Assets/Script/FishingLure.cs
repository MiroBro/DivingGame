using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FishingLure : MonoBehaviour
{
    //public Transform fishingLure;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Fish")) 
        {
            References.Instance.fishSpawnControl.GetFishInstance(collision.GetComponent<FishInstanceInfo>().id).moveTowardsLure = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision != null && collision.CompareTag("Fish"))
        {
            //Debug.Log("Encountered fish! " + collision.gameObject.GetComponent<FishMovement>().id);
            var fish = collision.gameObject.GetComponent<FishInstanceInfo>();
            References.Instance.fishSpawnControl.GetFishInstance(collision.GetComponent<FishInstanceInfo>().id).moveTowardsLure = false;
        }
    }
}
