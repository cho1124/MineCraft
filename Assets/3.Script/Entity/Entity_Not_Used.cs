using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using System;


public class Entity_Not_Used : MonoBehaviour
{
    //// �������� ������ 3�ʰ� ���������� ���ڰŸ��� �ڵ� (�ʿ�� �����������ּ���)
    //// entity�� ���ݹ����� ������ ����
    //// ������ ��ƼŬ�� ����Ʈ
    //// awake���� entityeditor �� �ִ� ���� ���� =>������ ����� ü�� ������ ������
    //// ������ ���� ���ݰ� ������ ó�� �ϴ� �κ� �ش� Ŭ������ ����
    ////TakeDamage �޼��尡 ����� �� ü���� 0 ���ϰ� �Ǹ� Die �޼��尡 ȣ��Ǹ�, �̴� OnDeath �̺�Ʈ�� Ʈ�����մϴ�.
    //
    //public string type;
    //public string name;
    //public int damage = 10;
    //public int maxHealth = 100;
    //private int health;
   //// private float posture;
   //// private float defence;
   //// private float weight;
   //// private float speed;
    //public GameObject deathEffectPrefab;
    //
    //protected Animator animator;
    //private Renderer[] entityRenderer;
    //private Color[] originalColor;
    //protected Rigidbody rb;
    //protected Collider col;
    //protected Vector3 originalPosition; // ���� �� ��ġ�� ������ ����
    //
    //public event Action OnDeath; // ���� �̺�Ʈ ����
    //
    ////  protected virtual void Awake()
    ////  {
    ////      LoadEntityData();
    ////  }
    //
    //protected virtual void Start()
    //{
    //
    //    health = maxHealth;
    //    animator = GetComponent<Animator>();
    //    col = GetComponent<Collider>(); // �ݶ��̴� �ʱ�ȭ
    //    rb = GetComponent<Rigidbody>();
    //    entityRenderer = GetComponentsInChildren<Renderer>();
    //    //  LoadEntityData();
    //    originalColor = new Color[entityRenderer.Length]; //�� renderer�� ���� ������ �����ؼ� �����϶� ���� ������ ���ƿ� �� �ְ�.
    //    for (int i = 0; i < entityRenderer.Length; i++)
    //    {
    //        originalColor[i] = entityRenderer[i].material.color;
    //    }
    //}
    //
    //    public int Health {
    //    get {
    //        return health;
    //    }
    //    set {
    //        if (health > value) {
    //            StartCoroutine(BlinkRed());
    //            Debug.Log($"{name}�� �������� �Ծ� ���� ü��: {value}");
    //        }
    //        health = value;
    //
    //        if (health <= 0) {
    //            Die();
    //
    //        }
    //    }
    //}
    //
    //    protected virtual void Die() {
    //        Debug.Log($"{name}�׾�����̤�");
    //    OnDeath?.Invoke(); // ���� �̺�Ʈ ȣ��
    //      //  StartCoroutine(OnDie()); // �ٷ� OnDie �ڷ�ƾ ȣ��
    //}
    //
   //// private IEnumerator DelayedDie()
   //// {
   ////     // OnDeath �̺�Ʈ�� �Ϸ�� ������ ���
   ////     yield return new WaitForEndOfFrame();
   //// }
    //
    //protected IEnumerator BlinkRed() {
    //        float elapsedTime = 0;
    //        bool isRed = false;
    //
    //        while (elapsedTime < 2f) {
    //            for (int i = 0; i < entityRenderer.Length; i++) {
    //                // ObstacleDetector ������Ʈ�� ���� ������Ʈ�� ����
    //                if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
    //                    continue;
    //                }
    //
    //                entityRenderer[i].material.color = isRed ? originalColor[i] : Color.red;
    //            }
    //            isRed = !isRed;
    //            elapsedTime += 0.3f; // �����̴� �ӵ�
    //            yield return new WaitForSeconds(0.3f);
    //        }
    //        for (int i = 0; i < entityRenderer.Length; i++) {//�����̰� ���� ���� ����� ���ư��� �κ�
    //            if (entityRenderer[i].GetComponent<ObstacleDetector>() != null) {
    //                continue;
    //            }
    //
    //            entityRenderer[i].material.color = originalColor[i];
    //        }
    //    }
    //
    //    public virtual IEnumerator OnDie() // virtual Ű���� �߰�
    //    {
    //        animator.SetTrigger("Die");
    //        yield return new WaitForSeconds(2f);
    //        Debug.Log("�ִϸ��̼� ��� �Ϸ�");
    //  
    //        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //        Destroy(gameObject);
    //  
    //    }
    //
    //    public virtual void TakeDamage(int damage) {
    //    Debug.Log($"��ƼƼ!! {name}��(��) {damage}�� �������� ����");
    //    Health -= damage;
    //
    //}
    //
    //public void Attack(Entity target)
    //{
    //    if (target != null && target is IDamageable)
    //    {
    //        StartCoroutine(PerformAttack(target));
    //    }
    //}
    //
    //private IEnumerator PerformAttack(Entity target) //���� �ִϸ��̼� �����ϴ� �κи� ���ܵΰ� ��ǥ �����ϴ°� ��� ���� 
    //{
    //    // ���� �ִϸ��̼� Ʈ���� ����
    //    animator.SetBool("Fight", true);
    //
    //    // ���� �ִϸ��̼� ���̸�ŭ ���
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //    yield return new WaitForSeconds(stateInfo.length);
    //
    //    // ���� ��󿡰� ������ ������
    //    if (target != null && target is IDamageable)
    //    {
    //        ((IDamageable)target).TakeDamage(damage);
    //    }
    //    // ���� �ִϸ��̼� ����
    //    animator.SetBool("Fight", false);
    //}

  //  public void Initialize(Entity jsonEntity)
  //  {
  //      this.health = jsonEntity.health;
  //      this.maxHealth = jsonEntity.health;
  //      this.damage = jsonEntity.damage;
  //  }
}

public interface IDamageable //������ �Դ� �������̽� 
    {
        void TakeDamage(int damage);
    }


