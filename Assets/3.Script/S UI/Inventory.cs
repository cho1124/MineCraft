using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // ========== Inspector public ==========

    [Header("아이템 종류")]
    [SerializeField] private GameObject[] item_obj = null;

    [Header("조합 슬롯")]
    [SerializeField] private Transform[] crafting_slot = null;

    [Header("결과 조합 슬롯")]
    [SerializeField] private GameObject result_crafting_slot_obj = null;

    [Header("인벤토리")]
    [SerializeField] private GameObject on_off_obj = null;

    public bool click_tr = false;

    public bool[] crafting_slot_tr = new bool[8]; // 수정: 배열 크기를 8로 변경

    public bool on_off_tr = false;

    // ========== Inspector private ==========

    // ========== Test ==========

    [SerializeField] private int[] item_id = new int[4];

    [SerializeField] private bool[] item_tr = new bool[4];

    [SerializeField] private bool[] check1_tr = new bool[4];

    private void Start()
    {
        on_off_obj.SetActive(false);
        result_crafting_slot_obj.SetActive(false);
    }

    private void Update()
    {
        if (on_off_tr)
        {
            CraftingSlot1(0, "Pickaxe item");
            CraftingSlot1(4, "Shovel item"); // 수정: value 값을 4로 변경
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj.SetActive(on_off_tr);
        }
    }

    private void CraftingSlot1(int value, string item_name)
    {
        for (int i = 0; i < 4; i++) // 수정: 인덱스 범위를 고정값으로 변경
        {
            if (i + value < crafting_slot.Length) // 수정: 인덱스가 배열 범위를 초과하지 않도록 체크
            {
                if (crafting_slot[i + value].childCount == 1 && crafting_slot[i + value].GetChild(0).name == item_name)
                {
                    crafting_slot_tr[i + value] = true;
                }
                else
                {
                    crafting_slot_tr[i + value] = false;
                }
            }
        }

        CraftingSlot2();
    }

    private void CraftingSlot2()
    {
        // 수정: 배열 길이를 벗어나지 않도록 조건을 체크
        if (crafting_slot_tr.Length >= 4 && crafting_slot_tr[0] && crafting_slot_tr[1] && !crafting_slot_tr[2] && !crafting_slot_tr[3])
            NewItem(0, "Pickaxe item");

        // 수정: 배열 길이를 벗어나지 않도록 조건을 체크
        if (crafting_slot_tr.Length >= 8 && !crafting_slot_tr[4] && !crafting_slot_tr[5] && crafting_slot_tr[6] && crafting_slot_tr[7])
            NewItem(1, "Shovel item");
    }

    private void NewItem(int item_index, string item_name)
    {
        if (result_crafting_slot_obj.transform.childCount == 0)
        {
            result_crafting_slot_obj.SetActive(true);

            GameObject item = Instantiate(item_obj[item_index]);
            item.transform.localScale = new Vector3(5, 5, 1); // test
            item.transform.SetParent(result_crafting_slot_obj.transform);

            Debug.Log($"{item_name} 아이템 제작 완료!");

            DeleteItem();
        }
    }

    private void DeleteItem()
    {
        for (int i = 0; i < crafting_slot.Length; i++)
        {
            if (i < crafting_slot_tr.Length && crafting_slot_tr[i] && click_tr)
            {
                Destroy(crafting_slot[i].GetChild(0).gameObject);
            }
        }
    }

    private void ItemInfo()
    {
        for (int i = 0; i < crafting_slot.Length; i++)
        {
            if (crafting_slot[i].childCount == 1)
            {
                switch (crafting_slot[i].GetChild(0).name)
                {
                    case "Pickaxe item":
                        item_id[i] = 1; // 예시로 1로 설정
                        break;
                    case "Shovel item":
                        item_id[i] = 2; // 예시로 2로 설정
                        break;
                }
            }
        }
    }
}
