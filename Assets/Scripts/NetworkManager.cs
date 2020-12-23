using System;
using UnityEngine;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static Action OnConectedToServerEvent;

    public override void OnEnable()
    {
        base.OnEnable();
        Menu.OnConectivitySelectedEvent += Connect;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        Menu.OnConectivitySelectedEvent -= Connect;
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
        OnConectedToServerEvent?.Invoke();
    }
}