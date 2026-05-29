using UnityEngine;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour
{
    public RectTransform cursorTransform;

    private const float CursorSpeed = 3200f;
    private const float DeadZone = 0.05f;

    private Vector2 cursorPos;
    private float vertical = 0f;
    private float horizontal = 0f;

    void Start()
    {
    #if !UNITY_EDITOR
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    #endif
    
        cursorPos = cursorTransform.anchoredPosition;
    }

    void Update()
    {
        if (TrackIRManager.Instance != null && TrackIRManager.Instance.IsTracking)
        {
            horizontal = TrackIRManager.Instance.HeadYaw;
            vertical = -TrackIRManager.Instance.HeadPitch;
        }

        if (Mathf.Abs(horizontal) < DeadZone)
            horizontal = 0f;
        if (Mathf.Abs(vertical) < DeadZone)
            vertical = 0f;

        cursorPos += new Vector2(horizontal, vertical) * Time.unscaledDeltaTime * CursorSpeed;
        
        RectTransform canvas = cursorTransform.root as RectTransform;

        cursorPos.x = Mathf.Clamp(cursorPos.x, canvas.rect.min.x, canvas.rect.max.x);
        cursorPos.y = Mathf.Clamp(cursorPos.y, canvas.rect.min.y, canvas.rect.max.y);

        cursorTransform.anchoredPosition = cursorPos;
    }
}
