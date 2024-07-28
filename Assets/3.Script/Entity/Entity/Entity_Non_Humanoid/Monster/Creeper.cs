using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Creeper : Entity_Non_Humanoid
{

    public Collider coll;
    private Entity entity;
    public GameObject explosionEffectPrefab;
    public float explosionScaleFactor = 10;
    public float explosionDuration = 5f;
    public bool showExplosionRange = true; // ���� ������ �������� ���θ� �����ϴ� ����

    protected void Start()
    {
        entity = GetComponent<Entity>();
        if (entity != null)
        {
            //entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }


    }

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
            Debug.Log("�ݶ��̴� ũ�� ���� ��");
            yield return null;
        }

        coll.transform.localScale = targetScale;
        // �ݶ��̴��� ũ�⸦ ��ǥ ũ��� �����մϴ�.
        Debug.Log("����!");

        // �ݶ��̴��� ��� ��� ������Ʈ ó��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coll.bounds.extents.magnitude);
        foreach (var hitCollider in hitColliders)
        {
            // Player �Ǵ� Animals �±׸� ���� ��ü�� ü�� ����
            if (hitCollider.gameObject.CompareTag("Entity"))
            {
                hitCollider.gameObject.GetComponent<Entity>().On_Hit(100f, gameObject.GetComponent<Collider>());
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
    }

}
