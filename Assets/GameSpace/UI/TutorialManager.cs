using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class TutorialManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UIDocument tutorialDocument;
    [SerializeField] private SpaceshipDamage player;
    [SerializeField] private AsteroidSpawner asteroidSpawner;
    [SerializeField] private GameObject legacyHudCanvasRoot;
    [SerializeField] private Label tutorialLabel;
    [SerializeField] private ProgressBar tutorialProgressBar;
    [SerializeField] private GroupBox toggleGroup;
    [SerializeField] private Toggle toggle1;
    [SerializeField] private Toggle toggle2;
    [SerializeField] private Toggle toggle3;
    [SerializeField] private Toggle toggle4;
    [SerializeField] private Toggle toggle5;

    public bool tutorialComplete = false;
    private PlayerInputActions spaceshipControls;
    private InputAction shootAction;    
    private InputAction thrustInput;
    private InputAction pitchInput;
    private InputAction yawInput;
    private InputAction rollInput;
    private int currentTutorialStep = 0;
    private string[] tutorialTexts;
    private Vector3 lastPosition = Vector3.zero;
    private float distanceMoved;
    private Quaternion lastRotation = Quaternion.identity;
    private float totalDegreesRotated;
    private float shotsFired = 0f;
    private bool hasSpawnedCurrentStep = false;


    // Notes:
    // Movement: 
        // Control Thrust:
        // Lean closer to the screen to move forward.
        // Lean farther back to slow down or move backward.
        // Fill up a meter by moving a certain distance to continue to the next stage.

        //Steer with Your Head:
        // Look Left / Right to Yaw.
        // Look Up / Down to Pitch.
        // Fill up a meter by rotating the ship a certain amount of degrees to continue to the next stage.
    void OnEnable()
    {
        if (!tutorialDocument) tutorialDocument = GetComponent<UIDocument>();
        shootAction.performed += OnShoot;
        var root = tutorialDocument.rootVisualElement;
        tutorialLabel = root.Q<Label>("tutorialLabel");
        tutorialProgressBar = root.Q<ProgressBar>("tutorialProgressBar");
        toggleGroup = root.Q<GroupBox>("ToggleGroup");
        toggle1 = root.Q<Toggle>("Toggle1");
        toggle2 = root.Q<Toggle>("Toggle2");
        toggle3 = root.Q<Toggle>("Toggle3");
        toggle4 = root.Q<Toggle>("Toggle4");
        toggle5 = root.Q<Toggle>("Toggle5");
        currentTutorialStep = 0;
        SetTutorialText();
    }

    void OnDisable()
    {
        if (shootAction != null)
        {
            shootAction.performed -= OnShoot;
        }
    }

    private void Awake()
    {
        legacyHudCanvasRoot?.SetActive(false);
        
        
        // Initialize input actions before using them
        thrustInput = InputSystem.actions.FindAction("Thrust");
        pitchInput = InputSystem.actions.FindAction("Pitch");
        yawInput = InputSystem.actions.FindAction("Yaw");
        shootAction = InputSystem.actions.FindAction("Shoot");
        tutorialTexts = new string[]
        {
            "Control Thrust:\nLean closer (" + thrustInput.GetBindingDisplayString(2) + ") or farther (" + thrustInput.GetBindingDisplayString(1) + ") from the screen to move forward or backward.",
            "Steer with Your Head:\nLook Left (" + yawInput.GetBindingDisplayString(1) + ") / Right (" + yawInput.GetBindingDisplayString(2) + ") to control Yaw. \nLook Up (" + pitchInput.GetBindingDisplayString(1) + ") / Down (" + pitchInput.GetBindingDisplayString(2) + ") to control Pitch.",
            "Destroy Astroids by pressing " + shootAction.GetBindingDisplayString(0) + " to shoot.",
            "The bomb asteroid will explode when hit. And the healing asteroid will drop a health pack when hit.",
            "Tutorial complete! Game Start!"
        };
    }

    private void OnShoot(InputAction.CallbackContext _)
    {
        if (currentTutorialStep == 2)
        {
            shotsFired++;
        }
    }

    void Update()
    {
        if (!tutorialComplete)
        {
            TutorialStep();
        }
    }

    public void SetTutorialText()
    {
        if (tutorialLabel != null)
        {
            tutorialLabel.text = tutorialTexts[currentTutorialStep];
        }
        ClearTutorialProgressBar();
    }

    private void TutorialStep()
    {
        Debug.Log("Tutorial step: " + currentTutorialStep);
        switch (currentTutorialStep)
        {
            case 0:
                tutorialProgressBar.value = GetDistanceMoved() / 100f;
                if (tutorialProgressBar.value >= 1f)
                {
                    AdvanceTutorialStep();
                }
                break;

            case 1:
                tutorialProgressBar.value = GetDegreesRotated() / 360f;
                if (tutorialProgressBar.value >= 1f)
                {
                    AdvanceTutorialStep();
                    tutorialProgressBar.RemoveFromHierarchy();
                }
                break;

            case 2:
                Debug.Log("Tutorial step 2");
                toggleGroup.visible = true;
                toggle1.value = GetShotsFired() > 0;
                toggle2.value = GetShotsFired() > 1;
                toggle3.value = GetShotsFired() > 2;
                toggle4.value = GetShotsFired() > 3;
                toggle5.value = GetShotsFired() > 4;
                if (!hasSpawnedCurrentStep)
                {
                    asteroidSpawner.SpawnOneAsteroid(0, new Vector3(player.transform.localPosition.x, player.transform.localPosition.y, player.transform.localPosition.z + 2000));
                    
                    hasSpawnedCurrentStep = true;
                }

                if (GetShotsFired() >= 5)
                {
                    asteroidSpawner.RemoveAllAsteroids();
                    AdvanceTutorialStep();
                    toggleGroup.RemoveFromHierarchy();
                }
                break;

            case 3:
                Debug.Log("Tutorial step 3" + hasSpawnedCurrentStep);
                if (!hasSpawnedCurrentStep)
                {
                    Debug.Log("Spawning asteroids for tutorial step 3");
                    asteroidSpawner.SpawnOneAsteroid(1, new Vector3(player.transform.localPosition.x - 200, player.transform.localPosition.y, player.transform.localPosition.z + 2000));
                    asteroidSpawner.SpawnOneAsteroid(2, new Vector3(player.transform.localPosition.x + 200, player.transform.localPosition.y, player.transform.localPosition.z + 2000));
                    hasSpawnedCurrentStep = true;
                }

                if (AsteroidSpawner.asteroidCount == 0)
                {
                    AdvanceTutorialStep();
                }
                break;

            case 4:
                Debug.Log("Tutorial step 4");
                tutorialComplete = true;
                asteroidSpawner.StartGame();
                break;
        }
    }

    private void AdvanceTutorialStep()
    {
        currentTutorialStep++;
        hasSpawnedCurrentStep = false;
        Debug.Log("Advanced to tutorial step: " + currentTutorialStep);
        if (currentTutorialStep < tutorialTexts.Length)
        {
            SetTutorialText();
        }
        else
        {
            tutorialDocument.gameObject.SetActive(false);
        }
    }

    private void ClearTutorialProgressBar()
    {
        if (tutorialProgressBar != null)
        {
            tutorialProgressBar.value = 0f;
        }
    }

    private float GetDistanceMoved()
    {
        distanceMoved += Vector3.Distance(player.transform.position, lastPosition);
        lastPosition = player.transform.position;
        return distanceMoved;
    }

    private float GetDegreesRotated()
    {
        if(lastRotation == Quaternion.identity)
        {
            lastRotation = player.transform.rotation;
        }
        totalDegreesRotated += Quaternion.Angle(player.transform.rotation, lastRotation);
        lastRotation = player.transform.rotation;
        return totalDegreesRotated;
    }

    private float GetShotsFired()
    {
        Debug.Log("Shots fired: " + shotsFired);
        return shotsFired;
    }
}
