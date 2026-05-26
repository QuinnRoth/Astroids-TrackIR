using UnityEngine;
using UnityEngine.UI;

public class CursorMovement : MonoBehaviour
{
    public RectTransform cursorTransform;

    private const float CursorSpeed = 2000f;
    private const float GameOverCursorSpeed = 3200f;
    private const float DeadZone = 0.075f;
    private const float GameOverDeadZone = 0.05f;

    private static bool useGameOverTuning;
    private Vector2 cursorPos;
    private float vertical = 0f;
    private float horizontal = 0f;

    public static void SetGameOverTuning(bool enabled)
    {
        useGameOverTuning = enabled;
    }

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

        float currentDeadZone = useGameOverTuning ? GameOverDeadZone : DeadZone;
        float currentSpeed = useGameOverTuning ? GameOverCursorSpeed : CursorSpeed;

        if (Mathf.Abs(horizontal) < currentDeadZone)
            horizontal = 0f;
        if (Mathf.Abs(vertical) < currentDeadZone)
            vertical = 0f;

        cursorPos += new Vector2(horizontal, vertical) * Time.unscaledDeltaTime * currentSpeed;
        
        RectTransform canvas = cursorTransform.root as RectTransform;

        cursorPos.x = Mathf.Clamp(cursorPos.x, canvas.rect.min.x, canvas.rect.max.x);
        cursorPos.y = Mathf.Clamp(cursorPos.y, canvas.rect.min.y, canvas.rect.max.y);

        cursorTransform.anchoredPosition = cursorPos;
    }
}
