using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using RiptideNetworking;
using RiptideNetworking.Transports.SteamTransport;
using RiptideNetworking.Utils;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using SteamClient = RiptideNetworking.Transports.SteamTransport.SteamClient;

public class NetworkManager : Singleton<NetworkManager>
{
    #region UnityInspector
    [Header("NetworkSettings")] [Space(10)]
    public bool UseSteam;
    [SerializeField] private bool _useLocalHost;
    [SerializeField] private ushort _port = 7777;
    public ushort MaxPlayer = 4;
    #endregion
    
    #region Fields
    public Server Server { private set; get; }
    public Client Client { private set; get; }
    [HideInInspector] public GameState GameState { private set; get; } = GameState.OffLine;
    public Dictionary<ushort, PlayerIdentity> Players { private set; get; }  = new Dictionary<ushort, PlayerIdentity>();
    public PlayerIdentity  LocalPlayer;
    public ClientMessages ClientMessages { private set; get; }
    public ServerMessages ServerMessages { private set; get; }
    #endregion
    
    protected override void Awake()
    {
        base.Awake();

        ClientMessages = gameObject.AddComponent<ClientMessages>();
        ServerMessages = gameObject.AddComponent<ServerMessages>();

        if (UseSteam)
        {
            gameObject.AddComponent<SteamManager>();
            gameObject.AddComponent<SteamLobbyManager>();
        }
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        SteamServer steamServer = new SteamServer();
        InitializeClient(steamServer);
        InitializeServer(steamServer);
    }

    private void FixedUpdate()
    {
        Client.Tick();
        
        if(Server.IsRunning) Server.Tick();
    }

    private void OnApplicationQuit()
    {
        switch (GameState)
        {
            case GameState.OffLine:
                break;
            case GameState.Lobby:
                Leave();
                break;
            case GameState.Gameplay:
                Leave();
                break;
        }
    }

    private void InitializeClient(SteamServer steamServer)
    {
        Client = UseSteam ? new Client(new SteamClient(steamServer)) : new Client();

        Client.Connected += ClientOnConnected;
        Client.Disconnected += ClientOnDisconnected;
        Client.ConnectionFailed += ClientOnConnectionFailed;
        Client.ClientConnected += ClientOnPlayerJoin;
        Client.ClientDisconnected += ClientOnPlayerLeft;
    }

    private void InitializeServer(SteamServer steamServer)
    {
        Server = UseSteam ? new Server(steamServer) : new Server();

        Server.ClientConnected += ServerOnClientConnected;
        Server.ClientDisconnected += ServerOnClientDisconnected;
    }
    
    #region ClientCallbacks
    private void ClientOnConnected(object sender, EventArgs e)
    {
        GameState = GameState.Lobby;

        ClientMessages.SendClientConnected(UseSteam ? (ulong)SteamUser.GetSteamID() : new ulong());

        PanelManager.Instance.EnablePanel(PanelType.Lobby);
    }

    private void ClientOnDisconnected(object sender, EventArgs e)
    {
        switch (GameState)
        {
            case GameState.Lobby:
                LobbyManager.Instance.ClearLobby();
                PanelManager.Instance.EnablePanel(PanelType.MainMenu);
                break;
            case GameState.Gameplay:
                GameManager.Instance.ClearPlayerInGame();
                SceneManager.LoadScene("StartMenuScene", LoadSceneMode.Single);
                break;
        }
        
        GameState = GameState.OffLine;
        
        if(!UseSteam) return;
        SteamLobbyManager.Instance.LeaveLobby();
    }
    
    private void ClientOnConnectionFailed(object sender, EventArgs e)
    {
        
    }
    
    private void ClientOnPlayerJoin(object sender, ClientConnectedEventArgs e)
    {
        
    }
    
    private void ClientOnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        
    }
    #endregion

    #region ServerCallbacks
    private void ServerOnClientConnected(object sender, ServerClientConnectedEventArgs e)
    {
        switch (GameState)
        {
            case GameState.Gameplay:
                Server.DisconnectClient(e.Client.Id);
                break;
        }
    }
    
    private void ServerOnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
    {
        ServerMessages.SendPlayerDisconnected(e.Id);
    }
    #endregion

    #region Client
    public void StartHost()
    {
        if (UseSteam)
        {
            SteamLobbyManager.Instance.CreateLobby(); ;
        }
        else
        {
            Server.Start(_port, MaxPlayer);
            Client.Connect( _useLocalHost ? $"127.0.0.1:{_port}" : $"{GetLocalIPAddress()}:{_port}");
            if (!_useLocalHost) IpAddress.Instance.SetIpAddress(GetLocalIPAddress());
        }
    }
    
    public void JoinLobby()
    {
        if (UseSteam) return;
        Client.Connect( _useLocalHost ? $"127.0.0.1:{_port}" : $"{IpAddress.Instance.GetIpAddress()}:{_port}");
    }
    
    public void Leave()
    {
        Client.Disconnect();
        ClientOnDisconnected(new object(), EventArgs.Empty);
        Server.Stop();
    }

    public void StartGame()
    {
        if (LocalPlayer.GetId != 1) return;
        
        ClientMessages.SendStartGame();
    }

    public void OnServerStartGame()
    {
        GameState = GameState.Gameplay;
        SceneManager.LoadScene("GameplayScene", LoadSceneMode.Single);
    }
    #endregion

    #region Server
    
    #endregion
    
    private string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new Exception("No network adapters with an IPv4 address in the system!");
    }
}

public enum GameState
{
    OffLine = 1,
    Lobby,
    Gameplay,
}