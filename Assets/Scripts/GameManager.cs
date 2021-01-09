using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Sirenix.OdinInspector;

public class GameManager : SerializedMonoBehaviour
{
    public static GameManager Instance => _instance;
    public bool IsOffline { get => _isOffline; set => _isOffline = value; }
    public Dictionary<int, Sprite> MemesSprites => _memesSprites;
    public int ImagesOptionsPerPlayer => _imagesOptionsPerPlayer;
    private static GameManager _instance = null;
    private const string MEMES_IMAGES_PATH = "Images";
    [SerializeField] private bool _isOffline = false;
    [OnValueChanged( nameof(CheckIfPair) )][SerializeField] private int _imagesOptionsPerPlayer = 4;
    [SerializeField] private Dictionary<int, Sprite> _memesSprites = new Dictionary<int, Sprite>();

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
        List<Sprite> sprites = new List<Sprite>();
        sprites = Resources.LoadAll<Sprite>(MEMES_IMAGES_PATH).ToList();
        for (int i = 0; i < sprites.Count; i++)
        {
            MemesSprites.Add(i, sprites[i]);            
        }
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
        IsOffline = conectivityState;
    }

    private void CheckIfPair()
    {
        if(_imagesOptionsPerPlayer % 2 == 0)
        {
            return;
        }
        _imagesOptionsPerPlayer = Mathf.Max(_imagesOptionsPerPlayer+1, _imagesOptionsPerPlayer);
    }
}