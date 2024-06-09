using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterPlantInstance : MonoBehaviour
{
    public UnderwaterPlant underwaterPlant;
    public Guid id;
    public SpriteRenderer plantSprite;

    public void SetUnderwaterPlantTo(UnderwaterPlant plant)
    {
        plantSprite.sprite = ItemReferences.Instance.GetUnderwaterSprite(plant);
        underwaterPlant = plant;
    }

    public void SetId(Guid newId)
    {
        id = newId;
    }
}
