using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Control : MonoBehaviour
{
    [SerializeField] private Transform rotation_anchor;

    private void Awake()
    {
        rotation_anchor = GameObject.Find("Player").transform.Find("Rotation_Anchor");
    }
    void Update()
    {
        transform.position = rotation_anchor.position + rotation_anchor.forward * -7f;
        transform.LookAt(rotation_anchor.position + rotation_anchor.forward * 5f);
    }
}
