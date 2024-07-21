using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDome : MonoBehaviour
{
    public Player player;
    public GameObject sun;
    public GameObject moon;

    public Material skyMaterial;  // �ϴ� ������ �����մϴ�.
    public float dayDuration = 180f;  // �ݳ���(�ذ� �ݹ��� ���� �ð�)�� 180�ʷ� �����մϴ�.

    public float rotationSpeed = 10f;  // ���� ȸ�� �ӵ�
    private float time;

    private void Start()
    {
        // ���� �ʱ� ��ġ�� �¾��� �ݴ��� ����
        SetMoonInitialPosition();
    }

    private void Update()
    {
        DaySet();
        PlanetRotation();
    }

    private void DaySet()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        time += Time.deltaTime;
        float normalizedTime = (time % dayDuration) / dayDuration;

        Vector2 offset = new Vector2(normalizedTime, 0);
        skyMaterial.mainTextureOffset = offset;
    }

    private void PlanetRotation()
    {
        // �¾� ȸ��
        Vector3 sunRelativePos = player.transform.position - sun.transform.position;
        Quaternion sunRotation = Quaternion.LookRotation(sunRelativePos);
        sun.transform.rotation = Quaternion.Lerp(sun.transform.rotation, sunRotation, Time.deltaTime * rotationSpeed);
        sun.transform.RotateAround(player.transform.position, Vector3.right, (180f / dayDuration) * Time.deltaTime);

        // �� ȸ�� (���� �ݴ��� ��ġ�ϵ��� ����)
        Vector3 moonRelativePos = player.transform.position - moon.transform.position;
        Quaternion moonRotation = Quaternion.LookRotation(moonRelativePos);
        moon.transform.rotation = Quaternion.Lerp(moon.transform.rotation, moonRotation, Time.deltaTime * rotationSpeed);
        moon.transform.RotateAround(player.transform.position, Vector3.right, (180f / dayDuration) * Time.deltaTime);
    }

    private void SetMoonInitialPosition()
    {
        // ���� ��ġ�� �¾��� �ݴ��� ����
        Vector3 sunToPlayer = player.transform.position - sun.transform.position;
        moon.transform.position = player.transform.position + sunToPlayer;
    }
}
