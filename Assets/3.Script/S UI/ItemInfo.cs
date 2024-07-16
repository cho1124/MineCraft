using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfoManager : MonoBehaviour
{
    public static ItemInfoManager instance = null;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}