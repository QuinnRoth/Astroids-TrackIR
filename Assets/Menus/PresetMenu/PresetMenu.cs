using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PresetMenu : MonoBehaviour
{
    [Header("Spaceship Reference")]
    [SerializeField] private GameObject spaceshipContainer;

    [Header("UI")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Menu Switching (match how your other menus work)")]
    [SerializeField] private GameObject presetMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameModeMenu;

    private VisualElement root;
    private VisualElement rootColors;
    private VisualElement rootPresets;
    private VisualElement MenuContainer;
    private VisualElement presetLeftColor;
    private VisualElement presetRightColor;

    private Button backButton;
    private Button doneButton;

    private readonly List<Button> shipSwatches = new();
    private readonly List<Button> astSwatches = new();
    private readonly List<Button> presets = new();

    // PlayerPrefs keys we will write
    private const string ShipPrefix = "ShipColor";

    // We write BOTH for asteroid to stay compatible with older code.
    private const string AstPrefixA = "AstColor";
    private const string AstPrefixB = "AsteroidColor";

    // Pick a palette that matches your neon style
    private static readonly Color32[] Palette =
    {
        new Color32(240, 250, 255, 255), // white
        new Color32(100, 182, 238, 255), // bright blue
        new Color32(255,   0,   0, 255), // intense red
        new Color32(170, 120, 255, 255), // purple
        new Color32(255,  80, 200, 255), // pink
        new Color32(255,  90,  70, 255), // red-ish
        new Color32(255, 166,   0, 255), // orange
        new Color32(255, 216,  74, 255), // yellow
        new Color32( 80, 255, 154, 255), // green
    };

    private static readonly Color32[] PresetColors =
    {
        // Default Optitrack brand themed
        new Color32(100, 182, 238, 255), // bright blue
        new Color32(255, 166,   0, 255), // orange
        // Active IO themed
        new Color32(197, 40, 46, 255), // red
        new Color32(255, 255, 255, 255), // white
        // TrackIR/Motive themed
        new Color32(255, 255, 255, 255), // white
        new Color32(255, 166,   0, 255), // orange
        // Planar themed
        new Color32(46, 84, 255, 255), // blue
        new Color32(255, 255, 255, 255), // white
        // OG Asteroids Arcade themed
        new Color32(255,  255,  0, 255), // yellow
        new Color32(0,  247, 247, 255), // cyan
        // Tron?
        new Color32(140,  217, 255, 255), // light blue
        new Color32(255,  122,  33, 255), // orange

    };

    private Color32 currentShip;
    private Color32 currentAst;

    private void OnEnable()
    {
        if (uiDocument == null) uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;
        MenuContainer = root.Q<VisualElement>("MenuContainer");
        rootPresets = root.Q<VisualElement>("rootPresets");

        backButton = root.Q<Button>("backButton");
        doneButton = root.Q<Button>("doneButton");

        // Gather swatch buttons by class

        presets.Clear();

        foreach (var b in rootPresets.Query<Button>().ToList())
        {
            if (b.ClassListContains("presets")) presets.Add(b);
        }
        SetupPresets(presets);

        currentShip = LoadColor32(ShipPrefix, defaultColor: new Color32(255, 166, 0, 255));
        currentAst = LoadColor32(AstPrefixA, defaultColor: new Color32(100, 182, 238, 255));
        // If AstColor not present, try AsteroidColor
        if (!HasRGB(AstPrefixA) && HasRGB(AstPrefixB))
            currentAst = LoadColor32(AstPrefixB, currentAst);



        backButton.clicked += OnBack;
        doneButton.clicked += OnDone;
    }

    private void OnDisable()
    {
        if (backButton != null) backButton.clicked -= OnBack;
        if (doneButton != null) doneButton.clicked -= OnDone;
    }

    private void SetupPresets(List<Button> presets)
    {
        int count = Mathf.Min(presets.Count, PresetColors.Length / 2);

        for (int i = 0; i < count; i++)
        {
            int idx = i * 2;
            var btn = presets[i];

            var left = btn.Q<VisualElement>("leftColor");
            var right = btn.Q<VisualElement>("rightColor");

            if (left != null)
                left.style.backgroundColor = new StyleColor(PresetColors[idx]);
            if (right != null)
                right.style.backgroundColor = new StyleColor(PresetColors[idx + 1]);

            btn.clicked += () =>
            {
                currentShip = PresetColors[idx];
                currentAst = PresetColors[idx + 1];
                // Save ship (ints 0..255)
                SaveColor32(ShipPrefix, currentShip);

                // Save asteroid in BOTH key formats to avoid future mismatch
                SaveColor32(AstPrefixA, currentAst);
                SaveColor32(AstPrefixB, currentAst);

                PlayerPrefs.Save();

                // Tell all ApplySavedColors instances to refresh right now
                ApplySavedColors.NotifyColorsChanged();


            };
        }
    }
    

    private void SetSelected(List<Button> swatches, Button selected)
    {
        foreach (var b in swatches)
            b.EnableInClassList("ColorSwatchSelected", b == selected);
    }

    private void OnBack()
    {
        // Just close without saving
        if (presetMenu != null) presetMenu.SetActive(false);
        if (mainMenu != null) mainMenu.SetActive(true);
        
        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().EnableSpaceshipModel();
    }

    private void OnDone()
    {
        

        // Close menu
        if (presetMenu != null) presetMenu.SetActive(false);
        if (gameModeMenu != null) gameModeMenu.SetActive(true);

        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().EnableSpaceshipModel();
    }

    private static void SaveColor32(string prefix, Color32 c)
    {
        PlayerPrefs.SetInt(prefix + "_R", c.r);
        PlayerPrefs.SetInt(prefix + "_G", c.g);
        PlayerPrefs.SetInt(prefix + "_B", c.b);
    }


    private static bool HasRGB(string prefix)
    {
        return PlayerPrefs.HasKey(prefix + "_R")
            && PlayerPrefs.HasKey(prefix + "_G")
            && PlayerPrefs.HasKey(prefix + "_B");
    }

    private static Color32 LoadColor32(string prefix, Color32 defaultColor)
    {
        if (!HasRGB(prefix))
            return defaultColor;

        // Prefer int if present
        int r = PlayerPrefs.GetInt(prefix + "_R", defaultColor.r);
        int g = PlayerPrefs.GetInt(prefix + "_G", defaultColor.g);
        int b = PlayerPrefs.GetInt(prefix + "_B", defaultColor.b);

        // If ints look wrong (like 0 but float exists), fall back to floats
        if ((r == 0 && g == 0 && b == 0) && PlayerPrefs.HasKey(prefix + "_R"))
        {
            float rf = PlayerPrefs.GetFloat(prefix + "_R", defaultColor.r / 255f);
            float gf = PlayerPrefs.GetFloat(prefix + "_G", defaultColor.g / 255f);
            float bf = PlayerPrefs.GetFloat(prefix + "_B", defaultColor.b / 255f);

            // handle 0..1 or 0..255 floats
            rf = rf > 1.5f ? rf / 255f : rf;
            gf = gf > 1.5f ? gf / 255f : gf;
            bf = bf > 1.5f ? bf / 255f : bf;

            return new Color(rf, gf, bf, 1f);
        }

        return new Color32((byte)Mathf.Clamp(r, 0, 255), (byte)Mathf.Clamp(g, 0, 255), (byte)Mathf.Clamp(b, 0, 255), 255);
    }
}
