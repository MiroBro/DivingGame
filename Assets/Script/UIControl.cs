using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour
{
    public TextMeshProUGUI experienceText;
    public GameObject fishMinigameUi;

    public GameObject buildMenu;
    public GameObject buildToggleBtn;

    public GameObject inventoryUi;

    public GameObject miniInfoUI;
    public TextMeshProUGUI miniInfoText;

    public GameObject applyingItemUi;
    public Image applyingItemIcon;

    public GameObject pickUpItem;
    public TextMeshProUGUI pickUpItemText;
    public Image pickUpItemIcon;

    public GameObject expPopUpUi;
    public TextMeshProUGUI expPopUpUiText;

    public GameObject buildingBtnPrefab;
    public Transform buildingBtnParent;
    public Transform buildingCategoryUi;


    private (DateTime, float) lastExpData;
    private TimeSpan expTimeSpan = TimeSpan.FromSeconds(3);
    Coroutine expIncRoutine;
    Dictionary<Item, (DateTime, int)> recentItems = new Dictionary<Item, (DateTime, int)>();
    private TimeSpan incTime = TimeSpan.FromSeconds(3);
    private Coroutine pickUpRoutine;

    private void Start()
    {
        UpdateExperienceText();
    }

    private void Update()
    {
        miniInfoUI.transform.position = Input.mousePosition;
        applyingItemUi.transform.position = Input.mousePosition;
    }

    public void OpenBuildingWallCategory()
    {
        RemoveBuildingBtns();
        foreach (var item in BuildReferences.Instance.allWalls)
        {
            var inst = Instantiate(buildingBtnPrefab, buildingBtnParent);
            inst.GetComponent<BuildBtnControl>().SetToWallType(item.type);
        }
        buildingCategoryUi.gameObject.SetActive(true);
    }

    public void OpenBuildingRoofCategory()
    {
        RemoveBuildingBtns();
        foreach (var item in BuildReferences.Instance.allRoofs)
        {
            var inst = Instantiate(buildingBtnPrefab, buildingBtnParent);
            inst.GetComponent<BuildBtnControl>().SetToRoofType(item.type);
        }
        buildingCategoryUi.gameObject.SetActive(true);
    }

    public void OpenBuildingFurnitureCategory()
    {
        RemoveBuildingBtns();
        foreach (var item in BuildReferences.Instance.allFurniture)
        {
            var inst = Instantiate(buildingBtnPrefab, buildingBtnParent);
            inst.GetComponent<BuildBtnControl>().SetToFurnitureType(item.type);
        }
        buildingCategoryUi.gameObject.SetActive(true);
    }


    private void RemoveBuildingBtns()
    {
        foreach (Transform btn in buildingBtnParent.transform)
        {
            Destroy(btn.gameObject);
        }
    }

    public void ToggleBuildingCategory()
    {
        buildingCategoryUi.gameObject.SetActive(!buildingCategoryUi.gameObject.activeSelf);
    }

    public void TurnOffBuildingCategory()
    {
        buildingCategoryUi.gameObject.SetActive(false);
    }

    public void ShowApplyingItemImage(Item itemType) 
    {
        applyingItemIcon.sprite = itemType.GetIcon();
        applyingItemUi.SetActive(true);
    }

    public void HideApplyingItemImage()
    {
        applyingItemUi.SetActive(false);
    }

    public void ShowMiniInfo(string miniInfo) 
    {
        miniInfoUI.SetActive(true);
        miniInfoText.text = miniInfo;
    }

    public void HideMiniInfo() 
    {
        miniInfoUI.SetActive(false);
    }

    public void UpdateExperienceText()
    {
        experienceText.text = $"Experience: {References.Instance.experienceControl.experience}";
    }

    public void CloseFishingMiniGameUi()
    {
        Time.timeScale = 1.0f;
        References.Instance.fishingHandler.CatchFish();
        fishMinigameUi.SetActive(false);
    }

    public void OpenFishingMiniGameUi() 
    {
        Time.timeScale = 0;
        fishMinigameUi.SetActive(true);
    }

    public void ToggleBuildMenu() 
    {
        buildMenu.SetActive(!buildMenu.activeSelf);
    }

    public void TurnOffBuildMenu() 
    {
        buildMenu.SetActive(false);
    }

    public void TurnOffBuildingOption()
    {
        buildMenu.SetActive(false);
        References.Instance.builderControl.ResetBuildingState();
        buildToggleBtn.GetComponent<Button>().interactable = false;
    }

    public void TurnOnBuildingOption()
    {
        buildToggleBtn.GetComponent<Button>().interactable = true;
    }

    public void ToggleInventory() 
    {
        inventoryUi.SetActive(!inventoryUi.activeSelf);
    }

    public void TurnOffInventory()
    {
        inventoryUi.SetActive(false);
    }

    public void TurnOnInventory()
    {
        inventoryUi.SetActive(true);
    }

    public void ShowPickUpItem(Item type, int amount)
    {
        if (pickUpRoutine != null)
            StopCoroutine(pickUpRoutine);   
        pickUpRoutine = StartCoroutine(ShowPickedUp(type, amount));
    }

    public void ShowExpInc(float amount)
    {
        if (expIncRoutine != null)
            StopCoroutine(expIncRoutine);

        expIncRoutine = StartCoroutine(ShowExpGained(amount));
    }

    IEnumerator ShowPickedUp(Item type, int amount)
    {
        var timeNow = DateTime.Now;

        if(recentItems.ContainsKey(type) && (timeNow - recentItems[type].Item1 >= incTime))
        {
            recentItems.Remove(type);
        }

        if (recentItems.ContainsKey(type))
        {
            recentItems[type] = (timeNow, recentItems[type].Item2 + 1);
           // (float, int)
        } else
        {
            recentItems.Add(type, (timeNow, amount));
        }

        pickUpItem.SetActive(true);
        pickUpItemIcon.sprite = type.GetIcon();  
        pickUpItemText.text = type.GetName() + " +" + recentItems[type].Item2;

        yield return new WaitForSeconds(1.5f);

        pickUpItem.SetActive(false);
    }

    IEnumerator ShowExpGained(float amount) 
    {
        var now = DateTime.Now;

        if (now - lastExpData.Item1 > expTimeSpan)
        {
            lastExpData = (now, amount);
        }
        else
        {
            lastExpData = (now, lastExpData.Item2 + amount);
        }

        expPopUpUi.SetActive(true);
        expPopUpUiText.text = "+ " + lastExpData.Item2 + " XP";

        yield return new WaitForSeconds(1.5f);

        expPopUpUi.SetActive(false);
    }
}








/*
public static void AddOre(int amount) 
{
    oreAmount += amount;
}
*/


/*
foreach (var fish in Inventory.Instance.fishes)
{
    stringBuilder.AppendLine($"{ItemReferences.Instance.fishTypes[fish.Key].fishName}: {fish.Value}");
}
foreach (var item in Inventory.Instance.ores)
{
    stringBuilder.AppendLine($"{ItemReferences.Instance.itemTypes[item.Key].itemName}: {item.Value}");
}
itemText.text = stringBuilder.ToString();
*/