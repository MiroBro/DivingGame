using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static FishMoveControl;
using static UnityEditor.PlayerSettings;

public class AquariumController : MonoBehaviour
{
    public GameObject auqariumFishPrefab;
    public Transform aquariumParent;
    public List<Fish> fishesInAquarium;
    public BoxCollider2D aquariumBounds;

    private void Start()
    {
        foreach (var fish in fishesInAquarium)
        {
            AddFish(fish);
        }
    }


    public void AddFish(Fish fishType)
    {
        var isnt = Instantiate(auqariumFishPrefab, aquariumBounds.gameObject.transform.position, auqariumFishPrefab.transform.rotation, aquariumParent);
        isnt.GetComponent<AquariumFishInstanceController>().SetColldiderEdges(aquariumBounds);
        isnt.GetComponent<AquariumFishInstanceController>().SetToFishType(fishType);
    }

}
