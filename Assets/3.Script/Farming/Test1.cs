using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject melonSeedPrefab;
    public Material leafMaterial;

    private void OnCollisionEnter(Collision collision) 
    {
        // 충돌한 오브젝트의 메터리얼을 가져옴
        Renderer renderer = collision.gameObject.GetComponent<Renderer>();
        
        if(renderer != null && renderer.material==leafMaterial) 
        {
            // 충돌한 오브젝트를 melonSeedPrefab으로 변경
            Vector3 position = collision.transform.position;
            Quaternion rotation = collision.transform.rotation;

            // 기존 오브젝트를 삭제
            Destroy(collision.gameObject);

            // 새로운 프리팹 인스턴스 생성
            Instantiate(melonSeedPrefab, position, rotation);
        }
    }
}
