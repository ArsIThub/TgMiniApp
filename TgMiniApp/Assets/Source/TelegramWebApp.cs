//using System.Runtime.InteropServices;
using UnityEngine;

public class TelegramWebApp : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetTelegramInitData();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern string GetTelegramUser();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void TelegramReady();

    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void TelegramExpand();
#endif

    public string Init()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        TelegramReady();
        TelegramExpand();
        return GetTelegramInitData();
#else
        return "test_init_data";
#endif
    }

    public string GetUserJson()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        return GetTelegramUser();
#else
        return "{\"id\":123,\"first_name\":\"Test\"}";
#endif
    }
}