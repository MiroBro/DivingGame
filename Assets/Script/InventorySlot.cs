using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler//, IPointerUpHandler
{
    public static InventorySlot hoveringOverSlot;
    public static InventorySlot draggingSlot;

    public TextMeshProUGUI amountText;
    public Image itemImage;

    public int amount;
    public Item itemType;
    public int inventoryOrder;

    public bool isEmpty;

    public void SetSlotToItem(Item itemType, int amount)
    {
        if (itemType == null)
        {
            SetSlotToEmpty();
        } else
        {
            itemImage.enabled = true;
            amountText.enabled = true;
            GetComponent<Button>().interactable = true;

            itemImage.sprite = itemType.GetIcon();
            amountText.text = amount.ToString();
            this.itemType = itemType;
            this.amount = amount;
            isEmpty = false;
        }
    }

    //public static bool IsDraggingItem()
    //{
    //    return draggingSlot != null;
    //}

    public void SetSlotToEmpty()
    {
        itemImage.enabled = false;
        amountText.enabled = false;
        this.itemType = null;
        isEmpty = true;
        GetComponent<Button>().interactable = false;
    }

    public bool IsEmpty()
    {
        return isEmpty;
    }


    //public void OnPointerUp(PointerEventData eventData)
    //{

    //}

    public void OnPointerDown(PointerEventData eventData)
    {
        draggingSlot = this;

        if (!isEmpty)
        {
            References.Instance.uiControl.ShowApplyingItemImage(itemType);
        }
        // Inventory.clickAndHeldSlot = inventoryOrder;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveringOverSlot = this;

        if (itemType != null)
            References.Instance.uiControl.ShowMiniInfo(itemType.GetName());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoveringOverSlot = null;

        References.Instance.uiControl.HideMiniInfo();
    }
}
