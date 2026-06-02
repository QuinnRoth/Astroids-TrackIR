using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;


public class GameOverMenu : MonoBehaviour
{
    [SerializeField] UIDocument gameOverMenuDocument;

    public GameObject gameOverMenu;

    private Label scoreField;
    private Label nameField;
    private Button backButton;
    private Button replayButton;
    private Button deleteButton;

    private readonly Dictionary<Button, Action> keyboardHandlers = new();

    private Label firstName, firstScore;
    private Label secondName, secondScore;
    private Label thirdName, thirdScore;
    private Label fourthName, fourthScore;
    private Label fifthName, fifthScore;
    private Label sixthName, sixthScore;

    List<LeaderboardEntry> list = new List<LeaderboardEntry>();

    private const float KeyWidth = 112f;
    private const float KeyHeight = 100f;
    private const float KeyHorizontalMargin = 18f;
    private const float KeyVerticalMargin = 4f;
    private const float KeyFontSize = 68f;
    private const float DeleteKeyWidth = 224f;
    private const float DeleteKeyFontSize = 68f;

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

        scoreField = root.Q<Label>("ScoreField");
        nameField = root.Q<Label>("NameField");
        backButton = root.Q<Button>("BackButton");
        replayButton = root.Q<Button>("ReplayButton");
        deleteButton = root.Q<Button>("Delete");

        backButton.clicked += ToMainMenu;
        replayButton.clicked += ReplayGame;
        deleteButton.clicked += DeleteChar;

        RegisterKeyboardButtons(root);
        ApplyKeyboardSizing(root);

        scoreField.text = ScoreManager.Instance.GetScore().ToString();

        firstName = root.Q<Label>("firstNameLabel");
        secondName = root.Q<Label>("secondNameLabel");
        thirdName = root.Q<Label>("thirdNameLabel");
        fourthName = root.Q<Label>("fourthNameLabel");
        fifthName = root.Q<Label>("fifthNameLabel");
        sixthName = root.Q<Label>("sixthNameLabel");

        firstScore = root.Q<Label>("firstScoreLabel");
        secondScore = root.Q<Label>("secondScoreLabel");
        thirdScore = root.Q<Label>("thirdScoreLabel");
        fourthScore = root.Q<Label>("fourthScoreLabel");
        fifthScore = root.Q<Label>("fifthScoreLabel");
        sixthScore = root.Q<Label>("sixthScoreLabel");

        switch (GameModeMenu.gameModeSetting)
        {
            case 0:
                TradeShowMode();
                break;
            case 1:
                EndlessMode();
                break;
            case 2:
                WaveMode();
                break;
        }
    }

    void OnDisable()
    {
        backButton.clicked -= ToMainMenu;
        replayButton.clicked -= ReplayGame;
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
    
    private void DeleteChar()
    {
        if (nameField.text != "")
            nameField.text = nameField.text[..^1];
    }

    private void ToMainMenu()
    {
        LeaderboardManager.Instance.AddScore(nameField.text, ScoreManager.Instance.GetScore(), GameModeMenu.gameModeSetting);

        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    private void ReplayGame()
    {
        LeaderboardManager.Instance.AddScore(nameField.text, ScoreManager.Instance.GetScore(), GameModeMenu.gameModeSetting);

        Time.timeScale = 1f;

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentScene);
    }

    // leaderboard function
    private void TradeShowMode()
    {
        list = LeaderboardManager.Instance.tsEntries;
        PopulateLeaderboard();
    }

    private void EndlessMode()
    {
        list = LeaderboardManager.Instance.endEntries;
        PopulateLeaderboard();
    }

    private void WaveMode()
    {
        list = LeaderboardManager.Instance.waveEntries;
        PopulateLeaderboard();
    }

    private void PopulateLeaderboard()
    {
        list.Sort((a, b) => b.score.CompareTo(a.score));

        Label[] nameLabels =
        {
            firstName, secondName, thirdName, fourthName, fifthName, sixthName
        };
        Label[] scoreLabels =
        {
            firstScore, secondScore, thirdScore, fourthScore, fifthScore, sixthScore
        };

        for (int i = 0; i < nameLabels.Length; i++)
        {
            if (i < list.Count)
            {
                nameLabels[i].text = list[i].playerName;
                scoreLabels[i].text = $"{list[i].score}";
            }
            else
            {
                nameLabels[i].text = "----";
                scoreLabels[i].text = "--";
            }
        }
    }
}
