using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    // ========== Inspector private ==========

    private RectTransform rt;

    private float background_speed = 20;

    private void Awake()
    {
        rt = this.GetComponent<RectTransform>();
    }

    private void Update()
    {
        float speed = this.background_speed * Time.deltaTime;

        this.rt.Translate(-speed, 0, 0);

        if (this.rt.position.x <= -4000)
        {
            this.rt.position = Vector3.zero;
        }
    }

    public void StartGame() // 게임 시작
    {
        SceneManager.LoadScene("Inventory");

        Debug.Log("게임 시작");
    }

    public void ExitGame() // 게임 종료
    {
        Application.Quit();

        //UnityEditor.EditorApplication.isPlaying = false;

        Debug.Log("게임 종료");
    }
}