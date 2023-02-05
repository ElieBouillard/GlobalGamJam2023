using UnityEngine;
using UnityEngine.UI;

public class GameEndPanel : Singleton<GameEndPanel>
{
    [SerializeField] private GameObject _victoryPanel = null;
    [SerializeField] private GameObject _defeatPanel = null;
    [SerializeField] private Button[] _leaveButtons = null;

    public bool AnyPanelOpen => _victoryPanel.activeSelf || _defeatPanel.activeSelf;
    
    public void OnGameEnd(bool isVictory)
    {
        _victoryPanel.SetActive(isVictory);
        _defeatPanel.SetActive(!isVictory);

        CameraController.ToggleCursor(true);
    }

    public void OnLeaveButtonClicked()
    {
        if (!NetworkManager.Exists())
        {
            Debug.LogWarning($"Cannot leave current room since {nameof(NetworkManager)} does not exist!", gameObject);
            return;
        }

        NetworkManager.Instance.Leave();
    }

    private void Start()
    {
        _victoryPanel.SetActive(false);
        _defeatPanel.SetActive(false);

        foreach (Button button in _leaveButtons)
            button.onClick.AddListener(OnLeaveButtonClicked);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (Button button in _leaveButtons)
            button.onClick.RemoveListener(OnLeaveButtonClicked);
    }
}
