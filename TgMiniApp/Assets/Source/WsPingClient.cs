using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

public class WsPingClient : MonoBehaviour
{
    private ClientWebSocket _ws;

    private async void Start()
    {
        _ws = new ClientWebSocket();

        var uri = new Uri("ws://localhost:8080");
        await _ws.ConnectAsync(uri, CancellationToken.None);
        Debug.Log("Connected");

        await SendText("ping");
        Debug.Log("Sended");

        string reply = await RecieveText();
        Debug.Log(reply);
    }

    private async Task SendText(string text)
    {
        var bytes = Encoding.UTF8.GetBytes(text);
        await _ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    private async Task<string> RecieveText() 
    {
        var buffer = new byte[1024];

        var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        return Encoding.UTF8.GetString(buffer, 0, result.Count);
    }

    private async void OnApplicationQuit() 
    {
        if (_ws != null && _ws.State == WebSocketState.Open) 
        {
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
        }
    }
}
