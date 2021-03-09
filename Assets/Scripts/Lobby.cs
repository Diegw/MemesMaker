using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Lobby : MonoBehaviour
{
    public static Action OnCreatingRoomEvent;
    public static Action<string> OnJoiningRoomEvent;

    [SerializeField] private GameObject _local = null;
    [SerializeField] private GameObject _online = null;
    [SerializeField] private GameObject _match = null;
    [SerializeField] private TextMeshProUGUI _roomDisplay = null;

    [SerializeField] private Button _createRoomButton = null;
    [SerializeField] private Button _joinRoomButton = null;
    [SerializeField] private InputField _joinRoomInputField = null;

    [SerializeField] private Button _playButton = null;
    [SerializeField] private Button _configurationsButton = null;

    private void Awake()
    {
        CheckButtons();
        bool localState = GameManager.Instance.IsOffline == true ? true : false;
        bool onlineState = GameManager.Instance.IsOffline == true ? false : true;
        _local.SetActive(localState);
        _online.SetActive(onlineState);
        _match.SetActive(false);
    }

    private void CheckButtons()
    {
        if (_local == null) Debug.LogError("Local GameObject Reference isnt assing. Check Lobby");
        if(_online == null) Debug.LogError("Online GameObject Reference isnt assing. Check Lobby");
        if(_match == null) Debug.LogError("Match GameObject Reference isnt assing. Check Lobby");
        if (_createRoomButton == null) Debug.LogError("CreateRoomButton isnt assing. Check Lobby");
        if (_joinRoomButton == null) Debug.LogError("JoinRoomButton isnt assing. Check Lobby");
        if (_joinRoomInputField == null) Debug.LogError("JoinRoomInputField isnt assing. Check Lobby");
        if (_playButton == null) Debug.LogError("PlayButton isnt assing. Check Lobby");
        if (_configurationsButton == null) Debug.LogError("ConfigurationsButton isnt assing. Check Lobby");
    }

    private void OnEnable()
    {
        _createRoomButton.onClick.AddListener(CreatingRoomEvent);
        _joinRoomButton.onClick.AddListener(JoiningRoomEvent);
        _joinRoomInputField.onValueChanged.AddListener(SetToUpperCase);

        NetworkManager.OnRoomCreatedEvent += ShowMatchButtons;
        NetworkManager.OnRoomJoinedEvent += ShowRoomName;

        _playButton.onClick.AddListener(Play);
        _configurationsButton.onClick.AddListener(Configurations);
    }

    private void Start()
    {
        GameObject objectToSelect = _online.activeSelf == true ? _createRoomButton.gameObject : null;
        ObjectSelect.Instance.SelectObject(objectToSelect);
    }

    private void OnDisable()
    {
        _createRoomButton.onClick.RemoveListener(CreatingRoomEvent);
        _joinRoomButton.onClick.RemoveListener(JoiningRoomEvent);
        _joinRoomInputField.onValueChanged.RemoveListener(SetToUpperCase);

        NetworkManager.OnRoomCreatedEvent -= ShowMatchButtons;
        NetworkManager.OnRoomJoinedEvent -= ShowRoomName;

        _playButton.onClick.RemoveListener(Play);
        _configurationsButton.onClick.RemoveListener(Configurations);
    }

    private void CreatingRoomEvent()
    {
        OnCreatingRoomEvent?.Invoke();
    }

    private void JoiningRoomEvent()
    {
        if(_joinRoomInputField.text == string.Empty || _joinRoomInputField.text.Length != 5)
        {
            return;
        }
        string roomToJoin = _joinRoomInputField.text;
        OnJoiningRoomEvent?.Invoke(roomToJoin);
    }

    private void SetToUpperCase(string inputText)
    {
        _joinRoomInputField.text = inputText.ToUpper();
    }

    private void ShowMatchButtons()
    {
        _online.SetActive(false);
        _match.SetActive(true);
        ObjectSelect.Instance.SelectObject(_playButton.gameObject);
    }

    private void ShowRoomName(string roomName)
    {
        _roomDisplay.text = $"- ROOM -\n{roomName}";
    }

    private void Play()
    {
        ScenesManager.Instance.LoadScene(ScenesManager.GAMEPLAY_SCENE);
    }

    private void Configurations()
    {
        //Abrir configuraciones de la partida en la misma escena
    }
}