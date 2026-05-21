using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffscreenRadar : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public List<Transform> asteroids = new List<Transform>();
    public RectTransform radarIconPrefab;   // prefab now

    [Header("UI Sliders")]
    public Slider distanceSlider;
    public Slider asteroidCountSlider;

    [Header("Settings")]
    public float screenEdgeMargin = 50f;
    public float maxDistance = 100f;

    [Header("Radar Warning Visuals")]
    public float minIconScale = 0.4f;
    public float maxIconScale = 1.5f;
    public float minIconAlpha = 0.25f;
    public float maxIconAlpha = 1.0f;
    public float blinkStartDistance = 35f;
    public float slowBlinkSpeed = 2f;
    public float fastBlinkSpeed = 12f;
    public float blinkMinAlphaMultiplier = 0.25f;

    private List<RectTransform> radarIcons = new List<RectTransform>();

    void Start()
    {
        if(GameModeMenu.gameModeSetting == 0)
            this.enabled = false;
                    
        if (player == null)
            player = transform;

        // Distance slider
        if (distanceSlider != null)
        {
            distanceSlider.value = maxDistance;
            distanceSlider.onValueChanged.AddListener(UpdateDistance);
        }

        // Asteroid amount slider
        if (asteroidCountSlider != null)
        {
            asteroidCountSlider.value = asteroids.Count;
            asteroidCountSlider.onValueChanged.AddListener(UpdateAsteroidAmount);
        }

        CreateRadarIcons();
    }

    void Update()
    {
        asteroids.RemoveAll(asteroid => asteroid == null);

        while (radarIcons.Count < asteroids.Count)
        {
            RectTransform icon = Instantiate(radarIconPrefab, radarIconPrefab.parent);
            icon.gameObject.SetActive(true);
            radarIcons.Add(icon);
        }

        for (int i = 0; i < radarIcons.Count; i++)
        {
            if (i >= asteroids.Count)
            {
                radarIcons[i].gameObject.SetActive(false);
                continue;
            }

            if (asteroids[i] == null)
            {
                radarIcons[i].gameObject.SetActive(false);
                continue;
            }

            UpdateRadarForAsteroid(asteroids[i], radarIcons[i]);
        }
    }

    public void AddAsteroid(Transform asteroid)
    {
        if (asteroid == null)
            return;

        AsteroidClass asteroidClass = asteroid.GetComponent<AsteroidClass>();

        if (asteroidClass == null)
            return;

        if (asteroidClass.GetAsteroidType() != AsteroidClass.InheritanceType.Basic)
            return;

        if (!asteroids.Contains(asteroid))
        {
            asteroids.Add(asteroid);
            CreateRadarIcons();
        }
    }

    public void RemoveAsteroid(Transform asteroid)
    {
        if (asteroid == null)
            return;

        if (asteroids.Contains(asteroid))
        {
            asteroids.Remove(asteroid);
            CreateRadarIcons();
        }
    }

    void UpdateDistance(float value)
    {
        maxDistance = value;
    }

    void UpdateAsteroidAmount(float value)
    {
        int targetCount = Mathf.RoundToInt(value);

        while (asteroids.Count > targetCount)
        {
            asteroids.RemoveAt(asteroids.Count - 1);
        }

        CreateRadarIcons();
    }

    void CreateRadarIcons()
    {
        // Clear old icons
        foreach (var icon in radarIcons)
        {
            if (icon != null)
                Destroy(icon.gameObject);
        }

        radarIcons.Clear();

        // Create one icon per asteroid
        foreach (var asteroid in asteroids)
        {
            RectTransform icon = Instantiate(radarIconPrefab, radarIconPrefab.parent);
            icon.gameObject.SetActive(true);
            radarIcons.Add(icon);
        }
    }

    void UpdateRadarForAsteroid(Transform asteroid, RectTransform icon)
    {
        float distance = Vector3.Distance(player.position, asteroid.position);

        if (distance > maxDistance)
        {
            icon.gameObject.SetActive(false);
            return;
        }

        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(asteroid.position);

        bool isVisible =
            viewportPoint.z > 0 &&
            viewportPoint.x > 0 && viewportPoint.x < 1 &&
            viewportPoint.y > 0 && viewportPoint.y < 1;

        if (isVisible)
        {
            icon.gameObject.SetActive(false);
            return;
        }

        icon.gameObject.SetActive(true);

        Vector3 screenPoint = Camera.main.WorldToScreenPoint(asteroid.position);

        if (screenPoint.z < 0)
        {
            screenPoint *= -1;
        }

        screenPoint.x = Mathf.Clamp(screenPoint.x, screenEdgeMargin, Screen.width - screenEdgeMargin);
        screenPoint.y = Mathf.Clamp(screenPoint.y, screenEdgeMargin, Screen.height - screenEdgeMargin);

        icon.position = screenPoint;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Vector3 direction = (screenPoint - screenCenter).normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        icon.rotation = Quaternion.Euler(0, 0, angle - 90f);

        UpdateRadarIconVisuals(icon, distance);
    }

    void UpdateRadarIconVisuals(RectTransform icon, float distance)
    {
        float closeness = 1f - Mathf.Clamp01(distance / maxDistance);

        float scale = Mathf.Lerp(minIconScale, maxIconScale, closeness);
        icon.localScale = Vector3.one * scale;

        Graphic graphic = icon.GetComponent<Graphic>();

        if (graphic == null)
            return;

        float alpha = Mathf.Lerp(minIconAlpha, maxIconAlpha, closeness);

        if (distance <= blinkStartDistance)
        {
            float blinkCloseness = 1f - Mathf.Clamp01(distance / blinkStartDistance);
            float blinkSpeed = Mathf.Lerp(slowBlinkSpeed, fastBlinkSpeed, blinkCloseness);
            float blink = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));

            alpha *= Mathf.Lerp(blinkMinAlphaMultiplier, 1f, blink);
        }

        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}