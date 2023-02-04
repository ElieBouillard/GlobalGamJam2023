using DG.Tweening;
using UnityEngine;

public class LocalPlayerDamageFlash : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController = null;
    [SerializeField] private CanvasGroup _canvasGroup = null;
    [SerializeField] private float _flashAlpha = 0.5f;
    [SerializeField] private float _flashDuration = 0.5f;
    [SerializeField] private Ease _flashEase = Ease.OutCirc;

    private Tween _flashTween;

    private void OnHealthChanged(int previousHealth, int currentHealth)
    {
        if (currentHealth >= previousHealth)
            return;

        _flashTween?.Kill();

        _canvasGroup.alpha = _flashAlpha;
        _flashTween = _canvasGroup.DOFade(0f, _flashDuration).SetEase(_flashEase);
        _flashTween.Play();
    }

    private void Start()
    {
        if (_playerController == null)
        {
            Debug.LogError($"{nameof(LocalPlayerHealth)} reference is missing!");
            return;
        }

        _playerController.HealthChanged += OnHealthChanged;
        _canvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (_playerController == null)
            return;

        _playerController.HealthChanged -= OnHealthChanged;
    }
}