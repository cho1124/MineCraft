using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creeper : Monster, IDamageable
{

    public Collider coll;
    private Entity entity;
    public GameObject explosionEffectPrefab;
    private int currentHealth; // �������� ���� ü��, �ʿ信 ���� �ʱ�ȭ
    public float explosionScaleFactor = 10;
    public float explosionDuration = 5f;
    public float explosionForce = 100000f; // ���߷��� �󸶳� �������� ����
    public bool showExplosionRange = true; // ���� ������ �������� ���θ� �����ϴ� ����

    protected override void Start()
    {
        entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }

        base.Start();

    }

  //  private void OnCollisionEnter(Collision collision) //�浹�� �ٸ� ������Ʈ���� �������� �� 
  //  {
  //      base.OnCollisionEnter(collision); // Monster Ŭ������ OnCollisionEnter �޼��� ȣ��
  //
  //      // �浹�� ��ü�� �÷��̾� �Ǵ� �������� Ȯ��
  //      if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Animals"))
  //      {
  //          IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
  //          if (damageable != null)
  //          {
  //              damageable.TakeDamage(10); // ���� ��ü���� 10�� �������� ����
  //          }
  //      }
  //  }

    private void HandleDeath()
    {
        /*
        ũ������ �ݶ��̴��� 3��� õõ�� Ŀ���鼭 
        �ݶ��̴��� ������ ������Ʈ�� �����̳� �÷��̾� �±׸� ������ ������ ü���� 100����
        �̿��� �ݶ��̴��� ��� ������Ʈ���� ��� �ı����Ѿ��� 
        ��ƼŬ�� �����ϴµ��� ����Ʈ�� ���;��� (��ƼŬ�� ť����. material�� creeper �� ���� material�� 
        */
        StartCoroutine(ExplosionSequence());

    }

    private IEnumerator ExplosionSequence()
    {
        Debug.Log("�����غ���");
        // �ݶ��̴� ũ�⸦ ���������� ����
        Vector3 originalScale = coll.transform.localScale;
        Vector3 targetScale = originalScale * explosionScaleFactor;
        float elapsedTime = 0f; // ��� �ð��� �ʱ�ȭ�մϴ�.

        while (elapsedTime < explosionDuration)
        {
            coll.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / explosionDuration);
            // ��� �ð��� ����Ͽ� �ݶ��̴��� ũ�⸦ ���� ũ�⿡�� ��ǥ ũ��� ���������� ������ŵ�ϴ�.
            elapsedTime += Time.deltaTime;
            Debug.Log($"�ݶ��̴� ũ�� ���� ��: {coll.transform.localScale}");
            yield return null;
        }

        coll.transform.localScale = targetScale;
        // �ݶ��̴��� ũ�⸦ ��ǥ ũ��� �����մϴ�.
        Debug.Log("����!");

        // �ݶ��̴��� ��� ��� ������Ʈ ó��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coll.bounds.extents.magnitude);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject != gameObject) // �ڱ� �ڽ��� ����
            {
                // Player �Ǵ� Animals �±׸� ���� ��ü�� ü�� ����
                if (hitCollider.gameObject.CompareTag("Player") || hitCollider.gameObject.CompareTag("Animals"))
                {
                    IDamageable damageable = hitCollider.GetComponent<IDamageable>();
                    if (damageable != null)
                    {
                        damageable.TakeDamage(100); // ü�� 100 ����
                        Debug.Log($"ü�� ����: {hitCollider.gameObject.name}");
                    }
                }
                else
                {
                    // Rigidbody�� �ִ� ��� ���߷��� ����
                    Rigidbody rb = hitCollider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        Vector3 explosionDirection = hitCollider.transform.position - transform.position;
                        rb.AddExplosionForce(explosionForce, transform.position, coll.bounds.extents.magnitude);
                        // Rigidbody�� �ִ� ���, ���߷��� ���մϴ�. AddExplosionForce�� ������ ������ ������Ʈ�� �о���ϴ�.
                    }

                    // Player �Ǵ� Animals �±װ� �ƴ� ��� ������Ʈ �ı�
                    Destroy(hitCollider.gameObject);
                    Debug.Log($"������Ʈ �ı�: {hitCollider.gameObject.name}");
                }
            }
        }

        // ���� ��ƼŬ ����Ʈ ����

        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Renderer explosionRenderer = explosionEffect.GetComponent<Renderer>();
        Renderer creeperRenderer = GetComponent<Renderer>();
        if (creeperRenderer != null && explosionRenderer != null)
        {
            explosionRenderer.material = creeperRenderer.material;
            // ũ������ material�� ���� ��ƼŬ ����Ʈ�� �����մϴ�.
            Debug.Log("��ƼŬ ����Ʈ ����");
        }

        Destroy(explosionEffect, 2f); // 2�� �� ��ƼŬ ����Ʈ ����

        // OnDie �ڷ�ƾ ȣ��
        StartCoroutine(entity.OnDie());
    }

}
