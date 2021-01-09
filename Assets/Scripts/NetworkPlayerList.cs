using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

[Serializable] public class NetworkPlayerList
{
    public List<NetworkPlayer> List => _list;

    [SerializeField] private List<NetworkPlayer> _list = new List<NetworkPlayer>();

    public void AddPlayer(Player newPlayer)
    {
        _list.Add(new NetworkPlayer(newPlayer));
    }

    public NetworkPlayer GetNetworkPlayer(int playerID)
    {
        foreach (NetworkPlayer networkPlayer in _list)
        {
            if(networkPlayer.ID == playerID)
            {
                return networkPlayer;
            }
        }
        return null;
    }
}