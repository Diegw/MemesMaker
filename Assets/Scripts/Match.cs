using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;

public class Match : SerializedMonoBehaviour, IPunInstantiateMagicCallback
{
    public static Match Instance => _instance;
    public static Action<List<Meme>> OnSpritesReadyToShow;

    private static Match _instance = null;
    [SerializeField] private Dictionary<int, Data> _matchData = new Dictionary<int, Data>();
    private PhotonView _photonView = null;

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;

        _photonView = info.photonView;
        if(_photonView == null)
        {
            Debug.LogError("There isnt PhotonView on Match Prefab");
            return;
        }

        foreach (NetworkPlayer player in NetworkManager.Instance.Players.List)
        {
            _matchData.Add(player.ID, new Data());
        }
    }

    private void Start()
    {
        List<int> availableSpritesID = new List<int>();
        foreach (int memeSpriteID in GameManager.Instance.MemesSprites.Keys)
        {
            availableSpritesID.Add(memeSpriteID);
        }

        foreach (var playerData in _matchData)
        {
            int[] spritesID = new int[GameManager.Instance.ImagesOptionsPerPlayer];
            for (int i = 0; i < spritesID.Length; i++)
            {
                int randomID = availableSpritesID[UnityEngine.Random.Range(0, availableSpritesID.Count)];
                availableSpritesID.Remove(randomID);
                spritesID[i] = randomID;
            }
            _photonView.RPC(nameof(SpritesToShow_RPC), RpcTarget.All, playerData.Key, spritesID);
        }
    }

    [PunRPC] private void SpritesToShow_RPC(int playerID, int[] spritesID)
    {
        if(!_matchData.ContainsKey(playerID))
        {
            return;
        }
        if(PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            return;
        }

        List<Meme> memesSelected = new List<Meme>();
        for (int i = 0; i < spritesID.Length; i++)
        {
            Sprite memeSprite = GameManager.Instance.MemesSprites[spritesID[i]];
            Meme meme = new Meme(spritesID[i], memeSprite);
            memesSelected.Add(meme);
            _matchData[playerID].Memes.Add(meme);
        }
        OnSpritesReadyToShow?.Invoke(memesSelected);
    }

    [Serializable] private class Data
    {
        public List<Meme> Memes => _memes;

        [SerializeField] private List<Meme> _memes = new List<Meme>();
    }
}