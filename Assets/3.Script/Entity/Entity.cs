using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;


public class Entity : MonoBehaviour, IDamageable {
        // �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���)
        // entity�� ���ݹ����� ������ ����
        // ������ ��ƼŬ�� ����Ʈ
        // awake���� entityeditor �� �ִ� ���� ���� �ھ����ȵǰ�����...
        //

        public string type;
        public string name;
        public int damage = 10;
        public int maxHealth = 100;
        public int health;
        private float posture;
        private float defence;
        private float weight;
        private float speed;
        public GameObject deathEffectPrefab;

        protected Animator animator;
        private Renderer[] entityRenderer;
        private Color[] originalColor;
    protected Rigidbody rb;
        protected  Vector3 originalPosition; // ���� �� ��ġ�� ������ ����
      
        public event Action OnDeath; // ���� �̺�Ʈ ����

    //  protected virtual void Awake()
    //  {
    //      LoadEntityData();
    //  }

    protected virtual void Start() {

        
        health = maxHealth;
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        entityRenderer = GetComponentsInChildren<Renderer>();
      //  LoadEntityData();
        originalColor = new Color[entityRenderer.Length];
            for (int i = 0; i < entityRenderer.Length; i++) {
                originalColor[i] = entityRenderer[i].material.color;
            }

        }

        public int Health {
            get {
                return health;
            }
            set {
                if (health > value) {
                    StartCoroutine(BlinkRed());
                }

                health = value;

                if (health <= 0) {
                Die();

                }
            }
        }

        protected virtual void Die() {
            Debug.Log($"{name}�׾�����̤�");
        OnDeath?.Invoke(); // ���� �̺�Ʈ ȣ��

    }

        private IEnumerator BlinkRed() {
            float elapsedTime = 0;
            bool isRed = false;

            while (elapsedTime < 2f) {
                for (int i = 0; i < entityRenderer.Length; i++) {
                    // ObstacleDetector ������Ʈ�� ���� ������Ʈ�� ����
                    if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
                        continue;
                    }

                    entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
                }
                isRed = !isRed;
                elapsedTime += 0.3f; // �����̴� �ӵ�
                yield return new WaitForSeconds(0.3f);
            }
            for (int i = 0; i < entityRenderer.Length; i++) {
                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
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

        public virtual void TakeDamage(int damage) {
        Debug.Log($"{name}��(��) {damage}��ŭ�� �������� �Ծ����ϴ�. ���� ü��: {health}");
        Health -= damage;
        }

    public void Attack(Entity target)
    {
        if (target != null && target is IDamageable)
        {
            StartCoroutine(PerformAttack(target));
        }
    }

    private IEnumerator PerformAttack(Entity target)
    {
        // ���� ��ġ ����
        originalPosition = transform.position;

        // Rigidbody�� x��� z�� �̵��� ����
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        // ���� �ִϸ��̼� Ʈ���� ����
        animator.SetBool("Fight", true);

        // ���� �ִϸ��̼� ���̸�ŭ ���
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        // ���� ��󿡰� ������ ������
        if (target != null && target is IDamageable)
        {
            ((IDamageable)target).TakeDamage(damage);
        }

        // Rigidbody�� �̵� ���� ����
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // x, z ��ġ�� ���� ���� ��ġ�� ����
        transform.position = new Vector3(originalPosition.x, transform.position.y, originalPosition.z);

        // ���� �ִϸ��̼� ����
        animator.SetBool("Fight", false);
    }

    public void Initialize(Entity jsonEntity) {
            this.type = jsonEntity.type;
            this.name = jsonEntity.name;
            this.health = jsonEntity.health;
            this.maxHealth = jsonEntity.health; // assuming maxHealth should be set to the loaded health
            this.damage = jsonEntity.damage;
        }
}

public interface IDamageable //������ �Դ� �������̽� 
    {
        void TakeDamage(int damage);
    }


