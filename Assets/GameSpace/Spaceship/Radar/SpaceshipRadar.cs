using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class SpaceshipRadar : MonoBehaviour
{
    // Asteroid List
    private List<GameObject> asteroidList = new List<GameObject>();

    [Header("Spaceship References")]
    [SerializeField] private GameObject spaceship;

    [Header("Radar")]
    public RectTransform radarIconPrefab;   // Assign your UI icon prefab here
    public Camera playerCamera;
    public float screenEdgeBuffer = 40f;
    public int maxRadarIcons = 3;           // How many asteroidList to track
    public float maxRadarDistance = 150f;   // Hide icons past this distance

    [Header("Radar Warning Visuals")]
    public float minRadarScale = 0.3f;
    // Size of icon when asteroid is far away
    // Lower = smaller distant warning
    // Higher = distant asteroidList stay more noticeable

    public float maxRadarScale = 1.3f;
    // Size of icon when asteroid is very close
    // Higher = stronger "danger" feeling
    // Controls how dramatic the growth is

    public float minRadarAlpha = 0.1f;
    // Transparency when asteroid is far away
    // Lower = very faint warning
    // Higher = always visible even at distance

    public float maxRadarAlpha = 1.0f;
    // Transparency when asteroid is close
    // Usually leave at 1.0 (fully visible)

    public float radarVisualMaxDistance = 100f;
    // Distance range used to scale size and transparency
    // Larger = slower visual change
    // Smaller = faster visual change

    public float blinkStartDistance = 70f;
    // Distance where blinking begins
    // Larger = blinking starts earlier
    // Smaller = blinking only when very close

    public float slowBlinkSpeed = 1f;
    // Blink speed when asteroid first enters danger range
    // Lower = slow pulse
    // Higher = faster blinking

    public float fastBlinkSpeed = 15f;
    // Blink speed when asteroid is extremely close
    // Creates aggressive warning behavior

    public float blinkMinAlphaMultiplier = 0.05f;
    // How dim the icon gets during blink
    // Lower = stronger flash
    // Higher = softer blink

    private List<RectTransform> radarIcons = new List<RectTransform>();

    private void Start()
    {
        if(GameModeMenu.gameModeSetting == 0)
            gameObject.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateRadar();
    }

    // ---------------- RADAR SYSTEM ----------------

    private void EnsureRadarIcons()
    {
        if (radarIcons.Count == maxRadarIcons)
            return;

        foreach (var icon in radarIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        radarIcons.Clear();

        for (int i = 0; i < maxRadarIcons; i++)
        {
            RectTransform icon = Instantiate(radarIconPrefab, radarIconPrefab.parent);
            radarIcons.Add(icon);
        }
    }

    private void UpdateRadar()
    {
        if (radarIconPrefab == null || playerCamera == null || spaceship == null)
            return;

        EnsureRadarIcons();

        List<GameObject> validAsteroids = new List<GameObject>();

        foreach (GameObject asteroid in asteroidList)
        {
            if (asteroid != null)
                validAsteroids.Add(asteroid);
        }

        validAsteroids.Sort((a, b) =>
            Vector3.Distance(spaceship.transform.position, a.transform.position)
            .CompareTo(
            Vector3.Distance(spaceship.transform.position, b.transform.position)));

        for (int i = 0; i < radarIcons.Count; i++)
        {
            if (i >= validAsteroids.Count)
            {
                radarIcons[i].gameObject.SetActive(false);
                continue;
            }

            GameObject target = validAsteroids[i];
            float distance = Vector3.Distance(spaceship.transform.position, target.transform.position);

            if (distance > maxRadarDistance)
            {
                radarIcons[i].gameObject.SetActive(false);
                continue;
            }

            Vector3 viewportPos = playerCamera.WorldToViewportPoint(target.transform.position);

            bool isVisible =
                viewportPos.z > 0 &&
                viewportPos.x > 0 && viewportPos.x < 1 &&
                viewportPos.y > 0 && viewportPos.y < 1;

            if (isVisible)
            {
                radarIcons[i].gameObject.SetActive(false);
                continue;
            }

            radarIcons[i].gameObject.SetActive(true);

            Vector3 direction = (target.transform.position - spaceship.transform.position).normalized;
            Vector3 screenDir = playerCamera.WorldToScreenPoint(spaceship.transform.position + direction) -
                                playerCamera.WorldToScreenPoint(spaceship.transform.position);

            float angle = Mathf.Atan2(screenDir.y, screenDir.x) * Mathf.Rad2Deg;
            radarIcons[i].rotation = Quaternion.Euler(0, 0, angle - 90f);

            Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
            Vector3 edgePosition = screenCenter + screenDir.normalized * (Screen.height / 2f - screenEdgeBuffer);

            radarIcons[i].position = edgePosition;

            // Scale icon based on distance
            float t = Mathf.Clamp01(1f - (distance / radarVisualMaxDistance));
            float scale = Mathf.Lerp(minRadarScale, maxRadarScale, t);

            radarIcons[i].localScale = Vector3.one * scale;

            // Change icon transparency based on distance
            Graphic radarGraphic = radarIcons[i].GetComponent<Graphic>();

            if (radarGraphic != null)
            {
                float alpha = Mathf.Lerp(minRadarAlpha, maxRadarAlpha, t);

                // Blink icon when asteroid is very close
                if (distance <= blinkStartDistance)
                {
                    float blinkT = Mathf.Clamp01(1f - (distance / blinkStartDistance));
                    float blinkSpeed = Mathf.Lerp(slowBlinkSpeed, fastBlinkSpeed, blinkT);
                    float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));

                    alpha *= Mathf.Lerp(blinkMinAlphaMultiplier, 1f, blink);
                }

                Color color = radarGraphic.color;
                color.a = alpha;
                radarGraphic.color = color;
            }
        }
    }

    public void HandleTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Asteroid"))
            asteroidList.Add(other.gameObject);
    }

    public void HandleTriggerExit(Collider other) 
    {
        if (other.CompareTag("Asteroid"))
            asteroidList.Remove(other.gameObject);
    }
}
