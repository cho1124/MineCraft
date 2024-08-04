using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    static string nextScene = null;

    [SerializeField] private Image progressBar = null;

    private void Start()
    {
        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(LoadSceneProcess());
        }
    }

    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("Asynchronous");
    }

    IEnumerator LoadSceneProcess()
    {
        // World 씬 비동기 로드
        AsyncOperation operation = SceneManager.LoadSceneAsync("World", LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        float timer = 0f;

        while (!operation.isDone)
        {
            yield return null;

            if (operation.progress < 0.9f)
            {
                progressBar.fillAmount = operation.progress;
            }
            else
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1f, timer);

                if (progressBar.fillAmount >= 1f)
                {
                    Debug.Log("로딩 완료, 씬 활성화...");
                    operation.allowSceneActivation = true;
                }
            }
        }

        // 씬 활성화 기다림
        while (!operation.isDone)
        {
            yield return null;
        }

        // World 인스턴스가 초기화될 때까지 대기
        while (World.Instance == null)
        {
            Debug.Log("World 인스턴스 초기화 대기 중...");
            yield return null;
        }

        Debug.Log("World 인스턴스 발견.");

        // World 씬 로딩 완료까지 대기
        while (!World.Instance.isLoadingComplete)
        {
            Debug.Log("World 로딩 완료 대기 중...");
            yield return null;
        }

        // 로딩 완료 후 로딩 씬 비활성화
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("Asynchronous");

        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("World 로딩 완료.");
    }
}
