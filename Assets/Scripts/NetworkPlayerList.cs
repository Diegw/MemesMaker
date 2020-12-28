using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

[Serializable] public class NetworkPlayerList
{
    [SerializeField] private List<NetworkPlayer> _players = new List<NetworkPlayer>();

    public void AddPlayer(Player newPlayer)
    {
        _players.Add(new NetworkPlayer(newPlayer));
    }
}