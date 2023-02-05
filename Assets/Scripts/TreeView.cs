using DG.Tweening;
using UnityEngine;

public class TreeView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Material[] _treeMaterials = null;

    [Header("General")]
    [SerializeField, Range(0f, 1f)] private float _healthPercentage = 1f;
    [SerializeField, Range(0f, 1f)] private float _pulsedHealthPercentage = 1f;
    [SerializeField] private AnimationCurve _progressionCurve = null;

    [Header("Pulse")]
    [SerializeField] private float _pulseValue = 0.15f;
    [SerializeField] private float _pulseDuration = 0.5f;

    [Header("Shader data")]
    [SerializeField] private Color _emissiveColorStart = Color.yellow;
    [SerializeField] private Color _emissiveColorEnd = Color.red;
    [Space(10)]
    [SerializeField, Min(0f)] private float _emissiveIntensityStart = 1f;
    [SerializeField, Min(0f)] private float _emissiveIntensityEnd = 5f;
    [Space(10)]
    [SerializeField, Range(0f, 0.5f)] private float _thresholdStart = 0.4f;
    [SerializeField, Range(0f, 0.5f)] private float _thresholdEnd = 0f;
    [Space(10)]
    [SerializeField, Range(0f, 0.1f)] private float _noiseSpeedStart = 0.01f;
    [SerializeField, Range(0f, 0.1f)] private float _noiseSpeedEnd = 0.1f;

    private Tween _pulseTween;

    public void SetHealthPercentage(float percentage)
    {
        _healthPercentage = Mathf.Clamp01(percentage);

        _pulseTween?.Kill();
        _pulseTween = DOTween.To(() => Mathf.Clamp01(percentage - _pulseValue), UpdateShaderData, percentage, _pulseDuration);
    }

    private void UpdateShaderData(float t)
    {
        _pulsedHealthPercentage = t;
        t = 1f - _progressionCurve.Evaluate(t);

        Color color = Color.Lerp(_emissiveColorStart, _emissiveColorEnd, t);
        float emissiveIntensity = Mathf.Lerp(_emissiveIntensityStart, _emissiveIntensityEnd, t);
        float threshold = Mathf.Lerp(_thresholdStart, _thresholdEnd, t);
        float noise = Mathf.Lerp(_noiseSpeedStart, _noiseSpeedEnd, t);

        foreach (Material material in _treeMaterials)
        {
            material.SetColor("_EmissivColor", color * emissiveIntensity);
            material.SetFloat("_EmissivPower", emissiveIntensity);
            material.SetFloat("_Threshold", threshold);
            material.SetFloat("_SpeedNoise", noise);
        }
    }

    private void Start()
    {
        DebugConsole.OverrideCommand(new Command<float>("tree_health_percentage", "Sets tree health percentage.", SetHealthPercentage));
    }

    private void OnValidate()
    {
        if (_treeMaterials == null || _treeMaterials.Length == 0)
            return;

        UpdateShaderData(_healthPercentage);
    }
}
