using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyDome : MonoBehaviour
{
    public Player player;
    public GameObject sun;
    public GameObject moon;


    public Material skyMaterial;  // 하늘 재질을 참조합니다.
    public float dayDuration = 180f;  // 반나절(해가 반바퀴 도는 시간)을 180초로 설정합니다.

    public float rotationSpeed = 10f;  // 해의 회전 속도

    private Light sunLight;
    private float time;
    private float offsetValue;

    private void Start()
    {
        sunLight = sun.GetComponent<Light>();
        skyMaterial.mainTextureOffset = new Vector2(0.62f, 0);
        offsetValue = 0.62f;
        // 달의 초기 위치를 태양의 반대편에 설정
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
        // 태양 회전
        Vector3 sunRelativePos = player.transform.position - sun.transform.position;
        Quaternion sunRotation = Quaternion.LookRotation(sunRelativePos);
        sun.transform.rotation = Quaternion.Lerp(sun.transform.rotation, sunRotation, Time.deltaTime * rotationSpeed);
        sun.transform.RotateAround(player.transform.position, Vector3.back, (180f / dayDuration) * Time.deltaTime);

        // 달 회전 (해의 반대편에 위치하도록 설정)
        Vector3 moonRelativePos = player.transform.position - moon.transform.position;
        Quaternion moonRotation = Quaternion.LookRotation(moonRelativePos);
        moon.transform.rotation = Quaternion.Lerp(moon.transform.rotation, moonRotation, Time.deltaTime * rotationSpeed);
        moon.transform.RotateAround(player.transform.position, Vector3.back, (180f / dayDuration) * Time.deltaTime);
    }

    private void SetMoonInitialPosition()
    {
        // 달의 위치를 태양의 반대편에 설정
        Vector3 sunToPlayer = player.transform.position - sun.transform.localPosition;
        moon.transform.position = player.transform.position + sunToPlayer;
    }
}
