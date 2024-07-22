using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    // ========== Inspector public ==========

    [Header("아이템 리스트")]
    [SerializeField] private List<ItemInfo> item_list = new List<ItemInfo>();
    [Header("인벤토리")]
    [SerializeField] private GameObject[] on_off_obj = null;
    public delegate void Onchangeitem();
    public Onchangeitem onchangeitem;

    // ========== Inspector private ==========

    [HideInInspector] public bool on_off_tr = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj[0].SetActive(on_off_tr);
        }

        if (Input.GetKeyDown(KeyCode.P)) // test
        {
            on_off_tr = !on_off_tr;

            on_off_obj[1].SetActive(on_off_tr);
        }
    }

    public bool AddItem(ItemInfo iteminfo)
    {
        if (item_list.Count < 27)
        {
            item_list.Add(iteminfo);

            if (onchangeitem != null)

            onchangeitem.Invoke();

            return true;
        }

        return false;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();

            if (AddItem(item.GetItemInfo()))
            {
                item.DestroyItem();
            }
        }
    }
}