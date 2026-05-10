using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class GameModeMenu : MonoBehaviour
{
    [SerializeField] UIDocument gameModeMenuDocument;

    public static int gameModeSetting = 2;

    public GameObject gameModeMenu;
    public GameObject mainMenu;


    private Button tradeShowButton;
    private Button endlessButton;
    private Button waveButton;
    private Button backButton;

    // Events for button hover
    [Header("Button Events")]
    public UnityEvent tradeShowButtonHover;
    public UnityEvent endlessButtonHover;
    public UnityEvent waveButtonHover;
    public UnityEvent backButtonHover;


    private void OnEnable()
    {
        VisualElement root = gameModeMenuDocument.rootVisualElement;

        tradeShowButton = root.Q<Button>("tradeShowButton");
        endlessButton = root.Q<Button>("endlessButton");
        waveButton = root.Q<Button>("waveButton");
        backButton = root.Q<Button>("backButton");


        tradeShowButton.clicked += TradeShowMode;
        endlessButton.clicked += EndlessMode;
        waveButton.clicked += WaveMode;
        backButton.clicked += MenuBack;

        // Hover and Exit
        tradeShowButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered tradeShow button!");
            tradeShowButtonHover.Invoke();
        });
        // tradeShowButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited tradeShow button!");
        // });
        endlessButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered endless button!");
            endlessButtonHover.Invoke();
        });
        // endlessButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited endless button!");
        // });
        waveButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered wave button!");
            waveButtonHover.Invoke();
        });
        // waveButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited wave button!");
        // });
        backButton.RegisterCallback<PointerEnterEvent>(evt => {
            // Debug.Log("Mouse entered back button!");
            backButtonHover.Invoke();
        });
        // backButton.RegisterCallback<PointerLeaveEvent>(evt => {
        //     Debug.Log("Mouse exited back button!");
        // });
    }

    private void OnDisable()
    {
        tradeShowButton.clicked -= TradeShowMode;
        endlessButton.clicked -= EndlessMode;
        waveButton.clicked -= WaveMode;
        backButton.clicked -= MenuBack;

        tradeShowButton.UnregisterCallback<PointerEnterEvent>(evt => {
            Debug.Log("Mouse entered tradeShow button!");
            tradeShowButtonHover.Invoke();
        });
        tradeShowButton.UnregisterCallback<PointerLeaveEvent>(evt => {
            Debug.Log("Mouse exited tradeShow button!");
        });
        endlessButton.UnregisterCallback<PointerEnterEvent>(evt => {
            Debug.Log("Mouse entered endless button!");
            endlessButtonHover.Invoke();
        });
        endlessButton.UnregisterCallback<PointerLeaveEvent>(evt => {
            Debug.Log("Mouse exited endless button!");
        });
        waveButton.UnregisterCallback<PointerEnterEvent>(evt => {
            Debug.Log("Mouse entered wave button!");
            waveButtonHover.Invoke();
        });
        waveButton.UnregisterCallback<PointerLeaveEvent>(evt => {
            Debug.Log("Mouse exited wave button!");
        });
        backButton.UnregisterCallback<PointerEnterEvent>(evt => {
            Debug.Log("Mouse entered back button!");
            backButtonHover.Invoke();
        });
        backButton.UnregisterCallback<PointerLeaveEvent>(evt => {
            Debug.Log("Mouse exited back button!");
        });
    }

    private void TradeShowMode()
    {
        gameModeSetting = 0;        // 0 for trade show
        SceneManager.LoadScene("GameScene");
    }
    
    private void EndlessMode()
    {
        gameModeSetting = 1;        // 1 for endless
        SceneManager.LoadScene("GameScene");
    }

    private void WaveMode()
    {
        gameModeSetting = 2;        // 2 for waves
        SceneManager.LoadScene("GameScene");
    }

    private void MenuBack()
    {
        gameModeMenu.SetActive(false);
        mainMenu.SetActive(true);
    }
}