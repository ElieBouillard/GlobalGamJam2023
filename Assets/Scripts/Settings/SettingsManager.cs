using UnityEngine;

public class SettingsManager : Singleton<SettingsManager>
{
    public float MouseSensitivity { get; private set; } = 1f;
    
    public static void SetMouseSensitivity(float sliderValue)
    {
        Instance.MouseSensitivity = Remap(Mathf.Clamp01(sliderValue), 0f, 1f, 0.1f, 2f);
        
        PlayerPrefs.SetFloat("MouseSensitivity", Instance.MouseSensitivity);
        PlayerPrefs.Save();
    }

    public static float Remap(float x, float r1Min, float r1Max, float r2Min, float r2Max)
    {
        return r2Min + (x - r1Min) * (r2Max - r2Min) / (r1Max - r1Min);
    }
    
    protected override void Awake()
    {
        base.Awake();

        if (PlayerPrefs.HasKey("MouseSensitivity"))
            Instance.MouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity");

        DebugConsole.OverrideCommand(new Command<float>("mouse_sensitivity", "Sets mouse sensitivity", SetMouseSensitivity));
    }
}