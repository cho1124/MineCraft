using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal_signal_OP : MonoBehaviour
{
   // /*
   //  
   //  ������ �Ӹ��� ���� heartObjectPrefab�� shockObjectPrefab ������Ʈ Ǯ���ؼ� 20���� ���� ����Ϸ���
   // �õ� 
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

