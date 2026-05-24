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
    private Button deleteButton;

    private readonly Dictionary<Button, Action> keyboardHandlers = new();

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
        deleteButton = root.Q<Button>("Delete");

        backButton.clicked += ToMainMenu;
        deleteButton.clicked += DeleteChar;

        RegisterKeyboardButtons(root);        

        scoreField.text = ScoreManager.Instance.GetScore().ToString();
    }

    void OnDisable()
    {
        backButton.clicked -= ToMainMenu;
        deleteButton.clicked -= DeleteChar;

        foreach (var pair in keyboardHandlers)
        {
            pair.Key.clicked -= pair.Value;
        }

        keyboardHandlers.Clear();
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
