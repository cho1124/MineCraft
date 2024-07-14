using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public GameObject melonSeedPrefab;
    public Material leafMaterial;

    private void OnCollisionEnter(Collision collision) 
    {
        // �浹�� ������Ʈ�� ���͸����� ������
        Renderer renderer = collision.gameObject.GetComponent<Renderer>();
        
        if(renderer != null && renderer.material==leafMaterial) 
        {
            // �浹�� ������Ʈ�� melonSeedPrefab���� ����
            Vector3 position = collision.transform.position;
            Quaternion rotation = collision.transform.rotation;

            // ���� ������Ʈ�� ����
            Destroy(collision.gameObject);

            // ���ο� ������ �ν��Ͻ� ����
            Instantiate(melonSeedPrefab, position, rotation);
        }
    }
}
