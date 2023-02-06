using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel : Panel
{
    [Header("References")]
    [SerializeField] private Button _createGameButton;
    [SerializeField] private Button _joinGameButton;
    [SerializeField] private Button _optionsButton;
    [SerializeField] private Button _quitButton;
    [SerializeField] private RSLib.Framework.GUI.EnterSubmitInputField _submitIPInputField;

    private void Start()
    {
        _joinGameButton.gameObject.SetActive(!NetworkManager.Instance.UseSteam);
        CameraController.ToggleCursor(true);
    }
    
    protected override void AssignButtonsReference()
    {
        _createGameButton.onClick.AddListener(OnClickCreateGame);
        _joinGameButton.onClick.AddListener(OnClickJoinGame);
        _optionsButton.onClick.AddListener(OnClickOptions);
        _quitButton.onClick.AddListener(OnClickQuit);
        _submitIPInputField.onSubmit.AddListener(_ => this.OnClickJoinGame());
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
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}