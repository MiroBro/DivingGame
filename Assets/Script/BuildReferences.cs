using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildReferences : MonoBehaviour
{
    private static BuildReferences _instance;

    public static BuildReferences Instance { get { return _instance; } }


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

    public Roof[] allRoofs;
    public Wall[] allWalls;
    public Furniture[] allFurniture;

    [System.Serializable]
    public class Roof
    {
        public GameObject roofPrefab;
        public RoofType type;
        public int cost;
        public string name;
        public Sprite uiIcon;
    }

    [System.Serializable]
    public class Wall
    {
        public GameObject wallPrefab;
        public WallType type;
        public int cost;
        public string name;
        public Sprite uiIcon;
    }

    [System.Serializable]
    public class Furniture
    {
        public GameObject furniturePrefab;
        public FurnitureType type;
        public int cost;
        public string name;
        public Sprite uiIcon;
    }
    public enum RoofType
    {
        None,
        Roof1,
        Roof2,
        Roof3,
        Roof4,
        Roof5,
    }

    public enum WallType
    {
        None,
        Wall1,
        Wall2,
    }

    public enum FurnitureType
    {
        None,
        Chair,
        Bed,
    }

    public Wall GetWall(WallType type)
    {
        foreach (var item in allWalls)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }

    public Roof GetRoof(RoofType type)
    {
        foreach (var item in allRoofs)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }

    public Furniture GetFurniture(FurnitureType type)
    {
        foreach (var item in allFurniture)
        {
            if (item.type == type)
                return item;
        }
        return null;
    }
}
