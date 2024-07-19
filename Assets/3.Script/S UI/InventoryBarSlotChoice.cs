using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBarSlotChoice : MonoBehaviour
{
    [SerializeField] Inventory inventory_class = null;

    // ========== Inspector public ==========

    [SerializeField] private Inventory Inventory_class = null;

    [SerializeField] private Transform[] inventory_bar_slot = null;

    public int inventory_select_index = 0;

    // ========== Inspector private ==========

    private Vector3[] slot_choice_pos = new Vector3[9];

    private void Start()
    {
        // 설계 사정으로 수동으로 좌표를 구했습니다.

        slot_choice_pos[0] = new Vector3(-400, -485, 0);
        slot_choice_pos[1] = new Vector3(-300, -485, 0);
        slot_choice_pos[2] = new Vector3(-200, -485, 0);
        slot_choice_pos[3] = new Vector3(-100, -485, 0);
        slot_choice_pos[4] = new Vector3(0, -485, 0);
        slot_choice_pos[5] = new Vector3(100, -485, 0);
        slot_choice_pos[6] = new Vector3(200, -485, 0);
        slot_choice_pos[7] = new Vector3(300, -485, 0);
        slot_choice_pos[8] = new Vector3(400, -485, 0);

        this.transform.localPosition = slot_choice_pos[0];
    }

    private void Update()
    {
        DropItem();
    }

    private void DropItem()
    {
        if (!Inventory_class.on_off_tr)
        {
            for (int i = 0; i < slot_choice_pos.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    this.transform.localPosition = slot_choice_pos[i];

                    break;
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (this.transform.localPosition == slot_choice_pos[i] && inventory_bar_slot[i].childCount == 1) // 
                    {
                        // inventory_bar_slot[i].GetChild(0).gameObject

                        // ItemInfo item = this.GetComponent<ItemInfo>();

                        // GameObject new_item_obj = Instantiate();

                        // Destroy(inventory_bar_slot[i].GetChild(0).gameObject);

                        Debug.Log("아이템 버리기 완료!");

                        break;
                    }
                }
            }
        }
    }
}







/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBarSlotChoice : MonoBehaviour
{
    // ========== Inspector public ==========

    [SerializeField] private Inventory Inventory_class = null;

    [SerializeField] private Transform[] inventory_bar_slot = null;

    public int inventory_select_index = 0;

    // ========== Inspector private ==========

    private Vector3[] slot_choice_pos = new Vector3[9];

    private void Start()
    {
        // 설계 사정으로 수동으로 좌표를 구했습니다.

        slot_choice_pos[0] = new Vector3(-400, -485, 0);
        slot_choice_pos[1] = new Vector3(-300, -485, 0);
        slot_choice_pos[2] = new Vector3(-200, -485, 0);
        slot_choice_pos[3] = new Vector3(-100, -485, 0);
        slot_choice_pos[4] = new Vector3(0, -485, 0);
        slot_choice_pos[5] = new Vector3(100, -485, 0);
        slot_choice_pos[6] = new Vector3(200, -485, 0);
        slot_choice_pos[7] = new Vector3(300, -485, 0);
        slot_choice_pos[8] = new Vector3(400, -485, 0);

        this.transform.localPosition = slot_choice_pos[0];
    }

    private void Update()
    {
        DropItem();
    }

    private void DropItem(int index_value)
    {
        if (!Inventory_class.on_off_tr)
        {
            for (int i = 0; i < slot_choice_pos.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    this.transform.localPosition = slot_choice_pos[i];

                    break;
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (this.transform.localPosition == slot_choice_pos[i] && inventory_bar_slot[i].childCount == 1)
                    {
                        Destroy(inventory_bar_slot[i].GetChild(0).gameObject);

                        Debug.Log("아이템 버리기 완료!");

                        break;
                    }
                }
            }
        }
    }
}
*/