using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
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
    public ItemType item_type;

    public bool ItemUse()
    {
        return false;
    }
}