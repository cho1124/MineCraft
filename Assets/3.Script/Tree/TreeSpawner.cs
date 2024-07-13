using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TreeSpawner : MonoBehaviour
{
    public GameObject[] objectsToSpawn; // 복제할 오브젝트 배열
    public GameObject plane; // 바닥 오브젝트
   
    public int minObjects = 10; // 최소 복제할 오브젝트 수
    public int maxObjects = 100; // 최대 복제할 오브젝트 수
   
    private float planeWidth;
    private float planeHeight;
   
    private void Start()
    {
        // plane의 크기를 계산
        MeshRenderer planeRenderer = plane.GetComponent<MeshRenderer>();
        planeWidth = planeRenderer.bounds.size.x;
        planeHeight = planeRenderer.bounds.size.z;

        // 오브젝트를 랜덤하게 복제
        SpawnObjects();
    }
   
    private void SpawnObjects()
    {
        // 복제할 오브젝트 수를 랜덤하게 결정
        int numberOfObjects = Random.Range(minObjects, maxObjects + 1);
   
        for (int i = 0; i < numberOfObjects; i++)
        {
            // 복제할 오브젝트를 랜덤하게 선택
            GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];
   
            // plane 내의 랜덤 위치 계산
            float randomX = Random.Range(-planeWidth / 2, planeWidth / 2);
            float randomZ = Random.Range(-planeHeight / 2, planeHeight / 2);
   
            // 정수 좌표로 변환
            int intX = Mathf.RoundToInt(randomX);
            int intZ = Mathf.RoundToInt(randomZ);
   
            // 정수 좌표를 사용하여 랜덤 위치 설정
            Vector3 randomPosition = new Vector3(intX, plane.transform.position.y, intZ) + plane.transform.position;
   
            // 오브젝트 복제
            Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
        }
    }
}
