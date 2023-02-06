using UnityEngine;

public class MouseSensitivitySlider : MonoBehaviour
{
    private UnityEngine.UI.Slider _slider;
    
    private void OnSliderValueChanged(float value)
    {
        SettingsManager.SetMouseSensitivity(value);
    }
    
    private void Awake()
    {
        _slider = this.GetComponent<UnityEngine.UI.Slider>();
        _slider.onValueChanged.AddListener(OnSliderValueChanged);
        _slider.value = (SettingsManager.Instance.MouseSensitivity - 0.1f) / (2f - 0.1f);
    }

    private void OnDestroy()
    {
        _slider.onValueChanged.RemoveListener(OnSliderValueChanged);
    }
}
