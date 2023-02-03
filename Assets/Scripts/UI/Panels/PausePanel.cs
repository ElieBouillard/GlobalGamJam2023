using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : Panel
{
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _disconnectionButton;

    protected override void AssignButtonsReference()
    {
        _resumeButton.onClick.AddListener(ResumeGame);
        _disconnectionButton.onClick.AddListener(Disconnect);
    }

    private void ResumeGame()
    {
        PanelManager.Instance.EnablePanel(PanelType.Pause);
    }

    private void Disconnect()
    {
        NetworkManager.Instance.Leave();
    }
}
