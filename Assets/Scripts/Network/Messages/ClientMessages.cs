using RiptideNetworking;
using UnityEngine;

public class ClientMessages : MonoBehaviour
{
    internal enum MessagesId : ushort
    {
        ClientConnected = 1,
        StartGame,
        Ready,
        Movements,
        Animations,
        Attack,
        Death,
        EnemyDeath,
    }
    
    #region Send
    public void SendClientConnected(ulong steamId)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.ClientConnected);
        message.AddULong(steamId);
        NetworkManager.Instance.Client.Send(message);
    }
    
    public void SendStartGame()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.StartGame);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendReady()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Ready);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendMovements(Vector3 pos, float y)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Movements);
        message.AddVector3(pos);
        message.AddFloat(y);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendAnimations(Vector3 input)
    {
        Message message = Message.Create(MessageSendMode.unreliable, MessagesId.Animations);
        message.AddVector3(input);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendAttack()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Attack);
        NetworkManager.Instance.Client.Send(message);
        //ToDo Send Attack
    }

    public void SendDeath()
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.Death);
        NetworkManager.Instance.Client.Send(message);
    }

    public void SendEnemyDeath(int id)
    {
        Message message = Message.Create(MessageSendMode.reliable, MessagesId.EnemyDeath);
        message.AddInt(id);
        NetworkManager.Instance.Client.Send(message);
    }
    #endregion

    #region Received
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerConnectedToLobby)]
    private static void OnClientConnectedToLobby(Message message)
    {
        LobbyManager.Instance.AddPlayerToLobby(message.GetUShort(), message.GetULong());
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.PlayerDisconnected)]
    private static void OnClientDisconnected(Message message)
    {
        ushort id = message.GetUShort();
        
        switch (NetworkManager.Instance.GameState)
        {
            case GameState.Lobby:
                LobbyManager.Instance.RemovePlayerFromLobby(id);
                break;
            
            case GameState.Gameplay:
                GameManager.Instance.RemovePlayerFromGame(id);
                break;
        }
    } 
    
    [MessageHandler((ushort) ServerMessages.MessagesId.StartGame)]
    private static void OnServerStartGame(Message message)
    {
        NetworkManager.Instance.OnServerStartGame();
    }

    [MessageHandler((ushort) ServerMessages.MessagesId.InitializeGameplay)]
    private static void OnServerInitializeClient(Message message)
    {
        GameManager.Instance.SpawnPlayers();
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.Movements)]
    private static void OnServerMovementsClient(Message message)
    {
        ((PlayerGameIdentity)NetworkManager.Instance.Players[message.GetUShort()]).MovementReceiver.SetMovements(message.GetVector3(), message.GetFloat());
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.Animations)]
    private static void OnServerAnimationsClient(Message message)
    {
        ((PlayerGameIdentity)NetworkManager.Instance.Players[message.GetUShort()]).MovementReceiver.SetAnimations(message.GetVector3());
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.Death)]
    private static void OnServerDeathClient(Message message)
    {
        //tODO Retarget Enemies
        ((PlayerGameIdentity)NetworkManager.Instance.Players[message.GetUShort()]).MovementReceiver.SetDeathAnim(true);
    }
    
    [MessageHandler((ushort)ServerMessages.MessagesId.SpawnEnemies)]
    private static void OnServerSpawnEnemies(Message message)
    {
        EnemySpawnersSystem spawnersSystem = EnemySpawnersSystem.Instance;
        
        int enemiesCount = message.GetInt();

        for (int i = 0; i < enemiesCount; i++)
        {
            spawnersSystem.Spawn(message.GetInt(), message.GetInt(), message.GetInt(), message.GetUShort());
        }
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.EnemyDeath)]
    private static void OnServerEnemyDeath(Message message)
    {
        GameManager.Instance.EnemySpawners.GetEnemy(message.GetInt()).Death(message.GetUShort());
    }

    [MessageHandler((ushort)ServerMessages.MessagesId.GameOver)]
    private static void OnServerGameOver(Message message)
    {
        GameEndPanel.Instance.OnGameEnd(message.GetBool());
    }
    #endregion
}
