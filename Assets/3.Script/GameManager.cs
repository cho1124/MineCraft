using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager instance = null;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //처음에 시작할 때 다른 매니저 클래스들을 소환
    }

    public void StartGame()
    {
        
    }


    //생각해 봐야할 점, 모든 매니저 클래스에 굳이 싱글턴을 써야할까?
    //사운드, 씬, 게임 매니저는 싱글턴을 쓰는게 맞겠지만
    //데이터 매니저와 같은 경우는 게임이 시작할 때만 부르고
    //데이터 매니저를 통해서 데이터를 부르면 그 다음에 세이브된 파일을 로드하는 세이브 로드 매니저를 호출하는 방식
    //그 다음에 전반적인 엔티티들을 관리하는 엔티티 매니저를 호출하는 것
    //

}
