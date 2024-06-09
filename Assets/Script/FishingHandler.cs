using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingHandler : MonoBehaviour
{
    public Guid currentFishId;
    public GameObject lure;
    public void StartFishing(Guid fishId) 
    {
        lure.SetActive(false);
        currentFishId = fishId;
        References.Instance.uiControl.OpenFishingMiniGameUi();
    }

    public void CatchFish() 
    {
        References.Instance.fishSpawnControl.CatchFish(currentFishId);
    }
}
