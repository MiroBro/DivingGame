using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class HandleHairStrands : MonoBehaviour
{
    public HairStrand[] allHairStrands;
    public Gradient strandDisabledColor;
    public Gradient strandEnableColor;

    public void EnableHairs()
    {
        foreach (var hair in allHairStrands)
        {
            hair.SetHairColor(strandEnableColor);
        }
    }

    public void DisableAllHairs()
    {
        foreach (var item in allHairStrands)
        {
            item.SetHairColor(strandDisabledColor);
            //item.gameObject.SetActive(false);
        }
    }

    public void UpdateAllHairStrands()
    {
        foreach (var item in allHairStrands) 
        {
            item.SetHairToCorrectPositions();
        }
    }
}
