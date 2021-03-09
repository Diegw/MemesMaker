using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance => _instance;
    public static Action OnSceneChanged;
    public static string MENU_SCENE => MENU;
    public static string LOBBY_SCENE => LOBBY;
    public static string GAMEPLAY_SCENE => GAMEPLAY;

    private const string MENU = "02.Menu";
    private const string LOBBY = "03.Lobby";
    private const string GAMEPLAY = "04.Gameplay";
    private static ScenesManager _instance = null;
    [SerializeField] private string _currentSceneName = string.Empty;

    public void Construct()
    {
        if(_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);

        SetCurrentScene(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SetCurrentScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SetCurrentScene;
    }

    private void SetCurrentScene(Scene scene, LoadSceneMode sceneMode)
    {
        _currentSceneName = scene.name;
        OnSceneChanged?.Invoke();
    }

    public void LoadScene(string sceneToLoad)
    {
        PhotonNetwork.LoadLevel(sceneToLoad);
    }
    
    private void OnDestroy()
    {
        OnDisable();
    }
}