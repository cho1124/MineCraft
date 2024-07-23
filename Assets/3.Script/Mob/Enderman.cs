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
        // ��� �ִϸ��̼��� Ʈ�����մϴ�.
        animator.SetTrigger("Die");

        // ��� �ִϸ��̼��� ���̸� ��� ����մϴ�.
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        yield return new WaitForSeconds(stateInfo.length);

        // ��� ȿ���� �����մϴ�.
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);

        // ������ ������Ʈ�� �ı��մϴ�.
        Destroy(gameObject);
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if (health <= 0)
        {
            Debug.Log($"{name}��(��) ����.");
            HandleDeath();
        }
    }
}
