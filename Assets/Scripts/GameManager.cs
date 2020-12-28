using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    public GameSettings GameSettings => _gameSettings;

    private static GameManager _instance = null;
    [SerializeField] private GameSettings _gameSettings = null;

    public void Construct()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        NetworkManager.OnConectedToServerEvent += SetOfflineMode;
    }

    private void OnDisable()
    {
        NetworkManager.OnConectedToServerEvent -= SetOfflineMode;
    }

    private void SetOfflineMode(bool conectivityState)
    {
        _gameSettings.IsOffline = conectivityState;
    }
}