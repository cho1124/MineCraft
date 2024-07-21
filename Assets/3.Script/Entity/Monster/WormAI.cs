using UnityEngine;
using System.Collections;
using Cinemachine;

public class WormAI : MonoBehaviour
{

    [Header("대왕벌레 경로")]
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
        //StartCoroutine(FollowPath());
        player = FindObjectOfType<Player_Control>();

        AI();
    }

    void AI()
    {
        UpdatePath();
        StartCoroutine(FollowPath());

    }




    //1번 패턴 : 기본 패턴
    //    //2번 패턴 : 오직 y축으로만 솟구쳤다 떨어지는 패턴

    void UpdatePath()
    {
        Vector3 playerPosition = player.transform.position + (player.GetComponent<Rigidbody>().velocity * 3);
        playerPosition.y = Mathf.Max(10, playerPosition.y);
        Vector3 randomRange = Random.insideUnitSphere * 100; //공통 부분
        int randNum = Random.Range(0, 3);
        randomRange.y = 0;


        switch(randNum)
        {
            case 0:
                Debug.Log("P1");
                Pattern1(playerPosition, randomRange);
                break;
            case 1:
                Debug.Log("P2");
                Pattern2(playerPosition, randomRange);
                break;
            case 2:
                //
                break;
            default:
                Debug.Log("error");
                break;
        }
        //Pattern1(playerPosition, randomRange);




        //pattern 1

        


        //Pattern2(playerPosition, randomRange);
        //startPosition = playerPosition + randomRange;
        //
        //
        //endPosition = playerPosition - randomRange;
        //
        //if (Physics.Raycast(startPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        //{
        //    startPosition = hitInfo.point;
        //
        //}
        //
        //if (Physics.Raycast(endPosition, Vector3.down, out hitInfo, 1000, terrainLayer.value))
        //{
        //    endPosition = hitInfo.point;
        //    //GroundDetection.Invoke(false, hitInfo.transform.CompareTag("Terrain") ? 0 : 1);
        //}
        //
        //path.m_Waypoints[0].position = startPosition + (Vector3.down * 15);
        //path.m_Waypoints[1].position = playerPosition + (Vector3.up * 10);
        //path.m_Waypoints[2].position = endPosition + (Vector3.down * 45);
        //
        //path.InvalidateDistanceCache();
        //cart.m_Position = 0;
        //
        ////speed
        //cart.m_Speed = cart.m_Path.PathLength / 1500;

        //OnBossReveal.Invoke(true);

    }

    private void Pattern1(Vector3 playerPosition, Vector3 randomRange)
    {
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

    }

    private void Pattern2(Vector3 playerPosition, Vector3 randomRange)
    {
        startPosition = playerPosition + randomRange;


        endPosition = startPosition;

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
        path.m_Waypoints[1].position = startPosition + (Vector3.up * 50);
        path.m_Waypoints[2].position = endPosition + (Vector3.down * 35);

        path.InvalidateDistanceCache();
        cart.m_Position = 0;

        //speed
        cart.m_Speed = cart.m_Path.PathLength / 1500;
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
