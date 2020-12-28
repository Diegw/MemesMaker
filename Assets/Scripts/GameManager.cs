using UnityEngine;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => _instance;
    public GameSettings GameSettings => _gameSettings;

    private static GameManager _instance = null;
    private const string MEMES_IMAGES_PATH = "Images";
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

    private void Start()
    {
        _gameSettings.MemesSprites = Resources.LoadAll<Sprite>(MEMES_IMAGES_PATH).ToList();
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