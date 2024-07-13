using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // ������ ������Ʈ �迭
    public GameObject plane; // �ٴ� ������Ʈ
   
    public int minObjects = 10; // �ּ� ������ ������Ʈ ��
    public int maxObjects = 100; // �ִ� ������ ������Ʈ ��
   
    private float planeWidth;
    private float planeHeight;
   
    private void Start()
    {
        // plane�� ũ�⸦ ���
        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        planeWidth = planeRenderer.bounds.size.x;
        planeHeight = planeRenderer.bounds.size.z;

        // ������Ʈ�� �����ϰ� ����
        SpawnObjects();
    }
   
    private void SpawnObjects()
    {
        // ������ ������Ʈ ���� �����ϰ� ����
        int numberOfObjects = Random.Range(minObjects, maxObjects + 1);
   
        for (int i = 0; i < numberOfObjects; i++)
        {
            // ������ ������Ʈ�� �����ϰ� ����
            GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
   
            // plane ���� ���� ��ġ ���
            float randomX = Random.Range(-planeWidth / 2, planeWidth / 2);
            float randomZ = Random.Range(-planeHeight / 2, planeHeight / 2);
   
            // ���� ��ǥ�� ��ȯ
            int intX = Mathf.RoundToInt(randomX);
            int intZ = Mathf.RoundToInt(randomZ);
   
            // ���� ��ǥ�� ����Ͽ� ���� ��ġ ����
            Vector3 randomPosition = new Vector3(intX, plane.transform.position.y, intZ) + plane.transform.position;
   
            // ������Ʈ ����
            Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        }
    }
}
