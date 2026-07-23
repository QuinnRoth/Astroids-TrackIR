using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class TutorialManager : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private UIDocument tutorialDocument;
    [SerializeField] private SpaceshipDamage player;
    [SerializeField] private GameObject legacyHudCanvasRoot;
    [SerializeField] private Label tutorialLabel;
    [SerializeField] private ProgressBar tutorialProgressBar;

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
            "Destroy Astroids by pressing " + shootAction.GetBindingDisplayString(0) + " to shoot."
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
        updateTutorialProgressBar();
        if (tutorialProgressBar.value >= 1f)
        {
            currentTutorialStep++;
            if (currentTutorialStep < tutorialTexts.Length)
            {
                SetTutorialText();
            }
            else
            {
                tutorialDocument.gameObject.SetActive(false);
            }
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

    private void updateTutorialProgressBar()
    {
        if (tutorialProgressBar != null)
        {
            if (currentTutorialStep == 0)
            {
                // increas the progress bar based on player moving 100 units
                tutorialProgressBar.value = GetDistanceMoved() / 100f;
            }
            else if (currentTutorialStep == 1)
            {
                // increase the progress bar based on player rotating N degrees
                tutorialProgressBar.value = GetDegreesRotated() / 720f;
            }
            else if (currentTutorialStep == 2)
            {
                // increase the progress bar based on player shooting 5 shots
                tutorialProgressBar.value = GetShotsFired() / 5f;
            }

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
