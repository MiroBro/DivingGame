using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BuilderControl : MonoBehaviour
{
    public Transform buildingParent;
    public Transform furnitureParent;

    public GameObject wallPrefab;
    public GameObject roofPrefab;
    private GameObject structureInstance;

    public GameObject furniturePrefab;
    private GameObject furnitureInstance;

    public BuildingStructureState buildingStage;

    public BuildReferences.WallType buildingWallType;
    public BuildReferences.RoofType buildingRoofType;
    public BuildReferences.FurnitureType buildingFurnitureType;

    public FurnitureStage furnitureStage;


    public Structure buildingStructure;

    private float wallHeightPlacement = 0;
    private float furnitureHeightPlacement = 0;
    private float roofHeightPlacement = 2f;

    //private float roofHeight = 1.5f;
    //private float wallHeight = 2.5f;

    private float minStructureWidth = 0.5f;

    public bool snap;

    public enum Structure 
    {
        None,
        Wall,
        Roof,
    }

    public enum BuildingStructureState 
    {
        None,
        StartBuildingStructure,
        StructureFollowCursor,
        DetermineStructureWidth,
        PlaceStructure,
    }

    public enum FurnitureStage
    {
        None,
        CreateFurniture,
        FurnitureFollowCursor,
        PlaceFurniture,
    }

    // Update is called once per frame
    void Update()
    {
        var snappedX = snap ? (float)Math.Round(References.Instance.mouseWorldPos.x * 2, MidpointRounding.AwayFromZero) / 2 : References.Instance.mouseWorldPos.x;
        var wallPos = new Vector2(snappedX, wallHeightPlacement);
        var furniturePos = new Vector2(snappedX, furnitureHeightPlacement);
        var roofPos = new Vector2(snappedX, roofHeightPlacement);

        BuildStructureIfProper(snappedX, buildingStructure == Structure.Roof ? roofPos : wallPos);
        PlaceFurnitureIfProper(snappedX, furniturePos);

    }

    private void PlaceFurnitureIfProper(float snappedX, Vector2 underCursorPos)
    {
        if (furnitureStage == FurnitureStage.CreateFurniture) 
        {
            furnitureStage = FurnitureStage.FurnitureFollowCursor;

            GameObject prefab = BuildReferences.Instance.GetFurniture(buildingFurnitureType).furniturePrefab;
            furnitureInstance = Instantiate(prefab, underCursorPos, prefab.transform.rotation, furnitureParent);
            furnitureInstance.transform.position = underCursorPos;
        }
        else if (furnitureStage == FurnitureStage.FurnitureFollowCursor)
        {
            furnitureInstance.transform.position = underCursorPos;
        }
        else if (furnitureStage == FurnitureStage.PlaceFurniture)
        {
            furnitureInstance = null;
            furnitureStage = FurnitureStage.None;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (furnitureStage == FurnitureStage.FurnitureFollowCursor)
            {
                furnitureStage = FurnitureStage.PlaceFurniture;
            }
        }
    }

    private void BuildStructureIfProper(float snappedX, Vector2 underCursorPos)
    {
        if (buildingStage == BuildingStructureState.PlaceStructure)
        {
            buildingStage = BuildingStructureState.StructureFollowCursor;

            if (buildingStructure == Structure.Wall) 
            {
                GameObject prefab = BuildReferences.Instance.GetWall(buildingWallType).wallPrefab;
                structureInstance = Instantiate(prefab, underCursorPos, prefab.transform.rotation, buildingParent);
            }
            else 
            {
                GameObject prefab = BuildReferences.Instance.GetRoof(buildingRoofType).roofPrefab;
                structureInstance = Instantiate(prefab, underCursorPos, prefab.transform.rotation, buildingParent);
            }

            structureInstance.transform.position = underCursorPos;
            //structureInstance.transform.localScale = new Vector2(1, structureInstance.transform.localScale.y);
        }
        else if (buildingStage == BuildingStructureState.StructureFollowCursor)
        {
            structureInstance.transform.position = underCursorPos;
        }
        else if (buildingStage == BuildingStructureState.DetermineStructureWidth)
        {
            float width = structureInstance.transform.position.x - snappedX;

            if (width > -minStructureWidth && width < minStructureWidth) 
            {
                width = width < 0 ? -minStructureWidth : minStructureWidth;
            }

            structureInstance.transform.localScale = new Vector2(width, structureInstance.transform.localScale.y);
            
            //structureInstance.transform.localScale = new Vector2((structureInstance.transform.position.x - snappedX), structureInstance.transform.localScale.y);

        }


        if (Input.GetMouseButtonDown(0))
        {
            if (buildingStage == BuildingStructureState.StructureFollowCursor)
            {
                buildingStage = BuildingStructureState.DetermineStructureWidth;
            }
            else if (buildingStage == BuildingStructureState.DetermineStructureWidth)
            {
                buildingStage = BuildingStructureState.None;
                buildingStructure = Structure.None;
                structureInstance = null;
            }
        }
    }

    public void StartBuildingWall(BuildReferences.WallType type) 
    {
        if (furnitureInstance != null || structureInstance != null)
        {
            ResetBuildingState();
        }
        else
        {
            buildingStructure = Structure.Wall;
            buildingWallType = type;
            buildingStage = BuildingStructureState.PlaceStructure;
        }
    }

    public void StartBuildingRoof(BuildReferences.RoofType type)
    {
        if (furnitureInstance != null || structureInstance != null)
        {
            ResetBuildingState();
        }
        else
        {
            buildingStructure = Structure.Roof;
            buildingRoofType = type;
            buildingStage = BuildingStructureState.PlaceStructure;
        }
    }

    public void StartPlacingFurniture(BuildReferences.FurnitureType type)
    {
        if (furnitureInstance != null || structureInstance != null)
        {
            ResetBuildingState();
        }
        else 
        {
            buildingFurnitureType = type;
            furnitureStage = FurnitureStage.CreateFurniture;
        }
    }

    public void ResetBuildingState() 
    {
        if (furnitureInstance != null)
        {
            Destroy(furnitureInstance.gameObject);
            furnitureInstance = null;

        }

        if (structureInstance != null)
        {
            Destroy(structureInstance.gameObject);
            structureInstance = null;

        }

        furnitureStage = FurnitureStage.None;
        buildingStage = BuildingStructureState.None;
        buildingStructure = Structure.None;
    }
}
