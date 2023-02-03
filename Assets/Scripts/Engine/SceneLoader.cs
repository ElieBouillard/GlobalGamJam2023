#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Scenes[] _scenesToLoad;
    
    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoad;

        for (int i = 0; i < _scenesToLoad.Length; i++)
        {
            SceneManager.LoadScene((int) _scenesToLoad[i], i == 0 ? LoadSceneMode.Single : LoadSceneMode.Additive);
        }
    }
    
    private void OnSceneLoad(Scene scene, LoadSceneMode loadSceneMode)
    {
        
    }

#if UNITY_EDITOR
    [MenuItem("SceneLoader/Play Build")]
    private static void PlayBuild()
    {
        EditorSceneManager.SaveOpenScenes();

        EditorSceneManager.OpenScene("Assets/Scenes/LaunchScene.unity", OpenSceneMode.Single);
        
        EditorApplication.EnterPlaymode();
    }

    [MenuItem("SceneLoader/Load LaunchScene")]
    private static void LoadLaunchScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/LaunchScene.unity", OpenSceneMode.Single);
    }
    
    [MenuItem("SceneLoader/Load StartMenuScene")]
    private static void LoadStartMenuScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/StartMenuScene.unity", OpenSceneMode.Single);
    }
    
    [MenuItem("SceneLoader/Load GameplayScene")]
    private static void LoadGameplayScene()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/GameplayScene.unity", OpenSceneMode.Single);
    }
#endif
}

public enum Scenes
{
    Launch = 0,
    StartMenu,
    Gameplay,
}