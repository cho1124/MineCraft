using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster, IDamageable 
    {
    public GameObject slimePrefab; // ���� ũ���� ������ ������
    public float deathAnimationDuration = 2f; // �״� �ִϸ��̼��� ���� �ð�
    private int currentHealth = 50; // �������� ���� ü��, �ʿ信 ���� �ʱ�ȭ

    private Vector3 deathPosition; // �������� ���� ���� ��ġ

    protected override void Start() 
    {
        base.Start();
           currentHealth = (int)Health; // �θ� Ŭ������ Health �ʱⰪ�� currentHealth�� ����
    }

    private void OnCollisionEnter(Collision collision) {
        base.OnCollisionEnter(collision); // Monster Ŭ������ OnCollisionEnter �޼��� ȣ��

        // �浹�� ��ü�� �÷��̾� �Ǵ� �������� Ȯ��
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Animals")) {
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(10); // ���� ��ü���� 10�� �������� ����
            }
        }
    }

    public void TakeDamage(float damage) {
        currentHealth -= (int)damage; // �������� ���� ü���� ���ҽ�ŵ�ϴ�.
        if (currentHealth <= 0) {

            // �������� ���� ���� ��ġ ����
            deathPosition = transform.position;

            StartCoroutine(OnDie());
        }
    }

    protected override IEnumerator OnDie() {
        yield return base.OnDie(); // �θ� Ŭ������ OnDie �ڷ�ƾ�� ����
        StartCoroutine(DieAndSplit()); // ������ �п� �ڷ�ƾ�� ����
    }

    private IEnumerator DieAndSplit() {
        Debug.Log("������ �п� ����");

        // �� ���� ���ο� ������ ����
        for (int i = 0; i < 2; i++) {
            GameObject newSlime = Instantiate(slimePrefab, deathPosition + new Vector3(i == 0 ? -0.5f : 0.5f, 0, 0), Quaternion.identity);
            newSlime.transform.localScale = transform.localScale / 2; // �п��� �������� ũ�⸦ ������ ����
            newSlime.GetComponent<Slime>().currentHealth = currentHealth / 2; // �п��� �������� ü�µ� ��������
        }

        // ���� �ð� ��� �� ���� ������ �ı�
        yield return new WaitForSeconds(0.5f);
        Debug.Log("������ �ı�");
        Destroy(gameObject);
    }
}
