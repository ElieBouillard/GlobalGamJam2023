using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using Random = UnityEngine.Random;

public class EnemySpawnersSystem : Singleton<EnemySpawnersSystem>
{
    [Header("Parameters")]
    [SerializeField] private float _spawnTime = 5f;
    [SerializeField] private int _enemyCount = 5;
    [SerializeField] private float _difficultyIncreaseStage = 20f;
    [SerializeField] private float _difficultyPercentageAdder = 25f;
    [SerializeField] private float _spawnTimeIncreaseValue = 0.2f;
    [SerializeField] private int _enemyToWin = 100;
    
    [Space(20)] [Header("References")]
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private GameObject _enemyTreeTargetPrefab;
    [SerializeField] private GameObject _enemyPlayerTargetPrefab;

    public List<EnemyIdentity> Enemies { private set; get; } = new List<EnemyIdentity>();
    
    private NetworkManager _networkManager;
    
    private float _spawnClock = -1;

    private int _enemyIdCount = 0;

    private int _enemySpawnedCounter = 0;

    private int _waveCounter = 1;

    private int _enemyDeadPerWave = 0;

    private int _enemyDead;
    
    protected override void Awake()
    {
        base.Awake();
        
        _networkManager = NetworkManager.Instance;
    }

    public void InitializeSpawn()
    {
        _spawnClock = 0;
        
        EnemyRemaining.Instance.SetText(_enemyToWin * _networkManager.Players.Count);
    }
    
    private void Update()
    {
        if (!_networkManager.Server.IsRunning) return;
     
        if (_spawnClock > 0)
        {
            _spawnClock -= Time.unscaledDeltaTime;
        }
        else if(_spawnClock != -1) 
        {
            _spawnClock = _spawnTime;
            ChooseSpawn();
        }
        
        if (_enemyDeadPerWave >= _difficultyIncreaseStage * _networkManager.Players.Count)
        {
            _waveCounter++;
            _enemyDeadPerWave = 0;
            _difficultyIncreaseStage += _difficultyIncreaseStage * _difficultyPercentageAdder / 100;
            if(_waveCounter % 2 != 0)
            {
                _enemyCount += 1;
                Debug.Log($"AUGMENTATION DU NOMBRE D'ENNEMIS PAR SPAWN : {_enemyCount}");
            }
            _spawnTime += _spawnTimeIncreaseValue;
            //Debug.Log($"DIFFICULTE AUGMENTE : {_difficultyIncreaseStage}");
        }
    }

    private void ChooseSpawn()
    {
        List<EnemySpawnData> enemiesSpawnData = new List<EnemySpawnData>();

        for (int i = 0; i < _enemyCount * _networkManager.Players.Count; i++)
        {
            int randomEnemyType = Random.Range(0, 2);
            
            int randomSpawnPoint = Random.Range(0, _spawnPoints.Length);

            ushort bestUshort = 0;
            
            if (randomEnemyType == 1)
            {
                float bestDist = Mathf.Infinity;
                
                foreach (var player in _networkManager.Players.Values)
                {

                    float dist = (player.transform.position - _spawnPoints[randomSpawnPoint].position).magnitude;
                    
                    if (dist < bestDist)
                    {
                        bestUshort = player.GetId;
                        bestDist = dist;
                    }
                }
            }
            
            if(_enemyIdCount < _enemyToWin * _networkManager.Players.Count)
            {
                Spawn(_enemyIdCount, randomEnemyType, randomSpawnPoint, bestUshort);

                EnemySpawnData enemySpawnData = new EnemySpawnData();
                enemySpawnData.EnemyId = _enemyIdCount;
                enemySpawnData.EnemyType = randomEnemyType;
                enemySpawnData.SpawnId = randomSpawnPoint;
                enemySpawnData.PlayerId = bestUshort;
            
                enemiesSpawnData.Add(enemySpawnData);
            
                _enemyIdCount++;
            }
        }

        _networkManager.ServerMessages.SendSpawnEnemies(enemiesSpawnData);
    }

    public void Spawn(int enemyId, int enemyType, int spawnId, ushort playerId)
    {
        GameObject enemyPrefab = enemyType == 0 ? _enemyTreeTargetPrefab : _enemyPlayerTargetPrefab;

        GameObject enemyInstance = Instantiate(enemyPrefab, _spawnPoints[spawnId].position, _spawnPoints[spawnId].rotation);

        if (enemyType == 0)
        {
            enemyInstance.GetComponent<EnemyTreeTargetIdentity>().Initialize(enemyId, TreeBehaviour.Position);
        }
        else if (enemyType == 1)
        {
            enemyInstance.GetComponent<EnemyPlayerTargetIdentity>().Initialize(enemyId, _networkManager.Players[playerId].transform);
        }
        
        Enemies.Add(enemyInstance.GetComponent<EnemyIdentity>());

        // _enemySpawnedCounter++;
        //Debug.Log($"ENNEMI SPAWNED : {_enemySpawnedCounter}");
    }

    public void RemoveEnemy(EnemyIdentity enemy)
    {
        Enemies.Remove(enemy);
        _enemyDeadPerWave++;
        _enemyDead++;
        
        EnemyRemaining.Instance.SetText(_enemyToWin * _networkManager.Players.Count - _enemyDead);
        
        if (NetworkManager.Instance.Server.IsRunning)
        {
            if (_enemyIdCount >= _enemyToWin * _networkManager.Players.Count)
            {
                if (Enemies.Count == 0)
                {
                    ServerMessages.SendGameOver(true);
                }
            }
        }
    }

    public EnemyIdentity GetEnemy(int id)
    {
        foreach (var enemy in Enemies)
        {
            if (enemy.Id == id)
            {
                return enemy;
            }
        }

        return null;
    }
}

public struct EnemySpawnData
{
    public int EnemyId;
    public int EnemyType;
    public int SpawnId;
    public ushort PlayerId;
}