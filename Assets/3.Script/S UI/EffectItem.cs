using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectItem : MonoBehaviour
{
    private float speed = 100;

    private void Update()
    {
        this.transform.Rotate(0, Time.deltaTime * speed, 0);
    }
}