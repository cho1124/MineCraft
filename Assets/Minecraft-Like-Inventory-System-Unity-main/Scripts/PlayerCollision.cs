using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField] private ItemComponent itemComponent;
    //public GameObject InventoryObj;

    private void Start()
    {
        


    }

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

            //if (AddItem(itemComponent))
            //{
            //    itemComponent.DestroyItem();
            //}




            

            //GetComponent<inventory>().items.Add(itemComponent);

            //Debug.Log(itemComponent.itemIcon.name);
            //Inventory.Instance.SpawnCollidedItem(itemComponent);
            Inventory.instance.AddItem(itemComponent);


            //SpawnCollidedItem(itemComponent);
        }

    }



    private void GetItem(GameObject gameObject)
    {

    }


}