using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectText : MonoBehaviour
{
    // ========== Inspector private ==========

    private RectTransform rt = null;

    private float scale = 0.1f;

    private void Awake()
    {
        rt = this.GetComponent<RectTransform>();
    }

    private void FixedUpdate()
    {
        this.rt.localScale += new Vector3(Time.fixedDeltaTime * scale, Time.fixedDeltaTime * scale, 0);

        if (this.rt.localScale.x >= 1.2 && this.rt.localScale.y >= 1.2)
        {
            this.rt.localScale = new Vector3(1, 1, 1);
        }
    }
}