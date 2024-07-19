using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    // �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���)
    // ���� �ڵ� �ϴܿ� �־ �ʿ�� �������� ���� �����մϴ�.
    // entity�� ���ݹ����� ������ ����
    // ������ ��ƼŬ�� ����Ʈ �������� �ϴ���
    // Start is called before the first frame update

    private float maxHealth=100;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;
    public GameObject deathEffectPrefab;

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

        while(elapsedTime<2f)
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
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
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