using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hunger_Module : MonoBehaviour
{
    [SerializeField] private Vector3 original_scale;
    [SerializeField] private int grow_level;
    [SerializeField] private float grow_time;
    [SerializeField] private float hunger_max;
    [SerializeField] private float hunger_current;

    private void Awake()
    {
        Grow();
        hunger_current = hunger_max;
    }

    private void Update()
    {
        hunger_current = Mathf.Max(hunger_current - Time.deltaTime, 0f);
        if (hunger_current > hunger_max / 2f) grow_time += Time.deltaTime;

        if(grow_time >= hunger_max && grow_level < 3)
        {
            grow_time = 0f;
            grow_level++;
            Grow();
        }
    }

    private void Grow()
    {
        switch (grow_level)
        {
            case 1:
                gameObject.transform.localScale = new Vector3(original_scale.x, original_scale.y, original_scale.z);
                break;
            case 2:
                gameObject.transform.localScale = new Vector3(original_scale.x * 2f, original_scale.y * 2f, original_scale.z * 2f);
                break;
            case 3:
                gameObject.transform.localScale = new Vector3(original_scale.x * 3f, original_scale.y * 3f, original_scale.z * 3f);
                break;
        }
    }

    private void OnTriggerEnter(Collider item)
    {
        if (item.CompareTag("Food"))
        {
            float hunger_amount =  item.GetComponent<ItemComponent>().hungerAmount;
            if(hunger_current + hunger_amount <= hunger_max)
            {
                hunger_current += hunger_amount;
                Destroy(item.gameObject);
            }
        }
    }
}
