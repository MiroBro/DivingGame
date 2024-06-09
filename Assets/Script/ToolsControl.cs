using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;
using UnityEngine.XR;

public class ToolsControl : MonoBehaviour
{
    //public Vector3 mousePosition;
    public Transform originPosition;
    public Transform beamToRotate;
    Vector3 objectPosition;
    float angle;

    public Transform beamOrigin;
    public Transform beamEnd;

    public Tilemap worldTilemap;
    public Tilemap zappedTilemap;

    float timeToDestroyBlock = 1f;

    public Tile breakTile1;
    public Tile breakTile2;
    public Tile breakTile3;

    public GameObject zapFX;
    public GameObject zapFXParent;
    public GameObject destroyFX;

    public Dictionary<Vector3Int, float> timesTilesBeenZapped = new Dictionary<Vector3Int, float>();

    public GameObject beamParent;

    public bool hittingBreakable = false;
    public Mineral hittingBreakableType;

    public LineRenderer line;
    public Sprite beam1;
    public Sprite beam2;
    public Sprite beam3;
    public float maxLureDistanceFromPlayer = 2;

    public bool useNewBreakingSystem = false;

    public GameObject hitBigEffect;
    public GameObject hitSmallEffect;

    Vector3 mouseWorldPos;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(objectPosition, 5);

    }

    void Update()
    {
        MoveLureToCursor();
        mouseWorldPos = References.Instance.mouseWorldPos;

        Vector3 direction = mouseWorldPos - originPosition.position;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        beamToRotate.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

        if (hittingBreakable)
        {
            zapFXParent.SetActive(true);
        }
        else
        {
            line.transform.gameObject.SetActive(false);
            zapFXParent.SetActive(false);
        }
    }

    public void BlastBeam()
    {
        Vector3 direction = mouseWorldPos - originPosition.position;
        hittingBreakable = false;
        beamParent.SetActive(true);
        // Cast a ray
        Vector3 rayOrigin = beamOrigin.transform.position;
        float distance = Vector3.Distance(beamEnd.position, beamOrigin.position);

        Debug.DrawRay(rayOrigin, direction, Color.green);

        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, distance);

        line.SetPosition(0, beamOrigin.transform.position);
        line.SetPosition(1, beamEnd.transform.position);
        line.transform.gameObject.SetActive(true);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.transform.CompareTag("Breakable"))
            {
                if (useNewBreakingSystem)
                {
                    Vector2 norm = direction.normalized * 0.05f;
                    var tilePos = worldTilemap.WorldToCell(hit.point + norm);//(hit.collider.transform.position);
                    zapFXParent.transform.position = hit.point;
                    line.SetPosition(1, hit.point);

                    hittingBreakable = true;

                    if (!References.Instance.mapSpawner.IsTileMined(tilePos) && References.Instance.mapSpawner.DoesTileContainMinableObject(tilePos))
                    {
                        if (timesTilesBeenZapped.ContainsKey(tilePos))
                        {
                            hitSmallEffect.SetActive(false);
                            hitBigEffect.SetActive(true);
                            zapFX.SetActive(true);

                            var time = Time.deltaTime;
                            timesTilesBeenZapped[tilePos] += time;

                            float timeZapped = timesTilesBeenZapped[tilePos];
                            float thirdWay = timeToDestroyBlock * 0.33f;
                            float twoThirdWays = timeToDestroyBlock * 0.67f;

                            if (timeZapped > thirdWay && timeZapped < twoThirdWays)
                            {
                                zappedTilemap.SetTile(tilePos, breakTile2);
                            }
                            else if (timeZapped > twoThirdWays && timeZapped < timeToDestroyBlock)
                            {
                                zappedTilemap.SetTile(tilePos, breakTile3);
                            }
                            else if (timesTilesBeenZapped[tilePos] >= timeToDestroyBlock)
                            {
                                DestroyWhatsOnTile(tilePos);

                                timesTilesBeenZapped.Remove(tilePos);
                            }
                        }
                        else
                        {
                            timesTilesBeenZapped.Add(tilePos, 0);
                            if (worldTilemap.GetTile(tilePos) != null)
                            {
                                zappedTilemap.SetTile(tilePos, breakTile1);
                            }
                        }
                        break;
                    }
                    else 
                    {
                        hitSmallEffect.SetActive(true);
                        hitBigEffect.SetActive(false);
                        zapFX.SetActive(false);

                    }
                }
                else
                {

                    Vector2 norm = direction.normalized * 0.05f;
                    var tilePos = worldTilemap.WorldToCell(hit.point + norm);//(hit.collider.transform.position);

                    zapFXParent.transform.position = hit.point;
                    line.SetPosition(1, hit.point);

                    if (timesTilesBeenZapped.ContainsKey(tilePos))
                    {
                        var time = Time.deltaTime;
                        timesTilesBeenZapped[tilePos] += time;

                        float timeZapped = timesTilesBeenZapped[tilePos];
                        float thirdWay = timeToDestroyBlock * 0.33f;
                        float twoThirdWays = timeToDestroyBlock * 0.67f;

                        if (timeZapped > thirdWay && timeZapped < twoThirdWays)
                        {
                            zappedTilemap.SetTile(tilePos, breakTile2);
                        }
                        else if (timeZapped > twoThirdWays && timeZapped < timeToDestroyBlock)
                        {
                            zappedTilemap.SetTile(tilePos, breakTile3);
                        }
                        else if (timesTilesBeenZapped[tilePos] >= timeToDestroyBlock)
                        {
                            DestroyTile(tilePos);

                            timesTilesBeenZapped.Remove(tilePos);
                        }
                    }
                    else
                    {
                        timesTilesBeenZapped.Add(tilePos, 0);
                        if (worldTilemap.GetTile(tilePos) != null)
                        {
                            zappedTilemap.SetTile(tilePos, breakTile1);
                        }
                    }
                    hittingBreakable = true;
                    break;
                }
            }
        }
    }

    public void ResetBreakableHit()
    {
        hittingBreakable = false;
    }

    public void ToggleBeamState(bool state)
    {
        beamParent.SetActive(state);
    }

    public void ToggleLureState(bool state)
    {
        References.Instance.fishingHandler.lure.SetActive(state);
    }

    public void MoveLureToCursor()
    {
        var originPoint = References.Instance.playerMovingTransform.position;
        var direction = (References.Instance.mouseWorldPos - References.Instance.playerMovingTransform.position).normalized;
        Vector3 newPosition = originPoint + new Vector3(direction.x, direction.y, 0) * maxLureDistanceFromPlayer;
        References.Instance.fishingLure.transform.position = newPosition;
    }

    private void DestroyWhatsOnTile(Vector3Int tilePos)
    {
        //if (References.Instance.underwaterGenerator.GetOreAtPosition(tilePos) != Mineral.None)
        //{
        //    References.Instance.experienceControl.AddExperience(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)).value);//(int)ItemReferences.Instance.GetItem(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)).value);
        //}
        //else
        //{
            References.Instance.experienceControl.AddExperience(1);
        //}

        //if (References.Instance.underwaterGenerator.GetOreAtPosition(tilePos) != Mineral.None)
        //{
        //    Inventory.Instance.AddItemToInventory(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)), 1);
        //    References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)), 1);
        //}

        //References.Instance.underwaterGenerator.SetObjectToMined(ItemCategory.Mineral, tilePos);
        //References.Instance.underwaterGenerator.PickUpAndDestroyAllItemsAtPos(tilePos);

        References.Instance.mapSpawner.SetObjectToMined(tilePos);
        References.Instance.mapSpawner.PickUpAndDestroyAllItemsAtPos(tilePos);

        //worldTilemap.SetTile(tilePos, null);
        zappedTilemap.SetTile(tilePos, null);
        var temp = worldTilemap.CellToWorld(tilePos);
        var inst = Instantiate(destroyFX, new Vector2((temp.x + worldTilemap.cellSize.x / 2), (temp.y + worldTilemap.cellSize.y / 2)), destroyFX.transform.rotation);
    }

    private void DestroyTile(Vector3Int tilePos)
    {
        if (References.Instance.underwaterGenerator.GetOreAtPosition(tilePos) != Mineral.None)
        {
            References.Instance.experienceControl.AddExperience(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)).value);//(int)ItemReferences.Instance.GetItem(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)).value);
        }
        else
        {
            References.Instance.experienceControl.AddExperience(1);
        }

        if (References.Instance.underwaterGenerator.GetOreAtPosition(tilePos) != Mineral.None)
        {
            Inventory.Instance.AddItemToInventory(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)), 1);
            References.Instance.uiControl.ShowPickUpItem(ItemReferences.Instance.GetMineral(References.Instance.underwaterGenerator.GetOreAtPosition(tilePos)), 1);
        }

        References.Instance.underwaterGenerator.SetObjectToMined(ItemCategory.Mineral, tilePos);
        References.Instance.underwaterGenerator.PickUpAndDestroyAllItemsAtPos(tilePos);

        worldTilemap.SetTile(tilePos, null);
        zappedTilemap.SetTile(tilePos, null);
        var temp = worldTilemap.CellToWorld(tilePos);
        var inst = Instantiate(destroyFX, new Vector2((temp.x + worldTilemap.cellSize.x / 2), (temp.y + worldTilemap.cellSize.y / 2)), destroyFX.transform.rotation);
    }

    private int count;
    private void SetLineMat()
    {
        count++;
        if (count % 3 == 0)
            line.material.mainTexture = beam1.texture;
        else if (count % 2 == 0)
            line.material.mainTexture = beam2.texture;
        else
            line.material.mainTexture = beam3.texture;
    }

    IEnumerator WaitCoroutine()
    {
        count++;
        if (count % 3 == 0)
            line.material.mainTexture = beam1.texture;
        else if (count % 2 == 0)
            line.material.mainTexture = beam2.texture;
        else
            line.material.mainTexture = beam3.texture;

        line.enabled = true;
        yield return new WaitForSeconds(0.15f);

        line.enabled = false;
    }
}