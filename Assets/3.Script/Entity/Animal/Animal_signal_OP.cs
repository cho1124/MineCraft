using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_signal_OP : MonoBehaviour
{
   // /*
   //  
   //  동물들 머리에 띄우는 heartObjectPrefab과 shockObjectPrefab 오브젝트 풀링해서 20개씩 만들어서 사용하려고
   // 시도 
   //  
   //   */
   //
   // public GameObject prefab;
   // public int initialSize = 20;
   // private Queue<GameObject> pool = new Queue<GameObject>();
   //
   // void Start()
   // {
   //     for(int i=0; i<initialSize;i++)
   //     {
   //         GameObject obj = Instantiate(prefab);
   //         obj.SetActive(false);
   //         pool.Enqueue(obj);
   //
   //     }
   // }
   //
   // public GameObject GetObject()
   // {
   //     if(pool.Count>0)
   //     {
   //         GameObject obj = pool.Dequeue();
   //         obj.SetActive(true);
   //         return obj;
   //     }
   //     else
   //     {
   //         GameManager obj = Instantiate(prefab);
   //         return obj;
   //     }
   // }
   //
   // public void ReturnObject(GameObject obj)
   // {
   //     obj.SetActive(false);
   //     pool.Enqueue(obj);
   //
   // }
    void Update()
    {
        
    }
    
}

