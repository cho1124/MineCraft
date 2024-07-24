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
    public List<ItemInfo> item_list = new List<ItemInfo>(); // 인벤토리 아이템 리스트 선언
    [SerializeField] private GameObject[] on_off_obj = null; // 인벤토리, Dead UI ON, OFF 선언 (Gameobject)

    // ========== Inspector private ==========

    [HideInInspector] public bool on_off_tr = false; // 인벤토리, Dead UI ON, OFF 선언 (Bool)

    private void Start() // 한번만 실행되는 생명 주기 메소드
    {
        on_off_obj[1].SetActive(on_off_tr); // 바로 인벤토리, Dead UI ON, OFF 상태 변경
    }

    private void Update() // 프레임마다 실행되는 생명 주기 메소드
    {
        if (Input.GetKeyDown(KeyCode.E)) // E키를 누른다면
        {
            on_off_tr = !on_off_tr; // 인벤토리 UI ON, OFF 체크

            on_off_obj[0].SetActive(on_off_tr); // 인벤토리 UI ON, OFF 기능
        }

        if (Input.GetKeyDown(KeyCode.P)) // P키를 누른다면 ////////// Test
        {
            on_off_tr = !on_off_tr; // Dead UI ON, OFF 체크

            on_off_obj[1].SetActive(on_off_tr); // Dead UI ON, OFF 기능
        }
    }

    public bool AddItem(ItemInfo iteminfo) // 인벤토리 아이템 추가 메소드
    {
        float slots_count = slots[0].childCount + slots[1].childCount; // Slots1, Slots2 자식 오브젝트들의 총 합

        if (item_list.Count <= slots_count) // 인벤토리 아이템 개수보다 슬롯 개수가 크거나 같다면
        {
            item_list.Add(iteminfo); // 인벤토리 아이템 리스트 넣기

            if (onchangeitem != null)
            {
                onchangeitem.Invoke();
            }

            return true; // 반환 값 True
        }

        return false; // 반환 값 False
    }

    private void OnTriggerEnter(Collider collision) // 트리거 메소드
    {
        if (collision.gameObject.CompareTag("Item")) // Item 태그 닿으면
        {
            Item item = collision.GetComponent<Item>();

            if (AddItem(item.GetItemInfo()))
            {
                item.DestroyItem();
            }
        }
    }
}