using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class LeaderboardGameOver : MonoBehaviour
{
    public UIDocument leaderboardDocument;

    private Label firstName, firstScore;
    private Label secondName, secondScore;
    private Label thirdName, thirdScore;

    private readonly Color activeColor = new(0.4f, 0.8f, 1f, 0.4f);
    
    List<LeaderboardEntry> list = new List<LeaderboardEntry>();


    private void OnEnable()
    {
        VisualElement root = leaderboardDocument.rootVisualElement;

        firstName = root.Q<Label>("firstNameLabel");
        secondName = root.Q<Label>("secondNameLabel");
        thirdName = root.Q<Label>("thirdNameLabel");

        firstScore = root.Q<Label>("firstScoreLabel");
        secondScore = root.Q<Label>("secondScoreLabel");
        thirdScore = root.Q<Label>("thirdScoreLabel");

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
        // Optional: sort by score descending
        list.Sort((a, b) => b.score.CompareTo(a.score));

        Label[] nameLabels =
        {
            firstName, secondName, thirdName
        };
        Label[] scoreLabels =
        {
            firstScore, secondScore, thirdScore
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