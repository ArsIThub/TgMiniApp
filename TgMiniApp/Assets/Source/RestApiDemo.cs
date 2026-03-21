using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;

public class RestApiDemo : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField scoreInput;
    [Space]
    [SerializeField] private TextMeshProUGUI outputText;
    [Space]
    [SerializeField] private Button scoreButton;
    [SerializeField] private Button leaderboardButton;

    private const string _baseUrl = "http://localhost:3000";

    private void Awake()
    {
        scoreButton.onClick.AddListener(SendScore);
        leaderboardButton.onClick.AddListener(GetLeaderBoard);
    }

    private void SendScore() 
    {
        string playername = nameInput.text;

        if (playername == "") 
        {
            ShowText("Enter your name!");
            return;
        }

        if (!int.TryParse(scoreInput.text, out int score)) 
        {
            ShowText("Score is invalid!");
            return;
        }

        StartCoroutine(SaveScore(playername, score));
    }

    private void GetLeaderBoard() 
    {
        StartCoroutine(RequestLeaderboard());
    }

    private IEnumerator SaveScore(string name, int score) 
    {
        string url = $"{_baseUrl}/save-score";

        SaveScoreRequest requestData = new SaveScoreRequest 
        {
            name = name,
            score = score,
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) 
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            ShowText("Sending score");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) 
            {
                ShowText("Error: " + request.error);
                yield break;
            }

            ShowText("Server response: " + request.downloadHandler.text);
        }
    }

    private IEnumerator RequestLeaderboard() 
    {
        string url = $"{_baseUrl}/leaderboard";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) 
        {
            ShowText("Loading leaderboard");

            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                ShowText("Error: " + request.error);
                yield break;
            }

            string json = request.downloadHandler.text;
            LeaderboardResponse response = JsonUtility.FromJson<LeaderboardResponse>(json);

            if (response == null || response.leaderboard == null) 
            {
                ShowText("Error: " + json);
                yield break;
            }

            string leaderboardName = "Leaderboard\n\n";

            for (int i = 0; i < response.leaderboard.Length; i++) 
            {
                var player = response.leaderboard[i];
                leaderboardName += $"{i + 1}. {player.name} - {player.score}\n";
            }

            ShowText(leaderboardName);
        }
    }

    private void ShowText(string message) 
    {
        if (outputText.text != null)
        {
            outputText.text = message;
        }
    }

    private void OnDestroy()
    {
        scoreButton.onClick.RemoveAllListeners();
        leaderboardButton.onClick.RemoveAllListeners();
    }
}

[Serializable]
public class SaveScoreRequest 
{
    public string name;
    public int score;
}

[Serializable]
public class PlayerData
{
    public int id;
    public string name;
    public int score;
}

[Serializable]
public class LeaderboardResponse 
{
    public bool success;
    public PlayerData[] leaderboard;
}
