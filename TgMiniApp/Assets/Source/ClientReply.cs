using UnityEngine;
using System;
using System.Net.WebSockets;
using System.Threading;

public class ClientReply : WsPingClient
{
    private async void Start()
    {
        _ws = new ClientWebSocket();

        var uri = new Uri("ws://localhost:8080");
        await _ws.ConnectAsync(uri, CancellationToken.None);
        Debug.Log("Client 2 connected");

        string reply = await RecieveText();
        Debug.Log(reply);
    }
}
