using UnityEngine;
using System.Collections;
using Cinemachine;

public class WormAI : MonoBehaviour
{

    [Header("��չ��� ���")]
    [SerializeField] private CinemachineSmoothPath path;
    [SerializeField] private CinemachineDollyCart cart;
    [SerializeField] LayerMask terrainLayer = default;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Player_Control player;

    private Vector3 startPosition;
    private Vector3 endPosition;

    RaycastHit hitInfo;


    void Start()
    {
        cart.m_Path = path;
        cart.m_Speed = speed;
        StartCoroutine(FollowPath());
        player = FindObjectOfType<Player_Control>();


    }



    


    //1�� ���� : �⺻ ����
    //    //2�� ���� : ���� y�����θ� �ڱ��ƴ� �������� ����

    void UpdatePath()
    {
        Vector3 playerPosition = player.transform.position + (player.GetComponent<Rigidbody>().velocity * 3);
        playerPosition.y = Mathf.Max(10, playerPosition.y);
        Vector3 randomRange = Random.insideUnitSphere * 100;
        randomRange.y = 0;
        startPosition = playerPosition + randomRange;
        endPosition = playerPosition - randomRange;

        if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            startPosition = hitInfo.point;

        }

        if (Physics.Raycast(endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        {
            endPosition = hitInfo.point;
            //GroundDetection.Invoke(false, hitInfo.transform.CompareTag("Terrain") ? 0 : 1);
        }

        path.m_Waypoints[0].position = startPosition + (Vector3.down * 15);
        path.m_Waypoints[1].position = playerPosition + (Vector3.up * 10);
        path.m_Waypoints[2].position = endPosition + (Vector3.down * 45);

        path.InvalidateDistanceCache();
        cart.m_Position = 0;

        //speed
        cart.m_Speed = cart.m_Path.PathLength / 1500;

        //OnBossReveal.Invoke(true);

    }

    void AI()
    {
        UpdatePath();
        StartCoroutine(FollowPath());
        
    }



    IEnumerator FollowPath()
    {
        while (true)
        {
            //play leaving ground effect

            yield return new WaitUntil(() => cart.m_Position >= 0.06f);
            //GroundContact.Invoke(true, true);
            yield return new WaitUntil(() => cart.m_Position >= 0.23f);
            //GroundContact.Invoke(false, true);

            // wait to reenter ground

            yield return new WaitUntil(() => cart.m_Position >= 0.60f);
            //GroundContact.Invoke(true, false);
            yield return new WaitUntil(() => cart.m_Position >= 0.90f);
            //GroundContact.Invoke(false, false);
            //OnBossReveal.Invoke(false);

            // wait a beat to come out of ground again
            yield return new WaitUntil(() => cart.m_Position >= 0.99f);
            yield return new WaitForSeconds(Random.Range(1, 2));

            //reset path
            UpdatePath();
            yield return new WaitUntil(() => cart.m_Position <= 0.05f);
        }
    }
}
