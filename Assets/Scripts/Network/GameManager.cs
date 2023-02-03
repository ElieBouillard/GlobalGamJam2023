using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _localPlayerPrefab;
    [SerializeField] private GameObject _otherPlayerPrefab;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        NetworkManager.Instance.ClientMessages.SendReady();
    }

    public void SpawnPlayers()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        PlayerIdentity[] playersTemp = networkManager.Players.Values.ToArray();

        foreach (var player in playersTemp)
        {
            AddPlayerInGame(player.GetId, player.GetSteamId);
        }
    }
    
    public void AddPlayerInGame(ushort playerId, ulong steamId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        GameObject playerObject = playerId == networkManager.Client.Id ? _localPlayerPrefab : _otherPlayerPrefab;

        Transform spawnPoint = null;
        
        for (int i = 0; i < networkManager.Players.Values.ToArray().Length; i++)
        {
            if (networkManager.Players.Values.ToArray()[i].GetId == playerId)
            {
                spawnPoint = _spawnPoints[i];
                break;
            }
        }

        GameObject playerTemp = Instantiate(playerObject, spawnPoint.position ,spawnPoint.rotation);
        PlayerGameIdentity playerIdentityTemp = playerTemp.GetComponent<PlayerGameIdentity>();

        if(networkManager.UseSteam) playerIdentityTemp.Initialize(playerId, steamId);
        else playerIdentityTemp.Initialize(playerId, $"Player : {playerId}");

        if(playerId == networkManager.Client.Id) networkManager.LocalPlayer = playerIdentityTemp;

        networkManager.Players[playerId] = playerIdentityTemp;
    }

    public void RemovePlayerFromGame(ushort playerId)
    {
        NetworkManager networkManager = NetworkManager.Instance;

        Destroy(networkManager.Players[playerId].gameObject);
        networkManager.Players.Remove(playerId);
    }
    
    public void ClearPlayerInGame()
    {
        NetworkManager networkManager = NetworkManager.Instance;

        networkManager.LocalPlayer = null;
        
        foreach (var player in networkManager.Players)
        {
            Destroy(player.Value.gameObject);
        }
            
        networkManager.Players.Clear();
    }
}
