using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleCollider : MonoBehaviour
{
    public Collider2D objColl;
    public void TurnOffEdgeColliderShortly() 
    {
        StopAllCoroutines();
        StartCoroutine(TurnOnAndOff(0.5f));
    }

    IEnumerator TurnOnAndOff(float time) 
    {
        objColl.enabled = false;
        yield return new WaitForSeconds(time);
        objColl.enabled = true;
    }
}
