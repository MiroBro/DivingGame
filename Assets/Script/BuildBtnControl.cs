using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildBtnControl : MonoBehaviour
{
    public enum BuildType
    {
        None,
        Wall,
        Roof,
        Furniture,
    }

    public BuildType toBuild;
    public BuildReferences.WallType wallType;
    public BuildReferences.RoofType roofType;
    public BuildReferences.FurnitureType furnitureType;

    public Image buildIcon;
    public TextMeshProUGUI buildCost;
    public TextMeshProUGUI buildName;

    //This is called on the building buttins on click
    public void StartBuilding()
    {
        switch (toBuild)
        {
            case BuildType.Wall:
                References.Instance.builderControl.StartBuildingWall(wallType);
                break;
            case BuildType.Roof:
                References.Instance.builderControl.StartBuildingRoof(roofType);
                break;
            case BuildType.Furniture:
                References.Instance.builderControl.StartPlacingFurniture(furnitureType);
                break;
            default:
                break;
        }
        References.Instance.uiControl.TurnOffBuildingCategory();
    }

    public void SetToWallType(BuildReferences.WallType wallType)
    {
        toBuild = BuildType.Wall;
        this.wallType = wallType;
        this.roofType = BuildReferences.RoofType.None;
        this.furnitureType = BuildReferences.FurnitureType.None;
        buildIcon.sprite = BuildReferences.Instance.GetWall(wallType).uiIcon;
        buildName.text = BuildReferences.Instance.GetWall(wallType).name;
        buildCost.text = BuildReferences.Instance.GetWall(wallType).cost.ToString();
    }

    public void SetToRoofType(BuildReferences.RoofType roofType)
    {
        toBuild = BuildType.Roof;
        this.wallType = BuildReferences.WallType.None;
        this.roofType = roofType;
        this.furnitureType = BuildReferences.FurnitureType.None;
        buildIcon.sprite = BuildReferences.Instance.GetRoof(roofType).uiIcon;
        buildName.text = BuildReferences.Instance.GetRoof(roofType).name;
        buildCost.text = BuildReferences.Instance.GetRoof(roofType).cost.ToString();
    }

    public void SetToFurnitureType(BuildReferences.FurnitureType furnitureType)
    {
        toBuild = BuildType.Furniture;

        this.wallType = BuildReferences.WallType.None;
        this.roofType = BuildReferences.RoofType.None;
        this.furnitureType = furnitureType;
        buildIcon.sprite = BuildReferences.Instance.GetFurniture(furnitureType).uiIcon;
        buildName.text = BuildReferences.Instance.GetFurniture(furnitureType).name;
        buildCost.text = BuildReferences.Instance.GetFurniture(furnitureType).cost.ToString();
    }
}
