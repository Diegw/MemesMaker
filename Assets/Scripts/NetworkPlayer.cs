using System;
using UnityEngine;
using Photon.Realtime;

[Serializable] public class NetworkPlayer
{
    public NetworkPlayer(Player newPlayer)
    {
        _ID = newPlayer.ActorNumber;
        _name = newPlayer.NickName;
        _simulation = newPlayer.UserId;
    }

    [SerializeField] private int _ID = -1;
    [SerializeField] private string _name = string.Empty;
    [SerializeField] private string _simulation = string.Empty;
}