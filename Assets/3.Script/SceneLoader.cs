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
        // World �� �񵿱� �ε�
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
                    Debug.Log("�ε� �Ϸ�, �� Ȱ��ȭ...");
                    operation.allowSceneActivation = true;
                }
            }
        }

        // �� Ȱ��ȭ ��ٸ�
        while (!operation.isDone)
        {
            yield return null;
        }

        // World �ν��Ͻ��� �ʱ�ȭ�� ������ ���
        while (World.Instance == null)
        {
            Debug.Log("World �ν��Ͻ� �ʱ�ȭ ��� ��...");
            yield return null;
        }

        Debug.Log("World �ν��Ͻ� �߰�.");

        // World �� �ε� �Ϸ���� ���
        while (!World.Instance.isLoadingComplete)
        {
            Debug.Log("World �ε� �Ϸ� ��� ��...");
            yield return null;
        }

        // �ε� �Ϸ� �� �ε� �� ��Ȱ��ȭ
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync("Asynchronous");

        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        Debug.Log("World �ε� �Ϸ�.");
    }
}
