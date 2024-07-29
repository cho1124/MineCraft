using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWorldButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject new_world_obj = null;
    [SerializeField] private Transform setparent = null;

    private void Awake() // 초기화
    {
        button = button.GetComponent<Button>();
    }

    public void AddWorld() // 새로운 세계 생성
    {
        if (setparent.childCount < 3)
        {
            button.transition = Selectable.Transition.SpriteSwap;
            button.interactable = true;

            GameObject obj = Instantiate(new_world_obj);

            obj.transform.SetParent(setparent);

            Debug.Log("새로운 세계 생성");
        }
        else
        {
            button.interactable = false;
            button.transition = Selectable.Transition.ColorTint;
        }
    }

    public void DestroyWorld(GameObject obj) // 선택된 세계 삭제
    {
        // 로직 요청

        Debug.Log("선택된 세계 삭제");
    }
}