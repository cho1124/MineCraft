using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Item_type
{
    Weapon,
    Item,
}

[System.Serializable]
public class ItemInfo : MonoBehaviour
{
    public string item_name;
    public int item_id;
    public Item_type item_type;
    public string item_explanation;
    public int count_max;
    public int count_current;

    public ItemInfo(string name, int id, Item_type type, string explanation, int count_max)
    {
        item_name = name;
        item_id = id;
        item_type = type;
        item_explanation = explanation;
        this.count_max = count_max;
        count_current = 1;
    }
}