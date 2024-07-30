using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creeper
{

    //public Collider coll;
    //private Entity entity;
    //public GameObject explosionEffectPrefab;
    //public float explosionScaleFactor = 5;
    //public float explosionDuration = 5f;
    //public float explosionForce = 100f; // ���߷��� �󸶳� �������� ����
    //public bool showExplosionRange = true; // ���� ������ �������� ���θ� �����ϴ� ����
    //
    //protected override void Start()
    //{
    //    entity = GetComponent<Entity>();
    //    if (entity != null)
    //    {
    //        entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
    //    }
    //
    //    base.Start();
    //
    //}
    //
    //public override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //    StartCoroutine(BlinkRed()); // �������� �Ծ��� �� BlinkRed ȣ��
    //}
    //
    //private void HandleDeath()
    //{
    //    /*
    //    ũ������ �ݶ��̴��� 2��� õõ�� Ŀ���鼭 
    //    �ݶ��̴��� ������ ������Ʈ�� �����̳� �÷��̾� �±׸� ������ ������ ü���� 100����
    //    �̿��� �ݶ��̴��� ��� ������Ʈ���� ��� �ı����Ѿ��� 
    //    ��ƼŬ�� �����ϴµ��� ����Ʈ�� ���;��� (��ƼŬ�� ť����. material�� creeper �� ���� material�� 
    //    */
    //    StartCoroutine(ExplosionSequence());
    //
    //}
    //
    //private IEnumerator ExplosionSequence()
    //{
    //    Debug.Log("�����غ���");
    //    // �ݶ��̴� ũ�⸦ ���������� ����
    //    Vector3 originalScale = coll.transform.localScale;
    //    Vector3 targetScale = originalScale * explosionScaleFactor;
    //    float elapsedTime = 0f; // ��� �ð��� �ʱ�ȭ�մϴ�.
    //
    //    while (elapsedTime < explosionDuration)
    //    {
    //        coll.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / explosionDuration);
    //        // ��� �ð��� ����Ͽ� �ݶ��̴��� ũ�⸦ ���� ũ�⿡�� ��ǥ ũ��� ���������� ������ŵ�ϴ�.
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //
    //    coll.transform.localScale = targetScale;
    //    // �ݶ��̴��� ũ�⸦ ��ǥ ũ��� �����մϴ�.
    //    Debug.Log("����!");
    //
    //    // �ݶ��̴��� ��� ��� ������Ʈ ó��
    //    Collider[] hitColliders = Physics.OverlapSphere(transform.position, coll.bounds.extents.magnitude);
    //    foreach (var hitCollider in hitColliders)
    //    {
    //        if (hitCollider.gameObject != gameObject) // �ڱ� �ڽ��� ����
    //        {
    //            // Player �Ǵ� Animals �±׸� ���� ��ü�� ü�� ����
    //            if (hitCollider.gameObject.CompareTag("Player") || hitCollider.gameObject.CompareTag("Animals"))
    //            {
    //                IDamageable damageable = hitCollider.GetComponent<IDamageable>();
    //                if (damageable != null)
    //                {
    //                    damageable.TakeDamage(100); // ü�� 100 ����
    //                    Debug.Log($"ü�� ����: {hitCollider.gameObject.name}");
    //                }
    //            }
    //            else
    //            {
    //                // Rigidbody�� �ִ� ��� ���߷��� ����
    //                Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
    //                if (rb != null)
    //                {
    //                    Vector3 explosionDirection = hitCollider.transform.position - transform.position;
    //                    rb.AddExplosionForce(explosionForce, transform.position, coll.bounds.extents.magnitude);
    //                    // Rigidbody�� �ִ� ���, ���߷��� ���մϴ�. AddExplosionForce�� ������ ������ ������Ʈ�� �о���ϴ�.
    //                }
    //
    //                // Player �Ǵ� Animals �±װ� �ƴ� ��� ������Ʈ �ı�
    //                AudioManager.instance.PlayRandomSFX("Creeper", "Die");
    //                AudioManager.instance.PlayRandomSFX("Creeper", "Explore");
    //                Destroy(hitCollider.gameObject);
    //                Debug.Log($"������Ʈ �ı�: {hitCollider.gameObject.name}");
    //            }
    //        }
    //    }
    //
    //    // ���� ��ƼŬ ����Ʈ ����
    //
    //    GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
    //    Destroy(explosionEffect, 2f); // 2�� �� ��ƼŬ ����Ʈ ����
    //
    //    // OnDie �ڷ�ƾ ȣ��
    //    StartCoroutine(entity.OnDie());
    //}
    //protected override void Die()
    //{
    //    HandleDeath(); // �⺻ Die ���� ��� ���� ������ ȣ��
    //}
}
