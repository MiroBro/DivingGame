using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private static Inventory _instance;

    public static Inventory Instance { get { return _instance; } }


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public GameObject inventorySlotParent;
    public GameObject bigInventorySlotParent;
    public GameObject inventorySlotPrefab;

    public const int smallInventorySlotCount = 10;
    public const int bigInventorySlotCount = 40;

    public Dictionary<Item, int> inventoryItems = new Dictionary<Item, int>();

    private GameObject[] allInventorySlots = new GameObject[smallInventorySlotCount + bigInventorySlotCount];

   // public static int clickAndHeldSlot;
   // public static int releasedCursorOnSlot;


    private void Start()
    {
        PopulateInventoryUiWithSlots();

        SortInventoryUi(SortingType.ByName);
    }

    public enum SortingType 
    {
        ByName,
        ByCount,
        ByType,
    }

    public void SortByName() 
    {
        SortInventoryUi(SortingType.ByName);
    }

    public void SortByCount()
    {
        SortInventoryUi(SortingType.ByCount);
    }

    public void SortByType()
    {
        SortInventoryUi(SortingType.ByType);
    }

    public void SortInventoryUi(SortingType sortingType = SortingType.ByName)
    {
        ResetInventorySlots();

        var list = inventoryItems.ToList<KeyValuePair<Item, int>>();
        List<KeyValuePair<Item, int>> sortedList;

        switch (sortingType)
        {
            case SortingType.ByName:
                sortedList = list.OrderBy(v => v.Key.GetName()).ToList();
                break;
            case SortingType.ByCount:
                sortedList = list.OrderBy(v => v.Value).ToList();
                break;
            case SortingType.ByType:
                sortedList = list.OrderBy(v => ItemReferences.IsFish(v.Key)).ToList();
                break;
            default:
                sortedList = list.OrderBy(v => v.Key.GetName()).ToList();
                break;
        }



        foreach (var item in sortedList)
        {
            GetNextFreeInventorySlot().GetComponent<InventorySlot>().SetSlotToItem(item.Key, item.Value);
        }
    }

    private GameObject GetNextFreeInventorySlot() 
    {
        foreach (var slot in allInventorySlots)
        {
            if (slot.GetComponent<InventorySlot>().IsEmpty())
                return slot;
        }
        return null;
    }


    private void ResetInventorySlots()
    {
        for (int i = 0; i < allInventorySlots.Length; i++)
        {
            allInventorySlots[i].GetComponent<InventorySlot>().SetSlotToEmpty();
        }
    }

    private void PopulateInventoryUiWithSlots()
    {
        foreach (Transform child in inventorySlotParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in bigInventorySlotParent.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < smallInventorySlotCount; i++)
        {
            var slot = Instantiate(inventorySlotPrefab, inventorySlotParent.transform);
            slot.GetComponent<InventorySlot>().SetSlotToEmpty();
            slot.GetComponent<InventorySlot>().inventoryOrder = i;
            allInventorySlots[i] = slot;
        }

        for (int i = smallInventorySlotCount; i < (smallInventorySlotCount + bigInventorySlotCount); i++)
        {
            var slot = Instantiate(inventorySlotPrefab, bigInventorySlotParent.transform);
            slot.GetComponent<InventorySlot>().SetSlotToEmpty();
            slot.GetComponent<InventorySlot>().inventoryOrder = i;
            allInventorySlots[i] = slot;
        }
    }

    public void AddItemToInventory(Item itemType, int amount)
    {
        if (inventoryItems.ContainsKey(itemType))
            inventoryItems[itemType] += amount;
        else
            inventoryItems.Add(itemType, amount);


        int indexOfItem = GetInventoryIndexOfItem(itemType);
        //SortInventoryUi();

        if (indexOfItem >= 0)
        {
            allInventorySlots[indexOfItem].GetComponent<InventorySlot>().SetSlotToItem(itemType, inventoryItems[itemType]);
        }
        else
        {
            var slot = GetNextFreeInventorySlot();
            if (slot != null)
            {
                slot.GetComponent<InventorySlot>().SetSlotToItem(itemType, amount);
            } else
            {
                Debug.LogWarning("Um, out of inventory space so can't add anything new...");
            }
        }
    }

    private void RefreshInventorySlots()
    {
        foreach (var slot in allInventorySlots)
        {
            InventorySlot invSlot = slot.GetComponent<InventorySlot>();

            if (!invSlot.IsEmpty())
            {
                invSlot.SetSlotToItem(invSlot.itemType, inventoryItems[invSlot.itemType]);
            } else
            {
                invSlot.SetSlotToEmpty();
            }
        }
    }

    private int GetInventoryIndexOfItem(Item type)
    {
        foreach (var item in allInventorySlots)
        {
            if (item.GetComponent<InventorySlot>().itemType == type) 
            {
                return item.GetComponent<InventorySlot>().inventoryOrder;
            }            
        }
        return -1;
    }

    public void RemoveItemFromInventory(Item itemType, int amount)
    {
        if (inventoryItems.ContainsKey(itemType)) 
        {
            inventoryItems[itemType] -= amount;

            if (inventoryItems[itemType] <= 0)
                inventoryItems.Remove(itemType);

            RefreshInventorySlots();
            //SortInventoryUi();
        }
        else 
        {
            Debug.Log("Uh trying to remove item that you don't have");
        }

    }

    
    public void SwapSlots(int slot1, int slot2) 
    {
        var s1 = allInventorySlots[slot1].GetComponent<InventorySlot>();
        var s1Amount = s1.amount;
        var s1Type = s1.itemType;

        var s2 = allInventorySlots[slot2].GetComponent<InventorySlot>();

        s1.SetSlotToItem(s2.itemType, s2.amount);
        s2.SetSlotToItem(s1Type, s1Amount);
    }
}
