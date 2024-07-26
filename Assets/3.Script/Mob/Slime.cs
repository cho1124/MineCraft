using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Monster, IDamageable 
    {

    public GameObject slimePrefab; // ���� ũ���� ������ ������
    public float deathAnimationDuration = 2f; // �״� �ִϸ��̼��� ���� �ð�
    private int currentHealth; // �������� ���� ü��, �ʿ信 ���� �ʱ�ȭ
    public RuntimeAnimatorController slimeAnimatorController; // ���� ������ �ִϸ����� ��Ʈ�ѷ�
    public RuntimeAnimatorController splitSlimeAnimatorController; // �п��� ������ �ִϸ����� ��Ʈ�ѷ�

    private Vector3 deathPosition; // �������� ���� ���� ��ġ
    private Entity entity;

    protected override void Start() 
    {
        entity = GetComponent<Entity>();
        if (entity != null)
        {
            entity.OnDeath += HandleDeath; // ���� �̺�Ʈ ����
        }

        base.Start();
    }

    // private void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Q))
    //     {
    //         DieAndSplit();
    //     }
    // }

    private void HandleDeath() //�������� ���� ��ġ�� �������� ���ο� �������� �� ���� �����մϴ�.
    {

        // �������� ũ�Ⱑ 0.5 ������ ��� �� �̻� �п����� ����
        if (transform.localScale.x <= 0.5f)
        {
            StartCoroutine(OnDie());
            return;
        }


        deathPosition = transform.position; // ���� ��ġ ����

            Vector3 spawnPosition1 = deathPosition + new Vector3(-0.5f, 0, 0);
        Vector3 spawnPosition2 = deathPosition + new Vector3(0.5f, 0, 0);

        GameObject newSlime1 = Instantiate(slimePrefab, spawnPosition1, Quaternion.identity);
        GameObject newSlime2 = Instantiate(slimePrefab, spawnPosition2, Quaternion.identity);

            // ������ �������� �Ӽ��� �ʱ�ȭ
            InitializeNewSlime(newSlime1);
            InitializeNewSlime(newSlime2);

        // OnDie �ڷ�ƾ ȣ��
        StartCoroutine(OnDie());
    }

    private void InitializeNewSlime(GameObject newSlime) //������ �ִϸ��̼� Ŭ������ scale �����ϰ� �־ �� �۾����°ſ��� ���� �ִϸ��̼���Ʈ�ѷ� ���� 
                                                         // ������ �������� ��Ʈ�ѷ��� scale �۾��� ��Ʈ�ѷ�(slime_s)�� �ٲ�
    {
        // �������� ũ�⸦ ������ ����
        newSlime.transform.localScale = transform.localScale * 0.5f;

        // �������� Animator Controller�� slime���� slime 1�� ����
        Animator animator = newSlime.GetComponent<Animator>();
        if (animator != null)
        {
            animator.runtimeAnimatorController = splitSlimeAnimatorController;
        }

        // �������� Health ����
        Slime slimeComponent = newSlime.GetComponent<Slime>();
        if (slimeComponent != null)
        {
            slimeComponent.currentHealth = currentHealth / 2;
            slimeComponent.Health = Health / 2;
        }

        // �������� material�� �������� material�� �����ϰ� ����(��� blinkred �Ҷ� ������ �п��Ǵϱ�)
        Renderer[] renderers = newSlime.GetComponentsInChildren<Renderer>();
        Renderer prefabRenderer = slimePrefab.GetComponent<Renderer>();
        if (prefabRenderer != null)
        {
            foreach (var renderer in renderers)
            {
                renderer.material = prefabRenderer.material;
            }
        }
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        StartCoroutine(BlinkRed()); // �������� �Ծ��� �� BlinkRed ȣ��
    }

    protected override void Die()
    {
        HandleDeath(); // �⺻ Die ���� ��� ���� ������ ȣ��
    }
}
