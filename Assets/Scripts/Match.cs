using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;
using MEC;

public class Match : SerializedMonoBehaviour, IPunInstantiateMagicCallback
{
    public static Match Instance => _instance;
    public static Action<int> OnStageTimeStarted;
    public static Action<float> OnStageTimeUpdated;
    public static Action<int> OnStageTimeEnded;
    public static Action<List<Meme>> OnMemesStage1;
    public static Action<List<Meme>> OnMemesStage2;
    public static Action<Meme> OnMemesStage3;
    public static Action<List<Meme>> OnTournamentStart;
    public static Action<Meme> OnMemeWinner;

    [TabGroup("DEBUG")][SerializeField] private Dictionary<int, PlayerData> _playersData = new Dictionary<int, PlayerData>();
    [TabGroup("DEBUG")][SerializeField] private int _currentStage = 0;
    [TabGroup("REFERENCES")][SerializeField] private Stage[] _stagesPrefabs = null;
    [TabGroup("REFERENCES")][SerializeField] private Tournament _tournamentPrefab = null;
    [TabGroup("REFERENCES")][SerializeField] private Showcase _showcasePrefab = null;
    private static Match _instance = null;
    private PhotonView _photonView = null;
    private CoroutineHandle _matchCoroutine = default;

    [Serializable] private class StageData
    {
        public Stage Stage => _stage;
        public bool HasStarted { get => _hasStarted; set => _hasStarted = value; }
        public bool HasEnded { get => _hasEnded; set => _hasEnded = value; }

        [SerializeField] private Stage _stage = null;
        [SerializeField] private bool _hasStarted = false;
        [SerializeField] private bool _hasEnded = false;
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        _photonView = info.photonView;
        if (_photonView == null)
        {
            Debug.LogError("There isnt PhotonView on Match Prefab");
            return;
        }
        Singleton();
        if(!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        InstantiateStages();
        InstantiateTournament();
        InstantiateShowcase();
        Activate();
        Initialize();
    }

    private void Singleton()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;
    }

    private void Initialize()
    {
        foreach (NetworkPlayer player in NetworkManager.Instance.Players.List)
        {
            _playersData.Add(player.ID, new PlayerData());
        }
        _matchCoroutine = Timing.RunCoroutine( Match_Coroutine().CancelWith(gameObject) );
    }

#region Match
    private IEnumerator<float> Match_Coroutine()
    {
        foreach (float stageDuration in GameManager.Instance.StagesDuration)
        {
            float elapsedTime = 4f;
            while(elapsedTime >= 0)
            {
                _photonView.RPC(nameof(UpdateStage_RPC), RpcTarget.All, elapsedTime);
                yield return Timing.WaitForSeconds(1f);
                elapsedTime--;
            }
            yield return Timing.WaitForOneFrame;

            _currentStage++;
            PrepareStages();
            yield return Timing.WaitForOneFrame;
            _photonView.RPC(nameof(StartStage_RPC), RpcTarget.All, _currentStage, stageDuration);

            elapsedTime = stageDuration;
            while(elapsedTime > 0)
            {
                yield return Timing.WaitForSeconds(1f);
                elapsedTime--;
                _photonView.RPC(nameof(UpdateStage_RPC), RpcTarget.All, elapsedTime);
            }
            _photonView.RPC(nameof(EndStage_RPC), RpcTarget.All, _currentStage, 0f);
            yield return Timing.WaitForOneFrame;
        }
        
        //mientras los memes no esten completos no comenzar
        yield return Timing.WaitForSeconds(1f);
        PrepareTournament();
    }

    [PunRPC] private void StartStage_RPC(int currentStage, float elapsedTime)
    {
        OnStageTimeStarted?.Invoke(currentStage);
        OnStageTimeUpdated?.Invoke(elapsedTime);
    }

    [PunRPC] private void UpdateStage_RPC(float elapsedTime)
    {
        OnStageTimeUpdated?.Invoke(elapsedTime);
    }

    [PunRPC] private void EndStage_RPC(int currentStage, float elapsedTime)
    {
        _currentStage = currentStage;
        OnStageTimeEnded?.Invoke(_currentStage);
        OnStageTimeUpdated?.Invoke(elapsedTime);
    }

    private void PrepareStages()
    {
        switch(_currentStage)
        {
            case 1:
            {
                PrepareStage1();
                break;
            }
            case 2:
            {
                PrepareStage2();
                break;
            }
            case 3:
            {
                PrepareStage3();
                break;
            }
        }
    }
#endregion

    private void Activate()
    {
        Stage1.OnMemeSelected += AddMemeSelected;
        Stage2.OnMemeChoosen += AddMemeChoosen;
        Stage2.OnStatementSubmit += AddStatement;
        Stage3.OnRemateSubmit += AddRemate;
        Tournament.OnTournamentEnd += SearchMemeWinner;
    }

    private void Desactivate()
    {
        Stage1.OnMemeSelected -= AddMemeSelected;
        Stage2.OnMemeChoosen -= AddMemeChoosen;
        Stage2.OnStatementSubmit -= AddStatement;
        Stage3.OnRemateSubmit -= AddRemate;
        Tournament.OnTournamentEnd -= SearchMemeWinner;
    }

#region Instantiation
    private void InstantiateStages()
    {
        for (int i = 0; i < _stagesPrefabs.Length; i++)
        {
            if(_stagesPrefabs[i] == null)
            {
                continue;
            }
            object[] instantiationData = new object[1] {i};
            PhotonNetwork.InstantiateRoomObject(_stagesPrefabs[i].gameObject.name, Vector3.zero, Quaternion.identity, 0, instantiationData);
        }
    }

    private void InstantiateTournament()
    {
        if(_tournamentPrefab == null)
        {
            return;
        }
        PhotonNetwork.InstantiateRoomObject(_tournamentPrefab.gameObject.name, Vector3.zero, Quaternion.identity);
    }

    private void InstantiateShowcase()
    {
        if(_showcasePrefab == null)
        {
            return;
        }
        PhotonNetwork.InstantiateRoomObject(_showcasePrefab.gameObject.name, Vector3.zero, Quaternion.identity);
    }
#endregion


#region Stage1
    private void PrepareStage1()
    {
        List<int> availableSpritesID = new List<int>();
        foreach (int memeSpriteID in GameManager.Instance.MemesSprites.Keys)
        {
            availableSpritesID.Add(memeSpriteID);
        }

        foreach (var playerData in _playersData)
        {
            int[] spritesID = new int[4];
            for (int i = 0; i < spritesID.Length; i++)
            {
                int randomID = availableSpritesID[UnityEngine.Random.Range(0, availableSpritesID.Count)];
                availableSpritesID.Remove(randomID);
                spritesID[i] = randomID;
            }
            _photonView.RPC(nameof(Stage1_RPC), RpcTarget.All, playerData.Key, spritesID);
        }
    }

    [PunRPC] private void Stage1_RPC(int playerID, int[] spritesID)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            return;
        }

        List<Meme> memesSelected = new List<Meme>();
        foreach (int spriteID in spritesID)
        {
            Sprite memeSprite = GameManager.Instance.MemesSprites[spriteID];
            Meme meme = new Meme(spriteID, memeSprite);
            memesSelected.Add(meme);
        }
        OnMemesStage1?.Invoke(memesSelected);
    }

    private void AddMemeSelected(int playerID, Meme meme)
    {
        if (!_playersData.ContainsKey(playerID))
        {
            return;
        }
        _playersData[playerID].MemesSelected.Add(meme);
    }
#endregion


#region Stage2
    private void PrepareStage2()
    {
        List<Meme> availableMemes = new List<Meme>();
        foreach (var playerData in _playersData)
        {
            foreach (Meme meme in playerData.Value.MemesSelected)
            {
                Meme newMeme = new Meme(meme.SpriteID, meme.Sprite, meme.PlayerID);
                availableMemes.Add(newMeme);
            }
        }
        foreach (var playerData in _playersData)
        {
            List<int> spritesID = new List<int>();
            List<int> playersID = new List<int>();
            if(_playersData.Count <= 1)
            {
                foreach (Meme meme in playerData.Value.MemesSelected)
                {
                    spritesID.Add(meme.SpriteID);
                    playersID.Add(meme.PlayerID);
                }
            }
            else
            {
                foreach (Meme meme in playerData.Value.MemesSelected)
                {
                    Meme randomMeme = null;
                    while(randomMeme == null || randomMeme.PlayerID == playerData.Key)
                    {
                        randomMeme = availableMemes[UnityEngine.Random.Range(0, availableMemes.Count)];
                    }
                    spritesID.Add(randomMeme.SpriteID);
                    playersID.Add(randomMeme.PlayerID);
                    availableMemes.Remove(randomMeme);
                }
            }
            _photonView.RPC(nameof(Stage2_RPC), RpcTarget.All, playerData.Key, spritesID.ToArray(), playersID.ToArray());
        }
    }

    [PunRPC] private void Stage2_RPC(int playerID, int[] spritesID, int[] playersID)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            return;
        }
        
        List<Meme> memesToChoose = new List<Meme>();
        for (int i = 0; i < spritesID.Length; i++)
        {
            Meme newMeme = new Meme(spritesID[i], GameManager.Instance.MemesSprites[spritesID[i]], playersID[i]);
            memesToChoose.Add(newMeme);
        }
        OnMemesStage2?.Invoke(memesToChoose);
    }

    private void AddMemeChoosen(int playerID, Meme meme)
    {
        if(!_playersData.ContainsKey(meme.PlayerID))
        {
            return;
        }
        _playersData[playerID].MemeChoosen = meme;
        _playersData[playerID].MemeChoosen.PlayerID = meme.PlayerID;
    }

    private void AddStatement(int playerID, int spriteID, string statementText)
    {
        int player = GetPlayerKeyByMemeChoosen(spriteID);
        if(_playersData.ContainsKey(player) == false)
        {
            return;
        }
        _playersData[player].MemeChoosen.StatementID = playerID;
        _playersData[player].MemeChoosen.Statement = statementText;
    }
#endregion


#region Stage3
    private void PrepareStage3()
    {
        List<Meme> availableMemes = new List<Meme>();
        foreach (PlayerData player in _playersData.Values)
        {
            Meme meme = player.MemeChoosen;
            Meme playerMeme = new Meme(meme.SpriteID, meme.Sprite, meme.PlayerID, meme.StatementID, meme.Statement);
            availableMemes.Add(playerMeme);
        }
        if(availableMemes.Count <= 0)
        {
            return;
        }
        foreach (var playerData in _playersData)
        {
            Meme randomMeme = null;
            if(_playersData.Keys.Count <= 1)
            {
                randomMeme = playerData.Value.MemeChoosen;
            }
            else if(_playersData.Keys.Count == 2)
            {
                while(randomMeme == null || randomMeme.StatementID == playerData.Key)
                {
                    randomMeme = availableMemes[UnityEngine.Random.Range(0, availableMemes.Count)];
                }
                availableMemes.Remove(randomMeme);
            }
            else
            {
                while(randomMeme == null || randomMeme.PlayerID == playerData.Key || randomMeme.StatementID == playerData.Key)
                {
                    randomMeme = availableMemes[UnityEngine.Random.Range(0, availableMemes.Count)];
                }
                availableMemes.Remove(randomMeme);
            }
            _photonView.RPC(nameof(Stage3_RPC), RpcTarget.All, playerData.Key, randomMeme.SpriteID, randomMeme.Statement);
        }
    }

    [PunRPC] private void Stage3_RPC(int playerID, int spriteID, string statement)
    {
        if(PhotonNetwork.LocalPlayer.ActorNumber != playerID)
        {
            return;
        }
        
        Meme memeToShow = new Meme(spriteID, GameManager.Instance.MemesSprites[spriteID], -1, -1, statement);
        OnMemesStage3?.Invoke(memeToShow);
    }

    private void AddRemate(int playerID, int spriteID, string remateText)
    {
        int player = GetPlayerKeyByMemeChoosen(spriteID);
        if(_playersData.ContainsKey(player) == false)
        {
            return;
        }
        _playersData[player].MemeChoosen.RemateID = playerID;
        _playersData[player].MemeChoosen.Remate = remateText;
    }
#endregion


#region Tournament
    private void PrepareTournament()
    {
        List<Meme> memesChoosen = new List<Meme>();
        foreach (PlayerData playerData in _playersData.Values)
        {
            Meme meme = playerData.MemeChoosen;
            Meme memeChoosen = new Meme(meme.SpriteID, meme.Sprite, meme.PlayerID, meme.StatementID, meme.Statement, meme.RemateID, meme.Remate);
            memesChoosen.Add(memeChoosen);
        }
        OnTournamentStart?.Invoke(memesChoosen);
    }

    private void SearchMemeWinner(int winnerSpriteID)
    {
        foreach (PlayerData playerData in _playersData.Values)
        {
            if(playerData.MemeChoosen.SpriteID == winnerSpriteID)
            {
                Meme meme = playerData.MemeChoosen;
                Meme memeWinner = new Meme(meme.SpriteID, meme.Sprite, meme.PlayerID, meme.StatementID, meme.Statement, meme.RemateID, meme.Remate);
                OnMemeWinner?.Invoke(memeWinner);
                break;
            }
        }
    }
#endregion

    public void SetParent(MonoBehaviour element)
    {
        element.transform.SetParent(transform);
    }

    public int GetPlayerKeyByMemeChoosen(int spriteID)
    {
        foreach (var playerData in _playersData)
        {
            if(playerData.Value.MemeChoosen.SpriteID == spriteID)
            {
                return playerData.Key;
            }
        }
        return -1;
    }

    private void OnDestroy()
    {
        Desactivate();
    }

    private class PlayerData
    {
        public List<Meme> MemesSelected => _memesSelected;
        public Meme MemeChoosen { get => _memeChoosen; set => _memeChoosen = value; }

        [SerializeField] private List<Meme> _memesSelected = new List<Meme>();
        [SerializeField] private Meme _memeChoosen = new Meme(-1, null);
    }
}