using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    static string next_scene = null;

    [SerializeField] private Image bar = null;

    public static void LoadScene(string scene_name)
    {
        next_scene = scene_name;

        SceneManager.LoadScene("Asynchronous");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }

    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(next_scene);

        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                bar.fillAmount = op.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;

                bar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (bar.fillAmount >= 1f)
                {
                    op.allowSceneActivation = true;

                    yield break;
                }
            }
        }
    }
}