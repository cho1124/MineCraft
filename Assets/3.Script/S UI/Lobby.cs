using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Lobby : MonoBehaviour
{
    private RectTransform rt;

    [SerializeField] private float speed = 0;

    private void Awake()
    {
        rt = this.GetComponent<RectTransform>();
    }

    private void Update()
    {
        float speed = this.speed * Time.deltaTime;

        this.rt.Translate(-speed, 0, 0);

        if (this.rt.position.x <= -4000)
        {
            this.rt.position = Vector3.zero;

            Debug.Log("dsadasd");
        }
    }

    public void StartGame() // 게임 시작
    {
        SceneManager.LoadScene("JDK");

        Debug.Log("게임 시작");
    }

    public void ExitGame() // 게임 종료
    {
        Application.Quit();

        UnityEditor.EditorApplication.isPlaying = false;

        Debug.Log("게임 종료");
    }
}