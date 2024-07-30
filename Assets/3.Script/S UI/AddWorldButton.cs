using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWorldButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject new_world_obj = null;
    [SerializeField] private Transform setparent = null;
    [SerializeField] private GameObject UI_obj = null;

    public void AddWorld()
    {
        if (setparent.childCount == 1)
        {
            button.transition = Selectable.Transition.SpriteSwap;

            button.interactable = true;

            UI_obj.SetActive(true);
        }
        else
        {
            GameObject new_obj = Instantiate(new_world_obj);

            new_obj.transform.SetParent(setparent);

            new_obj.transform.localPosition = Vector3.zero;

            Debug.Log("새로운 세계 생성");

            button.interactable = false;

            button.transition = Selectable.Transition.ColorTint;
        }
    }

    public void DestroyWorld()
    {
        Destroy(setparent.transform.parent.gameObject);

        Debug.Log("선택된 세계 삭제");
    }
}

/* using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddWorldButton : MonoBehaviour
{
    [SerializeField] private GameObject new_world_obj = null;
    [SerializeField] private Transform setparent = null;
    [SerializeField] private GameObject obj = null;

    public void AddWorld() // 새로운 세계 생성
    {
        if (setparent.childCount == 1)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        else
        {
            if (new_world_obj != null)
            {
                GameObject new_obj = Instantiate(new_world_obj);

                new_obj.transform.SetParent(setparent);

                new_obj.transform.localPosition = Vector3.zero;

                Debug.Log("새로운 세계 생성");
            }
        }
    }

    public void DestroyWorld() // 기존 세계 삭제
    {
        if (setparent.childCount == 1)
        {
            Destroy(setparent.GetChild(0).gameObject);

            Debug.Log("세계 삭제");
        }
    }
} */