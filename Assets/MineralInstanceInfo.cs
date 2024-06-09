using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MineralInstanceInfo : MonoBehaviour
{
    public Mineral mineralType;
    public SpriteRenderer iconSprite;

    public void SetItemToMineral(Mineral mineralType) 
    {
        this.mineralType = mineralType;
        //iconSprite.sprite = ItemReferences.Instance.GetMineralTile(mineralType).sprite;
        //References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetMineral(spawnedMinerals[atPosition].mineralType), 1);
        //pickUpItemIcon.sprite = type.GetIcon();  
        iconSprite.sprite = ItemReferences.Instance.GetMineral(mineralType).GetIcon();//ItemReferences.Instance.GetMineralTile(mineralType).sprite;
    }
}
