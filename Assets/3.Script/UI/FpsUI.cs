using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FpsUI : MonoBehaviour
{
    // ========== Inspector public ==========

    [SerializeField] private Sprite[] fps_image;

    // ========== Inspector private ==========

    private Image default_image = null;
    private float deltaTime;

    private void Awake()
    {
        default_image = this.GetComponent<Image>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        float fps = Mathf.Ceil(1.0f / deltaTime);

        if (fps >= 240)
        {
            default_image.sprite = fps_image[0];
        }
        else if (fps >= 120)
        {
            default_image.sprite = fps_image[1];
        }
        else if (fps >= 60)
        {
            default_image.sprite = fps_image[2];
        }
        else if (fps >= 30)
        {
            default_image.sprite = fps_image[3];
        }
        else if (fps >= 10)
        {
            default_image.sprite = fps_image[4];
        }
        else
        {
            default_image.sprite = fps_image[5];
        }
    }
}