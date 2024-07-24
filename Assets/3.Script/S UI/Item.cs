using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // ========== Inspector public ==========

    [SerializeField] private ItemInfo iteminfo = null;
    [SerializeField] private SpriteRenderer image = null;

    public void SetItemInfo(ItemInfo iteminfo)
    {
        this.iteminfo.item_name = iteminfo.item_name;
        this.iteminfo.item_image = iteminfo.item_image;
        this.iteminfo.item_type = iteminfo.item_type;
        this.image.sprite = iteminfo.item_image;
    }

    public ItemInfo GetItemInfo()
    {
        return iteminfo;
    }

    public void DestroyItem()
    {
        Destroy(this.gameObject);
    }
}