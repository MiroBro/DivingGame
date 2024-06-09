using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ExperienceControl : MonoBehaviour
{
    public float experience = 0;

    public void AddExperience(float exp)
    {
        experience += exp;
        References.Instance.uiControl.UpdateExperienceText();
        References.Instance.uiControl.ShowExpInc(exp);
    }
}
