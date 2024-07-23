using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enderman : Monster, IDamageable
{

    private void HandleDeath()
    {
        StartCoroutine(OnDie());
    }

    public override IEnumerator OnDie()
    {
        // 사망 애니메이션을 트리거합니다.
        animator.SetTrigger("Die");

        // 사망 애니메이션의 길이를 얻어 대기합니다.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        // 사망 효과를 생성합니다.
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // 엔더맨 오브젝트를 파괴합니다.
        Destroy(gameObject);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (health <= 0)
        {
            Debug.Log($"{name}이(가) 죽음.");
            HandleDeath();
        }
    }
}
