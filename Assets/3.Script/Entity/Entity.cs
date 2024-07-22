using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Entity : MonoBehaviour
{
    // �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���)
    // ���� �ڵ� �ϴܿ� �־ �ʿ�� �������� ���� �����մϴ�.
    // entity�� ���ݹ����� ������ ����
    // ������ ��ƼŬ�� ����Ʈ �������� �ϴ���
    // Start is called before the first frame update

    private float maxHealth = 100;
    private float health;
    private float posture;
    private float defence;
    private float weight;
    private float speed;
    public GameObject deathEffectPrefab;

    protected Animator animator;
    private Renderer[] entityRenderer;
    private Color[] originalColor;

    public event Action OnDeath; // ���� �̺�Ʈ ����

    protected virtual void Start()
    {
        health = maxHealth;
        animator = GetComponent<Animator>();
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
            if (health > value)
            {
                StartCoroutine(BlinkRed());
            }

            health = value;

            if (health <= 0)
            {
                Die();

            }
        }
    }

    protected virtual void Die()
    {
        Debug.Log("�׾�����̤�");
        OnDeath?.Invoke(); // ���� �̺�Ʈ ȣ��
    }

    private IEnumerator BlinkRed()
    {
        float elapsedTime = 0;
        bool isRed = false;

        while (elapsedTime < 2f)
        {
            for (int i = 0; i < entityRenderer.Length; i++)
            {
                // ObstacleDetector ������Ʈ�� ���� ������Ʈ�� ����
                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null)
                {
                    continue;
                }

                entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
            }
            isRed = !isRed;
            elapsedTime += 0.3f; // �����̴� �ӵ�
            yield return new WaitForSeconds(0.3f);
        }
        for (int i = 0; i < entityRenderer.Length; i++)
        {
            if (entityRenderer[i].GetComponent<ObstacleDetector>() != null)
            {
                continue;
            }

            entityRenderer[i].material.color = originalColor[i];
        }
    }

    public virtual IEnumerator OnDie() // virtual Ű���� �߰�
    {
        animator.SetTrigger("Die");
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);
        Debug.Log("�ִϸ��̼� ��� �Ϸ�");


        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }

    public void TakeDamage(float damage)
    {
        Health -= damage;
    }

}

public interface IDamageable {
    void TakeDamage(float damage);
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