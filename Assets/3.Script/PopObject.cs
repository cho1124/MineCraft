using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PopObject : MonoBehaviour
{
    [SerializeField]
    private float setScale = 0.25f;
    [SerializeField]
    private float tempRotateY;
    [SerializeField]
    private float tempPositionY = 1;

    private MeshRenderer meshRenderer;
    private MeshFilter meshFilter;
    private int vertexIndex = 0;

    private Chunk[,] chunk;
    private int chunkX, chunkY;

    [SerializeField]
    public GameObject worldObject;
    private World world;

    private Vector3 initialPosition;

    private void Start()
    {

    }
    public void Initialize(World world, Vector3 position, byte blockID)
    {


        transform.localScale = new Vector3(setScale, setScale, setScale);

        initialPosition = transform.position;

        JumpAnimation();
    }

    private void JumpAnimation()
    {
        // 기존 트윈 제거
        transform.DOKill();

        // 점프 애니메이션 설정
        transform.DOJump(initialPosition + new Vector3(0.5f, 0.5f, 0.5f), 1f, 1, 1f)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() =>
                 {
                     // 점프가 완료된 후 초기 위치 업데이트
                     initialPosition = transform.position;
                 });
    }


    private void Update()
    {
        if (tempRotateY >= 359f)
            tempRotateY = 1f;
        tempRotateY += 1f;

        transform.Rotate(new Vector3(0, tempRotateY * Time.deltaTime, 0));

        tempPositionY += Time.deltaTime;
        float newYPosition = Mathf.Sin(tempPositionY) * 0.2f + 0.5f;
        transform.position = new Vector3(initialPosition.x, initialPosition.y + newYPosition, initialPosition.z);
    }
}
