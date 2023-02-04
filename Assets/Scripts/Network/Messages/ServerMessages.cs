using System.Security.Cryptography.X509Certificates;
using RiptideNetworking;
using UnityEngine;

public class ServerMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        PlayerConnectedToLobby = 1,
        PlayerDisconnected,
        StartGame,
        InitializeGameplay,
        Movements,
    }

    #region Send
    public static void SendPlayerConnectedToLobby(ushort newPlayerId, ulong steamId)
    {
        foreach (var player in NetworkManager.Instance.Players)
        {
            Message message1 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
            message1.AddUShort(player.Value.GetId);
            message1.AddULong(player.Value.GetSteamId);
            NetworkManager.Instance.Server.Send(message1, newPlayerId);
        }
        
        Message message2 = Message.Create(MessageSendMode.reliable, MessagesId.PlayerConnectedToLobby);
        message2.AddUShort(newPlayerId);
        message2.AddULong(steamId);
        NetworkManager.Instance.Server.SendToAll(message2);
    }
    
    public void SendPlayerDisconnected(ushort playerId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.PlayerDisconnected);
        message.AddUShort(playerId);
        NetworkManager.Instance.Server.SendToAll(message, playerId);
    }

    private static void SendHostStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        NetworkManager.Instance.Server.SendToAll(message);
    }

    private static void SendInitializeClient(ushort id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.InitializeGameplay);
        NetworkManager.Instance.Server.Send(message, id);
    }

    private static void SendClientMovements(ushort id, Vector3 pos)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Movements);
        message.AddUShort(id);
        message.AddVector3(pos);
        NetworkManager.Instance.Server.SendToAll(message, id);
    }
    #endregion

    #region Received
    [MessageHandler((ushort) ClientMessages.MessagesId.ClientConnected)]
    private static void OnClientConnected(ushort id, Message message)
    {
        SendPlayerConnectedToLobby(id, message.GetULong());
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.StartGame)]
    private static void OnClientStartGame(ushort id, Message message)
    {
        if(id != 1) return;
        SendHostStartGame();
    }

    [MessageHandler((ushort) ClientMessages.MessagesId.Ready)]
    private static void OnClientReady(ushort id, Message message)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        networkManager.PlayersReady++;

        if (networkManager.PlayersReady == networkManager.Players.Count)
        {
            foreach (var playerId in networkManager.Players.Keys)
            {
                SendInitializeClient(playerId);
            }
        }
    }

    [MessageHandler((ushort)ClientMessages.MessagesId.Movements)]
    private static void OnClientMovements(ushort id, Message message)
    {
        SendClientMovements(id, message.GetVector3());
    }
    #endregion
}