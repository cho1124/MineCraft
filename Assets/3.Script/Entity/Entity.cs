using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // 데미지를 입으면 3초간 빨간색으로 깜박거리는 코드 (필요시 삭제수정해주세요) 24.07.16 수정(한유진)
    // Start is called before the first frame update
    private float maxHealth=100;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;

    private Renderer entityRenderer;
    private Color originalColor;

    void Start()
    {
        health = maxHealth;
        entityRenderer = GetComponent<Renderer>();
        if (entityRenderer != null)
        {
            originalColor = entityRenderer.material.color;
        }
    }

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            if(health > value)
            {
                StartCoroutine(BlinkRed());
            }

            health = value;

            if(health <= 0)
            {
                OnDie();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        float elapsedTime = 0;
        bool isRed = false;

        while(elapsedTime<3f)
        {
            entityRenderer.material.color = isRed ? originalColor : Color.red;
            isRed = !isRed;
            elapsedTime += 0.3f;  //깜박이는 속도
            yield return new WaitForSeconds(0.3f);
        }
        entityRenderer.material.color = originalColor;
    }

    private void OnDie()
    {
        Debug.Log($"{name} 이 죽었습니다. ");
        Destroy(gameObject);
    }
}
/*
 
★초기상태 필요하면 복붙해서 원복하기★

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    private float maxHealth;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;

    public float Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;

            if(health <= 0)
            {
                //Ondie 메서드 만들기
            }
        }
    }
    //나머지는 커스텀 해서 구현 할 것

}
 
 */