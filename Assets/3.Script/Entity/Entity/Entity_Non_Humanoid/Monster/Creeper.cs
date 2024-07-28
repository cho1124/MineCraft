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
    public bool showExplosionRange = true; // 폭발 범위를 보여줄지 여부를 설정하는 변수

    protected void Start()
    {
        entity = GetComponent<Entity>();
        if (entity != null)
        {
            //entity.OnDeath += HandleDeath; // 죽음 이벤트 구독
        }


    }

    private void HandleDeath()
    {
        /*
        크리퍼의 콜라이더가 3배로 천천히 커지면서 
        콜라이더에 감지된 오브젝트가 동물이나 플레이어 태그를 가지고 있으면 체력을 100감소
        이외의 콜라이더에 닿는 오브젝트들을 모두 파괴시켜야함 
        파티클로 폭발하는듯한 이펙트가 나와야함 (파티클은 큐브모양. material은 creeper 와 같은 material로 
        */
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        Debug.Log("폭발준비중");
        // 콜라이더 크기를 점진적으로 증가
        Vector3 originalScale = coll.transform.localScale;
        Vector3 targetScale = originalScale * explosionScaleFactor;
        float elapsedTime = 0f; // 경과 시간을 초기화합니다.

        while (elapsedTime < explosionDuration)
        {
            coll.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / explosionDuration);
            // 경과 시간에 비례하여 콜라이더의 크기를 원래 크기에서 목표 크기로 점진적으로 증가시킵니다.
            elapsedTime += Time.deltaTime;
            Debug.Log("콜라이더 크기 증가 중");
            yield return null;
        }

        coll.transform.localScale = targetScale;
        // 콜라이더의 크기를 목표 크기로 설정합니다.
        Debug.Log("폭발!");

        // 콜라이더에 닿는 모든 오브젝트 처리
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, coll.bounds.extents.magnitude);
        foreach (var hitCollider in hitColliders)
        {
            // Player 또는 Animals 태그를 가진 물체의 체력 감소
            if (hitCollider.gameObject.CompareTag("Entity"))
            {
                hitCollider.gameObject.GetComponent<Entity>().On_Hit(100f, gameObject.GetComponent<Collider>());
            }
        }

        // 폭발 파티클 이펙트 생성

        GameObject explosionEffect = Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        Renderer explosionRenderer = explosionEffect.GetComponent<Renderer>();
        Renderer creeperRenderer = GetComponent<Renderer>();
        if (creeperRenderer != null && explosionRenderer != null)
        {
            explosionRenderer.material = creeperRenderer.material;
            // 크리퍼의 material을 폭발 파티클 이펙트에 적용합니다.
            Debug.Log("파티클 이펙트 생성");
        }

        Destroy(explosionEffect, 2f); // 2초 후 파티클 이펙트 제거
    }

}
