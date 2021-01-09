using System;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager Instance => _instance;
    public static Action<bool> OnConectedToServerEvent;
    public static Action OnRoomCreatedEvent;
    public static Action<string> OnRoomJoinedEvent;
    public NetworkPlayerList Players => _players;

    private static NetworkManager _instance = null;
    [SerializeField] private string _roomName = string.Empty;
    [SerializeField] private NetworkPlayerList _players = new NetworkPlayerList();
    private char[] _roomCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789".ToCharArray();

    public void Construct()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public override void OnEnable()
    {
        base.OnEnable();
        Menu.OnConectivitySelectedEvent += Connect;
        Lobby.OnCreatingRoomEvent += TryToCreateRoom;
        Lobby.OnJoiningRoomEvent += TryToJoinRoom;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Menu.OnConectivitySelectedEvent -= Connect;
        Lobby.OnCreatingRoomEvent -= TryToCreateRoom;
        Lobby.OnJoiningRoomEvent -= TryToJoinRoom;
    }

    public void Connect(bool conectivityState)
    {
        PhotonNetwork.GameVersion = "0.1";
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.MinimalTimeScaleToDispatchInFixedUpdate = 0f;
        PhotonNetwork.ConnectUsingSettings(PhotonNetwork.PhotonServerSettings.AppSettings, conectivityState);
    }

    public override void OnConnectedToMaster()
    {
        OnConectedToServerEvent?.Invoke(PhotonNetwork.OfflineMode);
        ScenesManager.Instance.LoadScene(ScenesManager.LOBBY_SCENE);
    }

    private void TryToCreateRoom()
    {
        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        CreateRoom();
    }

    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions() { IsOpen = true, IsVisible = true, MaxPlayers = 8 };
        _roomName = RandomRoomName();
        PhotonNetwork.CreateRoom(_roomName, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        _players.AddPlayer(PhotonNetwork.MasterClient);
        OnRoomCreatedEvent?.Invoke();
    }

    private void TryToJoinRoom(string roomToJoin)
    {
        _roomName = roomToJoin;
        PhotonNetwork.JoinRoom(_roomName);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        OnRoomJoinedEvent?.Invoke(_roomName);
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if(_players.List.Contains(_players.GetNetworkPlayer(player.ActorNumber)))
            {
                continue;
            }
            _players.AddPlayer(player);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        _players.AddPlayer(newPlayer);
    }

    private string RandomRoomName()
    {
        _roomName = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            _roomName += _roomCharacters[UnityEngine.Random.Range(0, _roomCharacters.Length)];
        }
        return _roomName;
    }
}