using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    public Texture2D cursorTexture; // 커서로 사용할 텍스처
    public CursorMode cursorMode = CursorMode.Auto; // 커서 모드
    public Vector2 hotSpot = Vector2.zero; // 커서의 핫스팟

    void Start()
    {
        // 커서를 변경합니다.
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
}