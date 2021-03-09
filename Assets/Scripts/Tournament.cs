using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using TMPro;
using Photon.Pun;
using MEC;
using UnityEngine.EventSystems;

public class Tournament : SerializedMonoBehaviour, IPunInstantiateMagicCallback
{
    public static Action<float> OnUpdateBattleTimer;
    public static Action<int> OnTournamentEnd;

    [TabGroup("DEBUG")][SerializeField] private List<Meme> _totalParticipants = new List<Meme>();
    [TabGroup("DEBUG")][SerializeField] private List<Meme> _participants = new List<Meme>();
    [TabGroup("DEBUG")][SerializeField] private List<Meme> _winners = new List<Meme>();
    [TabGroup("REFERENCES")][SerializeField] private GameObject _battle = null;
    [TabGroup("REFERENCES")][SerializeField] private Dictionary<int, MemeUI> _memesUI = new Dictionary<int, MemeUI>();
    private CoroutineHandle _battlesCoroutine = default;
    private PhotonView _photonView = null;

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Match.Instance.SetParent(this);
        _photonView = info.photonView;
        
        SetRectTransform();
        if(CheckReferences() == false)
        {
            return;
        }
        _battle.SetActive(false);
    }

    private void SetRectTransform()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    private bool CheckReferences()
    {
        if (_photonView == null)
        {
            Debug.LogError("There isnt PhotonView on Tournament Prefab");
            return false;
        }
        if (_battle == null)
        {
            Debug.LogError("Battle Reference is not assign on Tournament Prefab");
            return false;
        }
        if (_memesUI == null)
        {
            Debug.LogError("MemesUI is null. Check Tournament Prefab");
            return false;
        }
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            if (memeUI.Button == null)
            {
                Debug.LogError("Some Button Reference is not assign on Tournament Prefab");
                return false;
            }
            if (memeUI.Statement == null)
            {
                Debug.LogError("Some Statement Display Reference is not assign on Tournament Prefab");
                return false;
            }
            if (memeUI.Remate == null)
            {
                Debug.LogError("Some Remate Display Reference is not assign on Tournament Prefab");
                return false;
            }
            if (memeUI.Score == null)
            {
                Debug.LogError("Some Score Display Reference is not assign on Tournament Prefab");
                return false;
            }
        }
        return true;
    }

    private void OnEnable()
    {
        Match.OnTournamentStart += StartTournament;
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            memeUI.Button.onClick.AddListener(Choice);
        }
    }

    private void OnDisable()
    {
        Match.OnTournamentStart -= StartTournament;
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            memeUI.Button.onClick.RemoveListener(Choice);
        }
    }

    private void StartTournament(List<Meme> memes)
    {
        _totalParticipants = new List<Meme>(memes);
        _participants = new List<Meme>(memes);
        CheckWinners();
    }

    private void ResetParticipants()
    {
        _participants = new List<Meme>(_winners);
        _participants.Clear();
        CheckWinners();
    }

    private void CheckWinners()
    {
        if (_winners.Count == 1)
        {
            OnTournamentEnd?.Invoke(_winners[0].SpriteID);
            _battle.SetActive(false);
        }
        else
        {
            Timing.KillCoroutines(_battlesCoroutine);
            _battlesCoroutine = Timing.RunCoroutine(Battles_Coroutine());
        }
    }

    private IEnumerator<float> Battles_Coroutine()
    {
        _winners.Clear();

        int battlesCount = Mathf.FloorToInt(_participants.Count/2);
        for (int battleIndex = 0; battleIndex < battlesCount; battleIndex++)
        {
            yield return Timing.WaitForOneFrame;
            float elapsedTime = 5f;
            while(elapsedTime >= 0)
            {
                yield return Timing.WaitForOneFrame;
                _photonView.RPC(nameof(UpdateBattleTimer_RPC), RpcTarget.All, elapsedTime);
                yield return Timing.WaitForSeconds(1f);
                elapsedTime--;
            }

            yield return Timing.WaitForOneFrame;
            int[] memesIndexes = new int[2];
            string[] memesStatements = new string[2];
            string[] memesRemates = new string[2];
            for (int i = 0; i < 2; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, _participants.Count);
                Meme randomMeme = _participants[randomIndex];
                memesIndexes[i] = randomMeme.SpriteID;
                memesStatements[i] = randomMeme.Statement;
                memesRemates[i] = randomMeme.Remate;
                _participants.Remove(randomMeme);
            }
            ResetVotes();
            _photonView.RPC(nameof(Battle_RPC), RpcTarget.All, memesIndexes, memesStatements, memesRemates);

            yield return Timing.WaitForOneFrame;
            elapsedTime = GameManager.Instance.BattleDuration;
            while(elapsedTime >= 0)
            {
                _photonView.RPC(nameof(UpdateBattleTimer_RPC), RpcTarget.All, elapsedTime);
                yield return Timing.WaitForSeconds(1f);
                elapsedTime--;
            }
            CheckBattleWinner();
        }

        if(_participants.Count == 1)
        {
            Meme meme = _participants[0];
            _winners.Add(new Meme(meme.SpriteID, meme.Sprite));
        }
        ResetParticipants();
    }

    [PunRPC] private void Battle_RPC(int[] indexes, string[] statements, string[] remates)
    {
        if(_memesUI.Count != 2)
        {
            return;
        }
        if(indexes.Length != 2 || statements.Length != 2 || remates.Length != 2)
        {
            return;
        }
        for (int i = 0; i < 2; i++)
        {
            _memesUI[i].SpriteID = indexes[i];
            _memesUI[i].Button.image.sprite = GameManager.Instance.MemesSprites[indexes[i]];
            _memesUI[i].Statement.text = statements[i];
            _memesUI[i].Remate.text = remates[i];
        }
        _battle.SetActive(true);
        EnableButtons();
        ObjectSelect.Instance.SelectObject(_memesUI[0].Button.gameObject);
    }

    private void EnableButtons()
    {
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            memeUI.Button.interactable = true;
        }
    }

    [PunRPC] private void UpdateBattleTimer_RPC(float elapsedTime)
    {
        OnUpdateBattleTimer?.Invoke(elapsedTime);
    }

    private void CheckBattleWinner()
    {
        int winnerSpriteID = -1;
        if(_memesUI[0].Votes > _memesUI[1].Votes)
        {
            winnerSpriteID = _memesUI[0].SpriteID;
        }
        else if(_memesUI[1].Votes > _memesUI[0].Votes)
        {
            winnerSpriteID = _memesUI[1].SpriteID;
        }
        else
        {
            int randomKey = UnityEngine.Random.Range(0, 2);
            winnerSpriteID = _memesUI[randomKey].SpriteID;
        }

        if(winnerSpriteID == -1)
        {
            Debug.LogError("Winner SpriteID is -1");
            return;
        }
        Meme winnerMeme = _totalParticipants.Find(meme => meme.SpriteID == winnerSpriteID);
        _winners.Add(new Meme(winnerMeme.SpriteID, winnerMeme.Sprite));
        _photonView.RPC(nameof(ShowBattleWinner_RPC), RpcTarget.All, _memesUI[0].Votes, _memesUI[1].Votes, winnerSpriteID);
    }

    [PunRPC] private void ShowBattleWinner_RPC(int leftVotes, int rightVotes, int winnerSpriteID)
    {
        _memesUI[0].Score.text = "+"+leftVotes.ToString();
        _memesUI[1].Score.text = "+"+rightVotes.ToString();
    }

    private void Choice()
    {
        int spriteID = GetSpriteID(EventSystem.current.currentSelectedGameObject);
        if(spriteID == -1)
        {
            return;
        }
        DisableButtons();
        int key = GetMemeUI(spriteID);
        if(key != -1)
        {
            _memesUI[key].Score.text = "VOTED";
        }
        _photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, spriteID);
    }

    private void DisableButtons()
    {
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            memeUI.Button.interactable = false;
        }
    }

    [PunRPC] private void Choice_RPC(int spriteID)
    {
        int key = GetMemeUI(spriteID);
        if(key == -1)
        {
            return;
        }
        _memesUI[key].Votes++;
    }

    private int GetSpriteID(GameObject gameObject)
    {
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            if(memeUI.Button.gameObject == gameObject)
            {
                return memeUI.SpriteID;
            }
        }
        return -1;
    }

    private int GetMemeUI(int spriteID)
    {
        foreach (var memeUI in _memesUI)
        {
            if(memeUI.Value.SpriteID == spriteID)
            {
                return memeUI.Key;
            }
        }
        return -1;
    }

    private void ResetVotes()
    {
        _photonView.RPC(nameof(ResetScore_RPC), RpcTarget.All);
    }

    [PunRPC] private void ResetScore_RPC()
    {
        foreach (MemeUI memeUI in _memesUI.Values)
        {
            memeUI.Score.text = "";
            memeUI.Votes = 0;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }

    [Serializable] private class MemeUI
    { 
        public int SpriteID { get => _spriteID; set => _spriteID = value; }
        public Button Button => _button;
        public TextMeshProUGUI Remate => _remateDisplay;
        public TextMeshProUGUI Statement => _statementDisplay;
        public TextMeshProUGUI Score => _score;
        public int Votes { get => _votes; set => _votes = value; }

        [SerializeField] private int _spriteID = -1;
        [SerializeField] private Button _button = null;
        [SerializeField] private TextMeshProUGUI _statementDisplay = null;
        [SerializeField] private TextMeshProUGUI _remateDisplay = null;
        [SerializeField] private TextMeshProUGUI _score = null;
        [SerializeField] private int _votes = 0;
    }
}