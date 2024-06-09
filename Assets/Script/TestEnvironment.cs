using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnvironment : MonoBehaviour
{
    public bool addItemsAndExp;
    public float experienceToAdd;

    private void Update()
    {
        if (addItemsAndExp)
        {
            addItemsAndExp = false;
            var items = ItemReferences.Instance.GetAllIInvtems();
            foreach ( var item in items )
            {
                Inventory.Instance.AddItemToInventory(item, Random.Range(5,15));
            }
            References.Instance.experienceControl.AddExperience(experienceToAdd);
        }
    }
}
