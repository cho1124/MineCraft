using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Inventory : MonoBehaviour
{
    // ========== Inspector public ==========

    public Onchangeitem onchangeitem; // ?
    public delegate void Onchangeitem(); // ?
    public Transform[] slots = null; // Slots 오브젝트 선언
    [SerializeField] private List<ItemInfo> item_list = new List<ItemInfo>(); // 인벤토리 아이템 리스트 선언
    [SerializeField] private GameObject[] on_off_obj = null; // 인벤토리, Dead UI ON, OFF 선언 (Gameobject)

    // ========== Inspector private ==========

    [HideInInspector] public bool on_off_tr = false; // 인벤토리, Dead UI ON, OFF 선언 (Bool)

    private void Start()
    {
        on_off_obj[1].SetActive(on_off_tr);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // E키를 누른다면
        {
            on_off_tr = !on_off_tr; // 인벤토리 UI ON, OFF (Bool)

            on_off_obj[0].SetActive(on_off_tr); // 인벤토리 UI ON, OFF (Gameobject)
        }

        if (Input.GetKeyDown(KeyCode.P)) // P키를 누른다면 ////////// Test
        {
            on_off_tr = !on_off_tr; // Dead UI ON, OFF (Bool)

            on_off_obj[1].SetActive(on_off_tr); // Dead UI ON, OFF (Bool)
        }
    }

    public bool AddItem(ItemInfo iteminfo)
    {
        float slots_count = slots[0].childCount + slots[1].childCount; // Slots1, Slots2 자식 오브젝트들의 총 합

        if (item_list.Count <= slots_count) // 인벤토리 아이템 개수보다 슬롯 개수가 크거나 같다면
        {
            item_list.Add(iteminfo); // 인벤토리 아이템 리스트 넣기

            if (onchangeitem != null)
            {
                onchangeitem.Invoke();
            }

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