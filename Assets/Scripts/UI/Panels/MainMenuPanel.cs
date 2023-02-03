using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : Panel
{
    [Header("References")]
    [SerializeField] private Button _createGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        _joinGameButton.gameObject.SetActive(!NetworkManager.Instance.UseSteam);
    }
    
    protected override void AssignButtonsReference()
    {
        _createGameButton.onClick.AddListener(OnClickCreateGame);
        _joinGameButton.onClick.AddListener(OnClickJoinGame);
        _optionsButton.onClick.AddListener(OnClickOptions);
        _quitButton.onClick.AddListener(OnClickQuit);
    }

    private void OnClickCreateGame()
    {
        NetworkManager.Instance.StartHost();
    }

    private void OnClickJoinGame()
    {
        NetworkManager.Instance.JoinLobby();
    }

    private void OnClickOptions()
    {
        PanelManager.Instance.EnablePanel(PanelType.Options);
    }

    private void OnClickQuit()
    {
        Application.Quit();
    }
}