using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity_Data;

public class Enderman : Entity_Non_Humanoid
{
    /*
    ���� ������ ü���� 0���Ϸ� �پ�� �ڵ鵥�� �޼��� �̺�Ʈ��
    ȣ����� �ʰ�����, ü�� -���� ��� ��
    
    => �̺�Ʈ ���� Entity Ŭ������ TakeDamage �޼��带 �������̵��Ͽ� ü���� 0 ������ �� ���� Die �޼��带 ȣ���ϵ���
    => �׷��� ������ ���̴� ����...
    */

    private Entity entity;

    protected void Start()
    {

        entity = GetComponent<Entity>();
        if (entity != null)
        {
            //entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }

    }

    private void Update()
    {

    }


    private void HandleDeath()
    {
        Debug.Log("HandleDeath ȣ���");
        //StartCoroutine(OnDie());
    }

    //public override IEnumerator OnDie()
    //{
    //    Debug.Log($"{name}�������.");
    //    // ��� �ִϸ��̼��� Ʈ�����մϴ�.
    //    animator.SetTrigger("Die");
    //
    //    // ��� �ִϸ��̼��� ���̸� ��� ����մϴ�.
    //    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //    yield return new WaitForSeconds(stateInfo.length);
    //
    //    // ��� ȿ���� �����մϴ�.
    //    Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
    //
    //    // ������ ������Ʈ�� �ı��մϴ�.
    //    Destroy(gameObject);
    //}

    //public override void TakeDamage(int damage)
    //{
    //    base.TakeDamage(damage);
    //    if (Health <= 0 ) {
    //        HandleDeath();
    //    }
    //}
}
