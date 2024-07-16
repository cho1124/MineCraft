using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���) 24.07.16 ����(������)
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
            elapsedTime += 0.3f;  //�����̴� �ӵ�
            yield return new WaitForSeconds(0.3f);
        }
        entityRenderer.material.color = originalColor;
    }

    private void OnDie()
    {
        Debug.Log($"{name} �� �׾����ϴ�. ");
        Destroy(gameObject);
    }
}
/*
 
���ʱ���� �ʿ��ϸ� �����ؼ� �����ϱ��

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
                //Ondie �޼��� �����
            }
        }
    }
    //�������� Ŀ���� �ؼ� ���� �� ��

}
 
 */