using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveScene : MonoBehaviour
{
    public void Title() // 타이틀
    {
        SceneManager.LoadScene("Title");
        Debug.Log("타이틀 이동");
    }

    public void Game() // 게임
    {
        SceneManager.LoadScene("Main");
        Debug.Log("게임 이동");
    }

    public void ExitGame() // 게임 종료
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Debug.Log("게임 종료");
    }
}
