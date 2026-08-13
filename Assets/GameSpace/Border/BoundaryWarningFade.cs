using UnityEngine;
using UnityEngine.UI;

public class BoundaryWarningFade : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Image whiteOverlay;

    [Header("Boundary Settings")]
    public Vector3 boxSize;
    public float warningDistance = 20f;

    [Header("Fade Settings")]
    public float maxAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private Vector3 halfSize;

    void Start()
    {
        if (GameModeMenu.gameModeSetting == 0)  // Trade show mode
        {
            boxSize = new Vector3(400f, 400f, 400f);
        }
        
        boxSize = player.GetComponent<Boundary>().boxSize;
        halfSize = boxSize * 0.5f;
    }

    void Update()
    {

        if (!player || !whiteOverlay) return;

        Vector3 pos = player.position;

        float distanceToWall = Mathf.Min(
            halfSize.x - Mathf.Abs(pos.x),
            halfSize.y - Mathf.Abs(pos.y),
            halfSize.z - Mathf.Abs(pos.z)
        );

        float targetAlpha = 0f;

        if (distanceToWall < warningDistance)
        {
            float t = 1f - (distanceToWall / warningDistance);
            targetAlpha = Mathf.Clamp01(t) * maxAlpha;
        }

        Color c = whiteOverlay.color;
        c.a = Mathf.Lerp(c.a, targetAlpha, Time.deltaTime * fadeSpeed);
        whiteOverlay.color = c;
    }
}
