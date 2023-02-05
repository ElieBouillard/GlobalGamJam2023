using System;
using UnityEngine;

public class LocalPlayerHealth : Singleton<LocalPlayerHealth>
{
    [SerializeField] private Canvas _canvas = null;
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private HeartUI[] _hearts = null;

    public void SetPlayerController(PlayerController playerController)
    {
        _playerController = playerController;
        
        _playerController.HealthChanged += OnHealthChanged;

        foreach (HeartUI heart in _hearts)
            heart.Toggle(true, force: true);
    }
    
    private void OnHealthChanged(int previousHealth, int currentHealth)
    {
        for (int i = 0; i < _hearts.Length; ++i)
            _hearts[i].Toggle(currentHealth > i);

        _canvas.enabled = currentHealth > 0;
    }

    // private void Start()
    // {
    //     if (_playerController == null)
    //     {
    //         Debug.LogError($"{nameof(LocalPlayerHealth)} reference is missing!");
    //         return;
    //     }
    //
    //     _playerController.HealthChanged += OnHealthChanged;
    //
    //     foreach (HeartUI heart in _hearts)
    //         heart.Toggle(true, force: true);
    // }

    private void OnDestroy()
    {
        if (_playerController == null)
            return;

        _playerController.HealthChanged -= OnHealthChanged;
    }
}
