using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using TMPro;
using System;
using UnityEngine.UI;

public class RestApiDemo : MonoBehaviour
{
    [SerializeField] private TMP_InputField scoreInput;
    [Space]
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    [Space]
    [SerializeField] private Button scoreButton;
    [SerializeField] private Button leaderboardButton;

    private string _telegramInitData;
    private string _telegramUserName = "Unknown";
    private bool _telegramReady;

    private const string _baseUrl = "http://localhost:3000";

    #if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern string GetTelegramInitData();
    #endif

    private void Awake()
    {
        scoreButton.onClick.AddListener(SendScore);
        leaderboardButton.onClick.AddListener(GetLeaderBoard);
    }

    private void Start()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
        GetTelegramInitData();
    #else
        ShowText("Init works only in WebGL");
        #endif
    }

    private void SendTelegramInitData(string telegramInitData) 
    {
        _telegramInitData = telegramInitData;
        _telegramReady = !string.IsNullOrEmpty(_telegramInitData);
        ShowText($"Telegram init data: {_telegramInitData}");
    }

    private void SetTelegramUserName(string userName) 
    {
        _telegramUserName = userName;
        playerNameText.text = _telegramUserName;
    }

    private void OnTelegramError(string error) 
    {
        ShowText($"Error: {error}");
    }

    private void SendScore() 
    {
        if (!_telegramReady) 
        {
            ShowText("Telegram data is not ready");
            return;
        }

        if (!int.TryParse(scoreInput.text, out int score)) 
        {
            ShowText("Error: score field is invalid");
            return;
        }

        StartCoroutine(SaveScore(score));
    }

    private void GetLeaderBoard() 
    {
        StartCoroutine(RequestLeaderboard());
    }

    private void AuthTelegram() 
    {
        if (!_telegramReady) 
        {
            ShowText("Telegram data is not ready");
            return;
        }

        StartCoroutine(AuthTelegramRoutine());
    }

    private IEnumerator AuthTelegramRoutine() 
    {
        string url = $"{_baseUrl}/auth/telegram";

        TelegramAuthRequest requestData = new TelegramAuthRequest
        {
            initData = _telegramInitData,
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST")) 
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            ShowText("Sending auth");
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) 
            {
                ShowText("Error: " + request.error);
                yield break;
            }

            ShowText("Auth success: " + request.downloadHandler.text);
        }
    }

    private IEnumerator SaveScore(int score) 
    {
        string url = $"{_baseUrl}/save-score";

        SaveScoreTelegramRequest requestData = new SaveScoreTelegramRequest 
        {
            initData = _telegramInitData,
            score = score
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
public class TelegramAuthRequest 
{
    public string initData;
}

[Serializable]
public class SaveScoreTelegramRequest 
{
    public string initData;
    public int score;
}

[Serializable]
public class PlayerData
{
    public long telegramId;
    public string name;
    public int score;
}

[Serializable]
public class LeaderboardResponse 
{
    public bool success;
    public PlayerData[] leaderboard;
}
