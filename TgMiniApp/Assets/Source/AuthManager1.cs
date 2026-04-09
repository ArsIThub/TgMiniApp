using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class AuthManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI playerNameText;
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI playerIdText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Config")]
    [SerializeField] private string serverUrl = "https://incompletely-nonburdensome-florrie.ngrok-free.dev";

    private TelegramWebApp telegramWebApp;

    [Serializable]
    private class AuthRequest
    {
        public string initData;
    }

    [Serializable]
    public class PlayerData
    {
        public long telegramId;
        public string name;
        public int score;
    }

    [Serializable]
    public class AuthResponse
    {
        public bool success;
        public string message;
        public PlayerData player;
    }

    private void Start()
    {
        telegramWebApp = FindObjectOfType<TelegramWebApp>();

        if (telegramWebApp == null)
        {
            SetStatus("TelegramWebApp not found");
            return;
        }

        StartCoroutine(AuthCoroutine());
    }

    private IEnumerator AuthCoroutine()
    {
        SetStatus("Telegram auth...");

        string initData = telegramWebApp.Init();

        Debug.Log("InitData: " + initData);

        if (string.IsNullOrEmpty(initData))
        {
            SetStatus("initData is empty. Open app from Telegram.");
            yield break;
        }

        AuthRequest requestData = new AuthRequest
        {
            initData = initData
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            bool hasError = request.result != UnityWebRequest.Result.Success;

            if (hasError)
            {
                Debug.LogError("Auth request error: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
                SetStatus("Server auth error");
                yield break;
            }

            string responseJson = request.downloadHandler.text;
            Debug.Log("Server response: " + responseJson);

            AuthResponse response = JsonUtility.FromJson<AuthResponse>(responseJson);

            if (response == null)
            {
                SetStatus("Response parse error");
                yield break;
            }

            if (!response.success || response.player == null)
            {
                SetStatus("Auth failed: " + response.message);
                yield break;
            }

            ShowPlayer(response.player);
            SetStatus("Auth success");
        }
    }

    private void ShowPlayer(PlayerData player)
    {
        if (playerNameText != null)
            playerNameText.text = "Name: " + player.name;

        if (playerScoreText != null)
            playerScoreText.text = "Score: " + player.score;

        if (playerIdText != null)
            playerIdText.text = "Telegram ID: " + player.telegramId;
    }

    private void SetStatus(string message)
    {
        Debug.Log(message);

        if (statusText != null)
            statusText.text = message;
    }
}