using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;


public class GameOverMenu : MonoBehaviour
{
    [SerializeField] UIDocument gameOverMenuDocument;
    [SerializeField] UIDocument gameOverLeaderboard;

    public GameObject gameOverMenu;

    private Label scoreField;
    private Label nameField;
    private Button backButton;
    private Button deleteButton;

    private readonly Dictionary<Button, Action> keyboardHandlers = new();

    private const float KeyWidth = 112f;
    private const float KeyHeight = 100f;
    private const float KeyHorizontalMargin = 18f;
    private const float KeyVerticalMargin = 4f;
    private const float KeyFontSize = 68f;
    private const float DeleteKeyWidth = 300f;
    private const float DeleteKeyFontSize = 52f;

    private static readonly string[] keyboardRowIds =
    {
        "numbersRow", "topRow", "middleRow", "bottomRow"
    };

    private static readonly string[] keyIds = new[]
    {
        "1","2","3","4","5","6","7","8","9","0",
        "Q","W","E","R","T","Y","U","I","O","P",
        "A","S","D","F","G","H","J","K","L",
        "Z","X","C","V","B","N","M"
    };


    void OnEnable()
    {
        VisualElement root = gameOverMenuDocument.rootVisualElement;
        if (gameOverLeaderboard != null)
            gameOverLeaderboard.gameObject.SetActive(true);

        scoreField = root.Q<Label>("ScoreField");
        nameField = root.Q<Label>("NameField");
        backButton = root.Q<Button>("BackButton");
        deleteButton = root.Q<Button>("Delete");

        backButton.clicked += ToMainMenu;
        deleteButton.clicked += DeleteChar;

        RegisterKeyboardButtons(root);
        ApplyKeyboardSizing(root);

        scoreField.text = ScoreManager.Instance.GetScore().ToString();
    }

    void OnDisable()
    {
        if (gameOverLeaderboard != null)
            gameOverLeaderboard.gameObject.SetActive(false);

        backButton.clicked -= ToMainMenu;
        deleteButton.clicked -= DeleteChar;

        foreach (var pair in keyboardHandlers)
        {
            pair.Key.clicked -= pair.Value;
        }

        keyboardHandlers.Clear();
    }

    private void ApplyKeyboardSizing(VisualElement root)
    {
        var keyboard = root.Q<VisualElement>("Keyboard");
        keyboard.style.width = Length.Percent(100);
        keyboard.style.alignSelf = Align.Stretch;

        foreach (string rowId in keyboardRowIds)
        {
            var row = root.Q<VisualElement>(rowId);
            row.style.justifyContent = Justify.Center;
            row.style.marginLeft = 0;
            row.style.marginRight = 0;
            row.style.marginTop = 4;
            row.style.marginBottom = 4;
        }

        root.Q<VisualElement>("numbersRow").style.marginTop = 16;

        foreach (string keyId in keyIds)
            StyleKeyboardButton(root.Q<Button>(keyId), KeyWidth, KeyFontSize);

        StyleKeyboardButton(deleteButton, DeleteKeyWidth, DeleteKeyFontSize);
    }

    private static void StyleKeyboardButton(Button button, float width, float labelFontSize)
    {
        if (button == null) return;

        button.style.width = width;
        button.style.height = KeyHeight;
        button.style.marginLeft = KeyHorizontalMargin;
        button.style.marginRight = KeyHorizontalMargin;
        button.style.marginTop = KeyVerticalMargin;
        button.style.marginBottom = KeyVerticalMargin;
        button.style.paddingLeft = 16;
        button.style.paddingRight = 16;

        var label = button.Q<Label>("label");
        if (label != null)
        {
            label.style.fontSize = labelFontSize;
            label.style.unityTextAlign = TextAnchor.MiddleCenter;
        }
    }

    private void RegisterKeyboardButtons(VisualElement root)
    {
        foreach (string keyId in keyIds)
        {
            var button = root.Q<Button>(keyId);
            void Handler() => AppendCharacter(keyId);
            button.clicked += Handler;
            keyboardHandlers[button] = Handler;
        }
    }

    private void AppendCharacter(string chara)
    {
        if (nameField.text.Length < 6)
            nameField.text = (nameField.text ?? string.Empty) + chara;
    }

    private void ToMainMenu()
    {
        LeaderboardManager.Instance.AddScore(nameField.text, ScoreManager.Instance.GetScore(), GameModeMenu.gameModeSetting);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void DeleteChar()
    {
        if (nameField.text != "")
            nameField.text = nameField.text[..^1];
    }
}
