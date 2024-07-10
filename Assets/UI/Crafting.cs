using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] Transform[] slots = null;
    [SerializeField] GameObject obj;

    private void Update()
    {
        Combination_item();
    }

    public void Combination_item()
    {
        if (slots[0].childCount == 1 && slots[1].childCount == 1 && slots[2].childCount == 1 && slots[3].childCount == 1)
        {
            obj.SetActive(true);

            Debug.Log("아이템 생성! ");



            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(false);
            }
        }
    }
}