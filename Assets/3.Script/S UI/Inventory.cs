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

    // ========== Inspector private ==========

    bool[] result; // test

    public bool on_off_tr = false;
    private bool end_crafting_tr = false;

    private void Start()
    {
        result = new bool[4]; // test

        on_off_obj.SetActive(false);
        result_crafting_slot_obj.SetActive(false);
    }

    private void Update()
    {
        if (on_off_tr)
        {
            CraftingSlot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj.SetActive(on_off_tr);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (result_crafting_slot_obj.transform.childCount == 0)
            {
                result_crafting_slot_obj.SetActive(true);

                GameObject item = Instantiate(item_obj[0]);

                item.transform.localScale = new Vector3(5, 5, 1); // test

                item.transform.SetParent(result_crafting_slot_obj.transform);

                Debug.Log("아이템 생성 성공!");
            }
        }
    }

    private void CraftingSlot() // 수정 필요
    {
        result[0] = crafting_slot[0].childCount == 1;
        result[1] = crafting_slot[1].childCount == 1;
        result[2] = crafting_slot[2].childCount == 1;
        result[3] = crafting_slot[3].childCount == 1;

        if (result[0] && result[2]) NewItem(0);
        else if (result[0] && result[3]) NewItem(1);

        DeleteItem();
    }

    private void ResultCraftingSlot()
    {

    }

    private void NewItem(int item_id)
    {
        if (result_crafting_slot_obj.transform.childCount == 0)
        {
            result_crafting_slot_obj.SetActive(true);

            GameObject item = Instantiate(item_obj[item_id]);
            item.transform.localScale = new Vector3(5, 5, 1); // test
            item.transform.SetParent(result_crafting_slot_obj.transform);

            end_crafting_tr = true;
        }
    }

    private void DeleteItem() // 아이템이 전부 다 삭제 됨
    {
        if (end_crafting_tr)
        {
            for (int i = 0; i < crafting_slot.Length; i++)
            {
                if (crafting_slot[i].childCount == 1)
                {
                    if (result[i])
                    {
                        Destroy(crafting_slot[i].GetChild(0).gameObject);
                    }
                }
            }
        }
    }
}