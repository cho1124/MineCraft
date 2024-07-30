using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSlot : InventorySlot
{
    
    public int itemID = -1;
    public int quantity;

    public void AddItem(int newItemID, int newQuantity, Sprite newIcon)
    {
        itemID = newItemID;
        quantity = newQuantity;
        
    }

    public void ClearSlot()
    {
        itemID = -1;
        quantity = 0;
        
    }
}

