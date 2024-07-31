using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ItemComponent itemComponent;
    //public GameObject InventoryObj;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            ItemComponent itemComponent = other.GetComponent<ItemComponent>();

            if (itemComponent == null)
            {
                Debug.LogError("ItemComponent is not attached to the collided object");
                return;
            }

            //AudioManager.instance.PlayRandomSFX("Humanoid", "Get");
            Inventory.instance.AddItem(itemComponent);


        }

    }

}