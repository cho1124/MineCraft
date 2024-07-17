using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���) 24.07.16 ����(������)
    // ���� �ڵ� �ϴܿ� �־ �ʿ�� �������� ���� �����մϴ�.
    // entity�� ���ݹ����� ������ ���ϰ� �Ϸ��� �� 
    // Start is called before the first frame update
    private float maxHealth=100;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;

    private Renderer[] entityRenderer;
    private Color[] originalColor;

    protected virtual void Start()
    {
        health = maxHealth;
        entityRenderer = GetComponentsInChildren<Renderer>();
        originalColor = new Color[entityRenderer.Length];
        for (int i = 0; i < entityRenderer.Length; i++)
        {
            originalColor[i] = entityRenderer[i].material.color;
        }
        if (entityRenderer.Length==0)
        {
            Debug.Log("������ ������Ʈ�� ã�� �� �����ϴ�.");
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
            for (int i = 0; i < entityRenderer.Length; i++)
            {
                entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
            }
                isRed = !isRed;
            elapsedTime += 0.3f;  //�����̴� �ӵ�
            yield return new WaitForSeconds(0.3f);
        }
        for (int i = 0; i < entityRenderer.Length; i++)
        {
            entityRenderer[i].material.color = originalColor[i];
        }
        }

    protected virtual void OnDie()
    {
        Debug.Log($"{name} �� �׾����ϴ�. ");
        Destroy(gameObject);
    }

    public void TakeDamage(float damage) {
        Health -= damage;
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