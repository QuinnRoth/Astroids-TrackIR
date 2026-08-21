using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

using System;

public class SettingsMenu : MonoBehaviour
{
    [Header("Spaceship Reference")]
    [SerializeField] private GameObject spaceshipContainer;

    [Header("")]
    [SerializeField] UIDocument settingsMenuDocument;

    public GameObject settingsMenu;
    public GameObject mainMenu;
    public GameObject audioMenu;

    private Button angularMomentum;
    private Label pitchScale;
    private Button pitchDec, pitchInc;
    private Label rollScale;
    private Button rollDec, rollInc;
    private Label yawScale;
    private Button yawDec, yawInc;
    private Button fullScreen;
    private Button quickStart;
    private Button pov;
    private Button backButton;
    private Button defaultsButton;
    private Button audioButton;

    [SerializeField] private InputActionAsset inputActions;
    private InputAction shootAction;
    private Button rebindShootButton;

    private void OnEnable()
    {
        VisualElement root = settingsMenuDocument.rootVisualElement;

        // get the UI element
        angularMomentum = root.Q<Button>("angularMomentum");

        pitchScale = root.Q<Label>("pitchScale");
        pitchDec = root.Q<Button>("pitchDec");
        pitchInc = root.Q<Button>("pitchInc");

        rollScale = root.Q<Label>("rollScale");
        rollDec = root.Q<Button>("rollDec");
        rollInc = root.Q<Button>("rollInc");

        yawScale = root.Q<Label>("yawScale");
        yawDec = root.Q<Button>("yawDec");
        yawInc = root.Q<Button>("yawInc");
        quickStart = root.Q<Button>("quickStart");
        fullScreen = root.Q<Button>("fullScreen");
        pov = root.Q<Button>("pointofView");

        backButton = root.Q<Button>("backButton");
        defaultsButton = root.Q<Button>("defaultsButton");
        audioButton = root.Q<Button>("audioButton");

        rebindShootButton = root.Q<Button>("rebindShoot");
        shootAction = inputActions.FindAction("Shoot");
        LoadBinding();
        UpdateRebindButtonText();

        // get the current values
        GetSettingsPrefs();

        // handler methods
        angularMomentum.clicked += AMToggle;

        pitchDec.clicked += DecPitch;
        pitchInc.clicked += IncPitch;

        rollDec.clicked += DecRoll;
        rollInc.clicked += IncRoll;

        yawDec.clicked += DecYaw;
        yawInc.clicked += IncYaw;

        quickStart.clicked += QSToggle;
        fullScreen.clicked += FSToggle;
        pov.clicked += PoVToggle;

        backButton.clicked += LeaveMenu;
        defaultsButton.clicked += RestoreDefaults;
        audioButton.clicked += ToAudioMenu;

        rebindShootButton.clicked += StartRebind;
    }

    private void GetSettingsPrefs()
    {
        if (PlayerPrefs.HasKey("angMoment"))
        {
            if (PlayerPrefs.GetInt("angMoment") == 1)
                angularMomentum.text = "ON";
            else
                angularMomentum.text = "OFF";
        }
        else
            angularMomentum.text = "OFF";
        
        if (PlayerPrefs.HasKey("ptchScl"))
            pitchScale.text = PlayerPrefs.GetFloat("ptchScl").ToString();
        else
            pitchScale.text = "1.0";     // default values in SpaceshipMovement.cs

        if (PlayerPrefs.HasKey("rollScl"))
            rollScale.text = PlayerPrefs.GetFloat("rollScl").ToString();
        else
            rollScale.text = "1.1";

        if (PlayerPrefs.HasKey("yawScl"))
            yawScale.text = PlayerPrefs.GetFloat("yawScl").ToString();
        else
            yawScale.text = "1.5";

        if (PlayerPrefs.HasKey("quickStart"))
        {
            if (PlayerPrefs.GetInt("quickStart") == 1)
                quickStart.text = "ON";
            else
                quickStart.text = "OFF";
        }
        else
            quickStart.text = "OFF";

        if (PlayerPrefs.HasKey("fullScrn"))
        {
            if (PlayerPrefs.GetInt("fullScrn") == 1)
            {
                fullScreen.text = "ON";
                Screen.fullScreen = true;
            }
            else
            {
                fullScreen.text = "OFF";
                Screen.fullScreen = false;
            }
        }
        else
        {
            fullScreen.text = "OFF";
            Screen.fullScreen = false;
        }

        if (PlayerPrefs.HasKey("povFrst"))
        {
            if (PlayerPrefs.GetInt("povFrst") == 1)
            {
                pov.text = "First";
                CameraSwitcher.isFirstPerson = true;
            }
            else
            {
                pov.text = "Third";
                CameraSwitcher.isFirstPerson = false;
            }
        }
        else
        {
            pov.text = "Third";
            CameraSwitcher.isFirstPerson = false;
        }

        
    }
    private void OnDisable()
    {
        //save playerPrefs
        PlayerPrefs.SetInt("angMoment", angularMomentum.text == "ON" ? 1 : 0);
        PlayerPrefs.SetFloat("ptchScl", float.Parse(pitchScale.text));
        PlayerPrefs.SetFloat("rollScl", float.Parse(rollScale.text));
        PlayerPrefs.SetFloat("yawScl", float.Parse(yawScale.text));
        PlayerPrefs.SetInt("quickStart", quickStart.text == "ON" ? 1 : 0);
        PlayerPrefs.SetInt("fullScrn", fullScreen.text == "ON" ? 1 : 0);
        PlayerPrefs.SetInt("povFrst", pov.text == "First" ? 1 : 0);
        PlayerPrefs.Save();

        angularMomentum.clicked -= AMToggle;

        pitchDec.clicked -= DecPitch;
        pitchInc.clicked -= IncPitch;

        rollDec.clicked -= DecRoll;
        rollInc.clicked -= IncRoll;

        yawDec.clicked -= DecYaw;
        yawInc.clicked -= IncYaw;

        quickStart.clicked -= QSToggle;
        fullScreen.clicked -= FSToggle;
        pov.clicked -= PoVToggle;

        backButton.clicked -= LeaveMenu;
        defaultsButton.clicked -= RestoreDefaults;
        audioButton.clicked -= ToAudioMenu;

        rebindShootButton.clicked -= StartRebind;
    }

    private void StartRebind()
    {
        rebindShootButton.text = "Set Key";

        shootAction.Disable();

        shootAction.PerformInteractiveRebinding(0)
            .WithControlsExcluding("Mouse")
            .OnComplete(operation =>
            {
                operation.Dispose();
                shootAction.Enable();

                SaveBinding();
                UpdateRebindButtonText();
            })
            .Start();
    }

    private void SaveBinding()
    {
        string rebinds = inputActions.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString("shootRebind", rebinds);
        PlayerPrefs.Save();
    }

    private void LoadBinding()
    {
        if (PlayerPrefs.HasKey("shootRebind"))
        {
            string rebinds = PlayerPrefs.GetString("shootRebind");
            inputActions.LoadBindingOverridesFromJson(rebinds);
        }
    }

    private void UpdateRebindButtonText()
    {
        string binding = shootAction.GetBindingDisplayString(0);
        rebindShootButton.text = binding;
    }

    private void AMToggle()
    {
        if(angularMomentum.text == "OFF")
            angularMomentum.text = "ON";
        else
            angularMomentum.text = "OFF";
    }

    private void DecPitch()
    {
        float value = float.Parse(pitchScale.text);
        if(value > 0.1f)
            pitchScale.text = (value - 0.1f).ToString();
    }

    private void IncPitch()
    {
        float value = float.Parse(pitchScale.text);
        if(value < 3.0f)
            pitchScale.text = (value + 0.1f).ToString();
    }

    private void DecRoll()
    {
        float value = float.Parse(rollScale.text);
        if(value > 0.1f)
            rollScale.text = (value - 0.1f).ToString();
    }

    private void IncRoll()
    {
        float value = float.Parse(rollScale.text);
        if(value < 3.0f)
            rollScale.text = (value + 0.1f).ToString();
    }

    private void DecYaw()
    {
        float value = float.Parse(yawScale.text);
        if(value > 0.1f)
            yawScale.text = (value - 0.1f).ToString();
    }

    private void IncYaw()
    {
        float value = float.Parse(yawScale.text);
        if(value < 3.0f)
            yawScale.text = (value + 0.1f).ToString();
    }

    private void QSToggle()
    {
        if(quickStart.text == "OFF")
            quickStart.text = "ON";
        else
            quickStart.text = "OFF";
    }

    private void FSToggle()
    {
        Screen.fullScreen = !Screen.fullScreen;

        if(fullScreen.text == "OFF")
            fullScreen.text = "ON";
        else
            fullScreen.text = "OFF";
    }

    private void PoVToggle()
    {
        if(pov.text == "First")
            pov.text = "Third";
        else
            pov.text = "First";
    }

    private void RestoreDefaults()
    {
        angularMomentum.text = "OFF";
        pitchScale.text = "1.0";
        rollScale.text = "1.1";
        yawScale.text =  "1.5";
        fullScreen.text = "ON";
        pov.text = "Third";

        shootAction.RemoveBindingOverride(0);
        SaveBinding();
        UpdateRebindButtonText();
    }

    private void LeaveMenu()
    {
        settingsMenu.SetActive(false);
        mainMenu.SetActive(true);
        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().EnableSpaceshipModel();
    }

    private void ToAudioMenu()
    {
        settingsMenu.SetActive(false);
        audioMenu.SetActive(true);
    }
}
