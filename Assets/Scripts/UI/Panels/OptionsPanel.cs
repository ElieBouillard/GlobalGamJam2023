using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsPanel : Panel
{
    [Header("References")]
    [SerializeField] private Button _returnButton;

    protected override void AssignButtonsReference()
    {
        _returnButton.onClick.AddListener(OnCLickReturn);
    }

    protected void OnCLickReturn()
    {
        PanelManager.Instance.EnablePanel(PanelType.MainMenu);
    }
}
