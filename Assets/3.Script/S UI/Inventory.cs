using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // ========== Inspector public ==========

    [Header("아이템")]
    [SerializeField] private GameObject item_obj = null;

    [Header("조합 슬롯")]
    [SerializeField] private Transform[] crafting_slot = null;

    [Header("결과 조합 슬롯")]
    [SerializeField] private GameObject result_crafting_slot_obj = null;

    [Header("인벤토리")]
    [SerializeField] private GameObject on_off_obj = null;

    [Header("디버그")]
    public bool on_off_tr = false;
    [SerializeField] private bool[] crafting_slot_tr = { false }; // test
    [SerializeField] private bool full_crafting_slot_tr = false;

    // ========== Inspector private ==========

    private InventoryItemControl InventoryItemControl_class = null;

    public Transform test; // test

    private void Awake()
    {
        InventoryItemControl_class = FindObjectOfType<InventoryItemControl>();
    }

    private void Start()
    {
        on_off_obj.SetActive(false);
        result_crafting_slot_obj.SetActive(false);
    }

    private void Update()
    {
        if (on_off_tr)
        {
            CraftingSlot();
            ResultCraftingSlot();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            on_off_tr = !on_off_tr;

            on_off_obj.SetActive(on_off_tr);
        }
    }

    private void CraftingSlot()
    {
        if (crafting_slot[0].childCount == 1 && crafting_slot[1].childCount == 1 && crafting_slot[2].childCount == 1 && crafting_slot[3].childCount == 1)
        {
            full_crafting_slot_tr = true;
        }
        else
        {
            full_crafting_slot_tr = false;
        }
    }

    private void ResultCraftingSlot()
    {
        // Debug.Log($"Click: {InventoryItemControl_class.click_tr}, full: {full_crafting_slot_tr}");

        if (InventoryItemControl_class.click_tr && full_crafting_slot_tr)
        {
            for (int i = 0; i < crafting_slot.Length; i++)
            {
                Destroy(crafting_slot[i].GetChild(0).gameObject);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space)) // 임시 치트 키
        {
            if (test.childCount == 0)
            {
                result_crafting_slot_obj.SetActive(false);

                GameObject item = Instantiate(item_obj);

                item.transform.localScale = new Vector3(5, 5, 1);

                item.transform.SetParent(result_crafting_slot_obj.transform);

                Debug.Log("아이템 생성 성공!");
            }
        }
    }
}