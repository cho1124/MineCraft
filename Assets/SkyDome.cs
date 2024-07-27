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

    private Light sunLight;
    private float time;
    private float offsetValue;

    private void Start()
    {
        sunLight = sun.GetComponent<Light>();
        skyMaterial.mainTextureOffset = new Vector2(0.62f, 0);
        offsetValue = 0.62f;
        // ���� �ʱ� ��ġ�� �¾��� �ݴ��� ����
        //SetMoonInitialPosition();
    }

    private void Update()
    {
        DaySet();
        PlanetRotation();
    }

    private void DaySet()
    {
        gameObject.transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z);
        //time += Time.deltaTime;

        offsetValue += Time.deltaTime / dayDuration * (1.25f - 0.62f);
        
        sunLight.bounceIntensity = 0;

        Vector2 offset = new Vector2(offsetValue, 0);

        if (sunLight.intensity <= 0)
        { 
            sunLight.intensity = 0;
        }

        if(sunLight.intensity >= 100000)
        {
            sunLight.intensity = 100000;
        }


        if (sun.transform.rotation.x <= 30)
        {
            sunLight.intensity--;
        }
        else
        { 
            sunLight.intensity++;
        }



        skyMaterial.mainTextureOffset = offset;
    }

    private void PlanetRotation()
    {
        // �¾� ȸ��
        Vector3 sunRelativePos = player.transform.position - sun.transform.position;
        Quaternion sunRotation = Quaternion.LookRotation(sunRelativePos);
        sun.transform.rotation = Quaternion.Lerp(sun.transform.rotation, sunRotation, Time.deltaTime * rotationSpeed);
        sun.transform.RotateAround(player.transform.position, Vector3.back, (180f / dayDuration) * Time.deltaTime);

        // �� ȸ�� (���� �ݴ��� ��ġ�ϵ��� ����)
        Vector3 moonRelativePos = player.transform.position - moon.transform.position;
        Quaternion moonRotation = Quaternion.LookRotation(moonRelativePos);
        moon.transform.rotation = Quaternion.Lerp(moon.transform.rotation, moonRotation, Time.deltaTime * rotationSpeed);
        moon.transform.RotateAround(player.transform.position, Vector3.back, (180f / dayDuration) * Time.deltaTime);
    }

    private void SetMoonInitialPosition()
    {
        // ���� ��ġ�� �¾��� �ݴ��� ����
        Vector3 sunToPlayer = player.transform.position - sun.transform.localPosition;
        moon.transform.position = player.transform.position + sunToPlayer;
    }
}
