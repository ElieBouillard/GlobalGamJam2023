using DG.Tweening;
using UnityEngine;

public class LocalPlayerDamageFlash : Singleton<LocalPlayerDamageFlash>
{
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private Material _material = null;
    [SerializeField] private float _startingFalloff = 0.1f;
    [SerializeField] private float _targetFalloff = 10f;
    [SerializeField] private float _flashDuration = 0.5f;
    [SerializeField] private Ease _flashEase = Ease.OutCirc;

    private Canvas _canvas;
    private Tween _flashTween;

    protected override void Awake()
    {
        base.Awake();

        _canvas = GetComponent<Canvas>();
    }

    public void Setup(Camera camera, PlayerController controller)
    {
        _canvas.worldCamera = camera;
        
        _playerController = controller;
        
        _playerController.HealthChanged += OnHealthChanged;
        SetFalloff(_targetFalloff);
    }
    
    private void OnHealthChanged(int previousHealth, int currentHealth)
    {
        if (currentHealth >= previousHealth)
            return;

        PlayFlashAnimation();
    }

    private void PlayFlashAnimation()
    {
        _flashTween?.Kill();

        SetFalloff(_startingFalloff);
        _flashTween = DOTween.To(() => _startingFalloff, SetFalloff, _targetFalloff, _flashDuration).SetEase(_flashEase);
        _flashTween.Play();
    }

    private void SetFalloff(float falloff)
    {
        _material.SetFloat("_Falloff", falloff);
    }

    // private void Start()
    // {
    //     if (_playerController == null)
    //     {
    //         Debug.LogError($"{nameof(LocalPlayerDamageFlash)} reference is missing!");
    //         return;
    //     }
    //
    //     _playerController.HealthChanged += OnHealthChanged;
    //     SetFalloff(_targetFalloff);
    //
    //     DebugConsole.OverrideCommand(new Command("dmg_flash", "Plays damage flash feedback.", () =>
    //     {
    //         FreezeFrameManager.FreezeFrame(2, 0.15f);
    //         FindObjectOfType<CameraController>().SetTrauma(1f);
    //         PlayFlashAnimation();
    //     }));
    // }

    private void OnDestroy()
    {
        if (_playerController == null)
            return;

        _playerController.HealthChanged -= OnHealthChanged;
    }
}