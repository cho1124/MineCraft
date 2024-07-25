using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        InventoryItem item = other.GetComponent<InventoryItem>();
        if (item != null)
        {
            item.PickupItem();

            Debug.Log("dsadsadsadsadsadsadsadsa");
        }
    }
}