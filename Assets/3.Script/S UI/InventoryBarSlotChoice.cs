using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryBarSlotChoice : MonoBehaviour
{
    [SerializeField] private GameObject[] item_obj = null;
    [SerializeField] private Inventory Inventory_class = null;
    [SerializeField] private Transform[] inventory_bar_slot = null;
    [SerializeField] private GameObject player = null; // 플레이어 오브젝트
    [SerializeField] private float throwDistance = 2.0f; // 플레이어 앞에 떨어질 거리
    [SerializeField] private int throwing_power = 10; // 던지기 힘

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

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (this.transform.localPosition == slot_choice_pos[i] && inventory_bar_slot[i].childCount == 1)
                    {
                        // 아이템 정보를 가져옵니다.
                        Transform itemTransform = inventory_bar_slot[i].GetChild(0);
                        if (itemTransform == null)
                        {
                            Debug.LogWarning("아이템이 존재하지 않습니다.");
                            return;
                        }

                        GameObject item = itemTransform.gameObject;
                        ItemInfo iteminfo = item.GetComponent<ItemInfo>();
                        if (iteminfo == null)
                        {
                            Debug.LogWarning("아이템 정보가 없습니다.");
                            Destroy(item);
                            return;
                        }

                        // Ensure the item_id is valid
                        if (iteminfo.item_id < 0 || iteminfo.item_id >= item_obj.Length || item_obj[iteminfo.item_id] == null)
                        {
                            Debug.LogWarning("유효하지 않은 item_id.");
                            Destroy(item);
                            return;
                        }

                        // 새 아이템 오브젝트를 인스턴스화 합니다.
                        GameObject new_item_obj = Instantiate(item_obj[iteminfo.item_id]);

                        // 플레이어의 앞쪽에 아이템을 배치
                        Vector3 playerPosition = player.transform.position;
                        Vector3 playerForward = player.transform.forward;
                        Vector3 dropPosition = playerPosition + playerForward * throwDistance;
                        new_item_obj.transform.position = dropPosition;

                        // 아이템 오브젝트를 씬으로 이동합니다.
                        new_item_obj.transform.SetParent(null);

                        // Add force for throwing
                        Rigidbody ry = new_item_obj.GetComponent<Rigidbody>();
                        if (ry != null)
                        {
                            ry.AddForce(playerForward * throwing_power, ForceMode.Impulse);
                        }
                        else
                        {
                            Debug.LogWarning("새 아이템 오브젝트에 Rigidbody가 없습니다.");
                        }

                        // 아이템 제거 후 로그 출력
                        Destroy(item);
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
    [SerializeField] private GameObject[] item_obj = null;

    // ========== Inspector public ==========

    [SerializeField] private Inventory Inventory_class = null;

    [SerializeField] private Transform[] inventory_bar_slot = null;

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

                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (this.transform.localPosition == slot_choice_pos[i] && inventory_bar_slot[i].childCount == 1)
                    {
                        ItemInfo iteminfo = inventory_bar_slot[i].GetChild(0).gameObject.GetComponent<ItemInfo>();

                        GameObject new_item_obj = Instantiate(item_obj[iteminfo.item_id]);

                        Destroy(inventory_bar_slot[i].GetChild(0).gameObject);

                        Debug.Log("아이템 버리기 완료!");

                        break;
                    }
                }
            }
        }
    }
} */