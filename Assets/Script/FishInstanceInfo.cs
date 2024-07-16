using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishInstanceInfo : MonoBehaviour
{

    //public ItemReferences.ItemType type;

    public Fish fishType;
    public Guid id;
    public SpriteRenderer fishSprite;

    public void SetId(Guid newId) 
    {
        id = newId;
    }

    public void SetToFishType( Fish fishType)
    {
        this.fishType = fishType;
        fishSprite.sprite = ItemReferences.Instance.GetFishSprite(fishType);
    }

    public void SetFishToTypeSetInEditor()
    {
        fishSprite.sprite = ItemReferences.Instance.GetFishSprite(fishType);
    }
}
