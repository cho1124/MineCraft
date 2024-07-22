using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


    public class Entity : MonoBehaviour, IDamageable {
        // 데미지를 입으면 3초간 빨간색으로 깜박거리는 코드 (필요시 삭제수정해주세요)
        // entity가 공격받을때 빨갛게 변함
        // 죽을때 파티클로 이펙트
        // Start is called before the first frame update

        public string entityType;
        public float damage = 10;
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

        public event Action OnDeath; // 죽음 이벤트 선언

        protected virtual void Start() {

            health = maxHealth;
            animator = GetComponent<Animator>();
            entityRenderer = GetComponentsInChildren<Renderer>();
            originalColor = new Color[entityRenderer.Length];
            for (int i = 0; i < entityRenderer.Length; i++) {
                originalColor[i] = entityRenderer[i].material.color;
            }

        }

        public float Health {
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
            Debug.Log("죽어버림ㅜㅜ");
            OnDeath?.Invoke(); // 죽음 이벤트 호출
        }

        private IEnumerator BlinkRed() {
            float elapsedTime = 0;
            bool isRed = false;

            while (elapsedTime < 2f) {
                for (int i = 0; i < entityRenderer.Length; i++) {
                    // ObstacleDetector 컴포넌트를 가진 오브젝트는 제외
                    if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
                        continue;
                    }

                    entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
                }
                isRed = !isRed;
                elapsedTime += 0.3f; // 깜박이는 속도
                yield return new WaitForSeconds(0.3f);
            }
            for (int i = 0; i < entityRenderer.Length; i++) {
                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
                    continue;
                }

                entityRenderer[i].material.color = originalColor[i];
            }
        }

        public virtual IEnumerator OnDie() // virtual 키워드 추가
        {
            animator.SetTrigger("Die");
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            yield return new WaitForSeconds(stateInfo.length);
            Debug.Log("애니메이션 대기 완료");


            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);

        }

        public void TakeDamage(float damage) {
            Health -= damage;
        }
        public void Attack(Entity target) {
            if (target != null && target is IDamageable) {
                ((IDamageable)target).TakeDamage(damage);
            }
        }

        public void Initialize(Entity jsonEntity) {
            this.entityType = jsonEntity.type;
            this.name = jsonEntity.name;
            this.health = jsonEntity.health;
            this.maxHealth = jsonEntity.health; // assuming maxHealth should be set to the loaded health
            this.damage = jsonEntity.damage;
        }

    }

    public interface IDamageable {
        void TakeDamage(float damage);
    }


