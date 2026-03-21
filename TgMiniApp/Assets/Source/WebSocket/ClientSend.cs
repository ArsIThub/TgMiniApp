using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;

public class ClientSend : WsPingClient
{
    private async void Start()
    {
        _ws = new ClientWebSocket();

        var uri = new Uri("ws://localhost:8080");
        await _ws.ConnectAsync(uri, CancellationToken.None);
        Debug.Log("Client 1 connected");

        await SendText("Hello, Client 2!");
        Debug.Log("Sended");
    }
}
