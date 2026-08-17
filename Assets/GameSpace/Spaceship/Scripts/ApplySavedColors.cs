using System;
using System.Collections.Generic;
using UnityEngine;

public class ApplySavedColors : MonoBehaviour
{
    // Allows ColorMenu to tell all ApplySavedColors instances to refresh.
    public static event Action ColorsChanged;
    public static void NotifyColorsChanged() => ColorsChanged?.Invoke();

    private static readonly int ColorId = Shader.PropertyToID("_Color");
    private static readonly int EmissionId = Shader.PropertyToID("_EmissionColor");


    [Header("Target (optional)")]
    [SerializeField] private GameObject target;

    [Header("Renderers (optional)")]
    [Tooltip("If empty, script will auto-find renderers under Target (or this GameObject).")]
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    [Header("Material Filtering")]
    [Tooltip("Only apply to materials whose name contains this text. Leave empty to apply to all materials on these renderers.")]
    [SerializeField] private string materialNameContains = "";

    [Tooltip("If true, edits sharedMaterial (all users of that material). If false, edits material instances per renderer.")]
    [SerializeField] private bool useSharedMaterial = true;

    [Header("Which color to apply on this object?")]
    [SerializeField] private bool applySpaceshipColor = false;
    [SerializeField] private bool applyAsteroidColor = false;

    // Keys we support (new + old)
    private const string ShipPrefix = "ShipColor";

    // We support BOTH asteroid prefixes because your project used both at different times.
    private static readonly string[] AsteroidPrefixes = { "AstColor", "AsteroidColor" };

    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

    private void Awake()
    {
        AutoCollectRenderersIfNeeded();
    }

    private void OnEnable()
    {
        ColorsChanged += ApplyNow;
    }

    private void OnDisable()
    {
        ColorsChanged -= ApplyNow;
    }

    private void Start()
    {
        ApplyNow();
    }

    public void ApplyNow()
    {
        AutoCollectRenderersIfNeeded();

        if (renderers == null || renderers.Count == 0)
            return;

        if (applySpaceshipColor)
        {
            var shipColor = LoadColorAny(new[] { ShipPrefix }, Color.white);
            ApplyColorToRenderers(shipColor);
        }

        if (applyAsteroidColor)
        {
            var astColor = LoadColorAny(AsteroidPrefixes, Color.white);
            ApplyColorToRenderers(astColor);
        }
    }

    private void AutoCollectRenderersIfNeeded()
    {
        if (renderers != null && renderers.Count > 0)
            return;

        var root = target != null ? target.transform : transform;
        renderers = new List<Renderer>(root.GetComponentsInChildren<Renderer>(true));
    }

    private void ApplyColorToRenderers(Color color)
    {
        foreach (var r in renderers)
        {
            if (r == null) continue;

            var mats = useSharedMaterial ? r.sharedMaterials : r.materials;
            if (mats == null) continue;

            for (int i = 0; i < mats.Length; i++)
            {
                var m = mats[i];
                if (m == null) continue;

                if (!string.IsNullOrEmpty(materialNameContains) && !m.name.Contains(materialNameContains))
                    continue;

                // URP Lit: Base Map tint uses _BaseColor
                if (m.HasProperty(BaseColorId))
                {
                    // keep existing alpha just in case
                    var cur = m.GetColor(BaseColorId);
                    m.SetColor(BaseColorId, new Color(color.r, color.g, color.b, cur.a));
                }

                else if (m.HasProperty(ColorId))
                {
                    var cur = m.GetColor(ColorId);
                    m.SetColor(ColorId, new Color(color.r, color.g, color.b, cur.a));
                }

            }
        }
    }

    // Loads color from either:
    // - ints 0..255 (SetInt)
    // - floats 0..1 (SetFloat)
    // - floats 0..255 (some projects did this too)
    private Color LoadColorAny(string[] prefixes, Color fallback)
    {
        foreach (var p in prefixes)
        {
            if (TryLoadColor(p, out var c))
                return c;
        }
        return fallback;
    }

    private bool TryLoadColor(string prefix, out Color color)
    {
        string rKey = prefix + "_R";
        string gKey = prefix + "_G";
        string bKey = prefix + "_B";

        // Need at least R key to consider it "present".
        if (!PlayerPrefs.HasKey(rKey) || !PlayerPrefs.HasKey(gKey) || !PlayerPrefs.HasKey(bKey))
        {
            color = default;
            return false;
        }

        float r = ReadChannel01(rKey);
        float g = ReadChannel01(gKey);
        float b = ReadChannel01(bKey);

        color = new Color(r, g, b, 1f);
        return true;
    }

    private float ReadChannel01(string key)
    {

        int i = PlayerPrefs.GetInt(key, int.MinValue);
        float f = PlayerPrefs.GetFloat(key, float.NaN);


        if (i != int.MinValue && i >= 0 && i <= 255)
            return Mathf.Clamp01(i / 255f);


        if (!float.IsNaN(f))
        {
            // If stored as 0..1
            if (f >= 0f && f <= 1.5f)
                return Mathf.Clamp01(f);

            // If stored as 0..255
            if (f > 1.5f)
                return Mathf.Clamp01(f / 255f);
        }

        // fallback
        return 1f;
    }
}
