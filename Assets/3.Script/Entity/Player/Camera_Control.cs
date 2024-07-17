using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Control : MonoBehaviour
{
    [SerializeField] private GameObject rotation_anchor;
    
    void Update()
    {
        transform.position = rotation_anchor.transform.position + rotation_anchor.transform.forward * -7f;
        transform.LookAt(rotation_anchor.transform.position + rotation_anchor.transform.forward * 5f);
    }
}
