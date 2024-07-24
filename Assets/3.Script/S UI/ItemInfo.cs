using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_Type
{
    Weapon,
    Equipment,
    Item,
    Use,
}

[System.Serializable]
public class ItemInfo
{
    public string item_name;
    public Sprite item_image;
    public Item_Type item_type;

    public bool ItemUse() //trye 
    {
        bool isUsed = false;

        isUsed = true;

        return isUsed;
    }
}