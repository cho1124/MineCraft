using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Handler : MonoBehaviour
{
    //[SerializeField] public GameObject target { get; private set; }
    public GameObject target;
    [SerializeField] private float tracking_time = 0f;

    private void Update()
    {
        if(target != null)
        {
            tracking_time += Time.deltaTime;
            if (tracking_time >= 10f)
            {
                target = null;
                tracking_time = 0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && other.transform.parent.gameObject != gameObject && (target == null || target.activeSelf == false)) target = other.transform.parent.gameObject;
    }
}
