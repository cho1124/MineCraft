using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private Transform find_inventory_slot = null;

    [SerializeField] private float speed = 100;

    private void Update()
    {
        this.transform.Rotate(0, Time.deltaTime * speed, 0);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            for (int i = 0; i <= find_inventory_slot.childCount; i++)
            {
                if (find_inventory_slot.GetChild(i).childCount == 0)
                {
                    this.transform.SetParent(find_inventory_slot.GetChild(i));

                    break;

                    // 수정 필요
                }
            }
        }
    }
}