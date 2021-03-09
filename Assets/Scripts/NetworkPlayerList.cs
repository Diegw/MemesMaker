using System;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

[Serializable] public class NetworkPlayerList
{
    public List<NetworkPlayer> List => _players;

    [SerializeField] private List<NetworkPlayer> _players = new List<NetworkPlayer>();

    public void AddPlayer(Player newPlayer)
    {
        _players.Add(new NetworkPlayer(newPlayer));
    }

    public NetworkPlayer GetNetworkPlayer(int playerID)
    {
        foreach (NetworkPlayer networkPlayer in _players)
        {
            if(networkPlayer.ID == playerID)
            {
                return networkPlayer;
            }
        }
        return null;
    }
}