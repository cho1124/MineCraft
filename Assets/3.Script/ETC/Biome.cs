using UnityEngine;

[CreateAssetMenu(fileName = "BiomeData", menuName = "ScriptableObjects/Biome", order = 1)]
public class Biome : ScriptableObject
{
    public string biomeName; // 생물군계 이름
    public Color biomeColor; // 생물군계 색상
    public GameObject[] creatures; // 생물군계에 속한 생명체들
    public Texture2D texture; // 생물군계의 텍스처
    // 다른 생물군계 관련 속성을 여기에 추가
}

//
