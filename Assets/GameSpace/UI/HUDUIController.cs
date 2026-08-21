using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class HUDUIController : MonoBehaviour
{
    public static HUDUIController Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private UIDocument hudDocument;
    [SerializeField] private SpaceshipDamage playerDamage;
    [SerializeField] private SpaceshipMovement playerMovement;
    [SerializeField] private GameObject legacyHudCanvasRoot;

    [Header("Lives")]
    [SerializeField] private int maxLives = 3;
    [Header("Score")]
    [SerializeField] private int zeroPad = 6;

    [Header("Heat")]
    [SerializeField] private float heatIncrement = 7.0f;
    [SerializeField] private float cooldownRate = 25.0f;
    private Label scoreLabel;
    private VisualElement healthSegmentsRoot;
    private VisualElement[] segments;
    private VisualElement heatBar;
    private VisualElement heatBarContents;
    private VisualElement Gauge;
    private VisualElement GaugeFill;
    private PlayerInputActions spaceshipControls;
    private InputAction shootAction;    

    private int lastLives = -999;
    private int lastScore = -999;
    private int score = 0;

    void OnEnable()
    {
        if (!hudDocument) hudDocument = GetComponent<UIDocument>();
        if (hudDocument == null) return;

        if (shootAction != null)
            shootAction.performed += OnShoot;

        var root = hudDocument.rootVisualElement;
        if (root == null) return;

        scoreLabel = root.Q<Label>("scoreLabel");
        healthSegmentsRoot = root.Q<VisualElement>("healthSegments");
        heatBar = root.Q<VisualElement>("HeatGauge");
        Gauge = root.Q<VisualElement>("Gauge");
        GaugeFill = root.Q<VisualElement>("GaugeFill");
        heatBarContents = root.Q<VisualElement>("HeatFill");

        if (heatBar != null)
            UpdateHeat();

        BuildSegments();
        ForceRefresh();
    }

    void OnDisable()
    {
        if (shootAction != null)
            shootAction.performed -= OnShoot;
    }

    void Update()
    {
        if (!playerDamage) return;

        int lives = Mathf.Clamp(playerDamage.playerHealth, 0, maxLives);

        score = ScoreManager.Instance ? ScoreManager.Instance.GetScore() : 0;

        if (lives != lastLives)
        {
            UpdateLives(lives);
            lastLives = lives;
        }

        if (score != lastScore)
        {
            UpdateScore(score);
            lastScore = score;
        }
        UpdateSpeed();
        UpdateHeat();
        
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        shootAction = InputSystem.actions.FindAction("Shoot");
        if (legacyHudCanvasRoot != null)
            legacyHudCanvasRoot.SetActive(false);
    }

    private void BuildSegments()
    {
        healthSegmentsRoot.Clear();
        segments = new VisualElement[maxLives];

        for (int i = 0; i < maxLives; i++)
        {
            var seg = new VisualElement();
            seg.AddToClassList("segment");
            healthSegmentsRoot.Add(seg);
            segments[i] = seg;
        }
    }

    private Color GetHeatColor(float value)
    {
        float normalized = Mathf.Clamp01(value / 100f);

        if (normalized < 0.5f)
            return Color.Lerp(Color.green, Color.yellow, normalized * 2f);

        return Color.Lerp(Color.yellow, Color.red, (normalized - 0.5f) * 2f);
    }

    private void UpdateSpeed()
    {
        if (playerMovement == null || GaugeFill == null)
            return;

        if (playerMovement.MaxSpeed <= 0f)
        {
            GaugeFill.style.height = new Length(0f, LengthUnit.Percent);
            return;
        }

        float speedRatio = Mathf.Clamp01(playerMovement.speed / playerMovement.MaxSpeed);
        GaugeFill.style.height = new Length(speedRatio * 100f, LengthUnit.Percent);
    }

    private void UpdateLives(int lives)
    {
        // Decide what color the *filled* segments should use
        string fillClass = null;

        if (lives >= maxLives)
            fillClass = "segment-good";          // 3/3
        else if (lives == maxLives - 1)
            fillClass = "segment-warn";          // 2/3
        else if (lives > 0)
            fillClass = "segment-danger";        // 1/3

        for (int i = 0; i < segments.Length; i++)
        {
            var seg = segments[i];

            // clear previous state
            seg.RemoveFromClassList("segment-empty");
            seg.RemoveFromClassList("segment-good");
            seg.RemoveFromClassList("segment-warn");
            seg.RemoveFromClassList("segment-danger");

            // apply new state
            if (i >= lives)
                seg.AddToClassList("segment-empty");
            else if (!string.IsNullOrEmpty(fillClass))
                seg.AddToClassList(fillClass);
            // else: leave default ".segment" styling
        }
    }
    private void OnShoot(InputAction.CallbackContext context)
    {
        if (!CanShoot)
            return;

        AddHeat();
    }

    private float currentHeat;
    private bool overheated;
    public bool CanShoot => !overheated;

    private void UpdateHeat()
    {
        if (heatBar != null)
        {
            if (currentHeat >= 100f)
            {
                overheated = true;
            }
            else if (currentHeat <= 0f)
            {
                overheated = false;
            }

            if (currentHeat >= 0f)
            {
                currentHeat = Mathf.Clamp(currentHeat - cooldownRate * Time.deltaTime, 0f, 100f);
            }

            if (heatBarContents != null)
            {
                heatBarContents.style.backgroundColor = new StyleColor(GetHeatColor(currentHeat));
                heatBarContents.style.height = new Length(Mathf.Clamp(currentHeat, 0f, 100f), LengthUnit.Percent);
            }
        }
    }

    public void AddHeat()
    {
        if (overheated)
            return;

        currentHeat = Mathf.Clamp(currentHeat + heatIncrement, 0f, 100f);
    }

    private void UpdateScore(int score)
    {
        if (scoreLabel != null)
            scoreLabel.text = $"SCORE {score.ToString().PadLeft(zeroPad, '0')}";
    }

    private void ForceRefresh()
    {
        if (!playerDamage) return;
        UpdateLives(Mathf.Clamp(playerDamage.playerHealth, 0, maxLives));
        UpdateScore(ScoreManager.Instance ? ScoreManager.Instance.GetScore() : 0);
        UpdateSpeed();
    }
}
