using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class MainMenu : MonoBehaviour
{
    [Header("Spaceship References")]
    [SerializeField] private GameObject spaceshipContainer;

    [Header("Music")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField, Range(0f, 1f)] private float mainMenuMusicVolume = 1f;

    [Header("UI Document")]
    [SerializeField] UIDocument mainMenuDocument;

    [Header("Menus")]
    public GameObject mainMenu;
    public GameObject gameModeMenu;
    public GameObject settingsMenu;
    public GameObject colorsMenu;
    public GameObject leaderboardMenu;

    [Header("Main Menu Buttons")]
    private Button playButton;
    private Button settingsButton;
    private Button leaderboardButton;
    private Button exitButton;
    private Button colorsButton;

    // Events for button hover
    [Header("Button Events")]
    public UnityEvent playButtonHover;
    public UnityEvent settingsButtonHover;
    public UnityEvent leaderboardButtonHover;
    public UnityEvent exitButtonHover;
    public UnityEvent colorsButtonHover;


    private void OnEnable()
    {
        MainMenuMusicController.Play(mainMenuMusic, mainMenuMusicVolume);

        VisualElement root = mainMenuDocument.rootVisualElement;

        playButton = root.Q<Button>("playButton");
        settingsButton = root.Q<Button>("settingButton");
        leaderboardButton = root.Q<Button>("leaderboardButton");
        exitButton = root.Q<Button>("exitButton");
        colorsButton = root.Q<Button>("colorsButton");

        // Clicked
        playButton.clicked += PlayGame;
        settingsButton.clicked += DisplaySettingMenu;
        leaderboardButton.clicked += DisplayLeaderBoard;
        exitButton.clicked += ExitGame;

        // Hover and Exit
        playButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered play button!");
            playButtonHover.Invoke();
        });
        // playButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited play button!");
        // });
        settingsButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered settings button!");
            settingsButtonHover.Invoke();
        });
        // settingsButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited settings button!");
        // });
        leaderboardButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered leaderboard button!");
            leaderboardButtonHover.Invoke();
        });
        // leaderboardButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited leaderboard button!");
        // });
        exitButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered exit button!");
            exitButtonHover.Invoke();
        });
        // exitButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited exit button!");
        // });

        if (colorsButton != null)
        {
            colorsButton.clicked += DisplayColorsMenu;
            colorsButton.RegisterCallback<PointerEnterEvent>(evt => {
                // Debug.Log("Mouse entered colors button!");
                colorsButtonHover.Invoke();
            });
            // colorsButton.RegisterCallback<PointerLeaveEvent>(evt => {
            //     Debug.Log("Mouse exited colors button!");
            // });
        }
    }

    private void OnDisable()
    {
        playButton.clicked -= PlayGame;
        settingsButton.clicked -= DisplaySettingMenu;
        leaderboardButton.clicked -= DisplayLeaderBoard;
        exitButton.clicked -= ExitGame;

        playButton.UnregisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered play button!");
            playButtonHover.Invoke();
        });
        // playButton.UnregisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited play button!");
        // });
        settingsButton.UnregisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered settings button!");
            settingsButtonHover.Invoke();
        });
        // settingsButton.UnregisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited settings button!");
        // });
        leaderboardButton.UnregisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered leaderbaord button!");
            leaderboardButtonHover.Invoke();
        });
        // leaderboardButton.UnregisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited leaderboard button!");
        // });
        exitButton.UnregisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered exit button!");
            exitButtonHover.Invoke();
        });
        // exitButton.UnregisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited exit button!");
        // });

        if (colorsButton != null)
        {
            colorsButton.clicked -= DisplayColorsMenu;
            colorsButton.UnregisterCallback<PointerEnterEvent>(evt => {
                // Debug.Log("Mouse entered colors button!");
                colorsButtonHover.Invoke();
            });
            // colorsButton.UnregisterCallback<PointerLeaveEvent>(evt => {
            //     Debug.Log("Mouse exited colors button!");
            // });
        }

    }

    private void PlayGame()
    {
        mainMenu.SetActive(false);
        gameModeMenu.SetActive(true);
    }

    private void DisplaySettingMenu()
    {
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().DisableSpaceshipModel();
    }

    private void DisplayColorsMenu()
    {
        mainMenu.SetActive(false);
        colorsMenu.SetActive(true);
        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().DisableSpaceshipModel();
    }

    private void DisplayLeaderBoard()
    {
        mainMenu.SetActive(false);
        leaderboardMenu.SetActive(true);
        spaceshipContainer.GetComponent<SpaceshipMainMenuButtonHover>().DisableSpaceshipModel();
    }

    private void ExitGame()
    {
        Application.Quit();

        Debug.Log("game is exiting");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
