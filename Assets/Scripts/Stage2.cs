using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Sirenix.OdinInspector;

public class Stage2 : Stage
{
    [TabGroup("DEBUG")][SerializeField] private bool _isChoiceCompleted = false;
    [TabGroup("DEBUG")][SerializeField] private bool _isStatementCompleted = false;
    [TabGroup("DEBUG")][SerializeField] protected Dictionary<Button, Meme> _memesToSelect = new Dictionary<Button, Meme>();
    [TabGroup("REFERENCES")][SerializeField] private GameObject _choice = null;
    [TabGroup("REFERENCES")][SerializeField] private GameObject _statement = null;
    [TabGroup("REFERENCES")][SerializeField] private Image _statementImage = null;
    [TabGroup("REFERENCES")][SerializeField] private TMP_InputField _statementInputField = null;
    [TabGroup("REFERENCES")][SerializeField] private List<Button> _buttons = new List<Button>();
    private int _statementSpriteID = -1;

    protected override void CheckReferences()
    {
        foreach (Button button in _buttons)
        {
            if(button == null) Debug.LogError("Some Button Reference couldnt be found. Check MatchUI");
        }
        if(_statement == null) Debug.LogError("Statement Reference isnt assign. Check MatchUI");
        if(_statementImage == null) Debug.LogError("StatementImage Reference isnt assign. Check MatchUI");
        if(_statementInputField == null) Debug.LogError("StatementInputField Reference isnt assign. Check MatchUI");
    }

    protected override void Initialize()
    {
        foreach (Button button in _buttons)
        {
            _memesToSelect.Add(button, new Meme(-1, null));
        }
    }

    protected override void Activate()
    {
        base.Activate();
        Match.OnMemesStage2 += ShowMemesStage2;
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].onClick.AddListener(Choice);
        }
        _statementInputField.onValueChanged.AddListener(SetTextToUpperCase);
    }

    protected override void Desactivate()
    {
        base.Desactivate();
        Match.OnMemesStage2 -= ShowMemesStage2;
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].onClick.RemoveListener(Choice);
        }
        _statementInputField.onValueChanged.RemoveListener(SetTextToUpperCase);
    }

    protected override void StartStage()
    {
        ToggleChoices(true, false);
    }

    protected override void EndStage()
    {
        ToggleChoices(false, false);
        CheckStage2();
        IsStageCompleted();
    }

    private void CheckStage2()
    {
        if (_isChoiceCompleted == false)
        {
            _isChoiceCompleted = true;
            int randomButtonIndex = UnityEngine.Random.Range(0, _buttons.Count);
            Button randomButton = _buttons[randomButtonIndex];
            int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            int memePlayerID = _memesToSelect[randomButton].PlayerID;
            int spriteID = _memesToSelect[randomButton].SpriteID;
            _statementSpriteID = spriteID;
            photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, memePlayerID, spriteID);
        }
        Statement();
    }

    private void ShowMemesStage2(List<Meme> memes)
    {
        foreach (Button button in _buttons)
        {
            int buttonIndex = _buttons.IndexOf(button);
            button.image.sprite = memes[buttonIndex].Sprite;
            _memesToSelect[button] = memes[buttonIndex];
        }
        ObjectSelect.Instance.SelectObject(_buttons[0].gameObject);
    }

    private void Choice()
    {
        ToggleChoices(false, true);
        _isChoiceCompleted = true;
        Button buttonPressed = ObjectSelect.Instance.ObjectSelected.GetComponent<Button>();
        _statementImage.sprite = _memesToSelect[buttonPressed].Sprite;
        int playerID = PhotonNetwork.LocalPlayer.ActorNumber;
        int memePlayerID = _memesToSelect[buttonPressed].PlayerID;
        int spriteID = _memesToSelect[buttonPressed].SpriteID;
        _statementSpriteID = spriteID;
        photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, playerID, memePlayerID, spriteID);
        ObjectSelect.Instance.SelectObject(_statementInputField.gameObject);
    }

    [PunRPC] private void Choice_RPC(int playerID, int memePlayerID, int spriteID)
    {
        OnVsDisplayHide?.Invoke();
        Sprite memeSprite = GameManager.Instance.MemesSprites[spriteID];
        Meme memeChoosen = new Meme(spriteID, memeSprite, memePlayerID);
        OnMemeChoosen?.Invoke(playerID, memeChoosen);
    }

    private void SetTextToUpperCase(string currentStatementText)
    {
        _statementInputField.text = currentStatementText.ToUpper();
    }

    private void Statement()
    {
        _isStatementCompleted = true;
        string statement = string.Empty;
        if(_statementInputField.text.Length <= 1)
        {
            statement = "NO STATEMENT";
        }
        else
        {
            statement = _statementInputField.text;
        }
        photonView.RPC(nameof(Statement_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, _statementSpriteID, statement);
    }

    [PunRPC] private void Statement_RPC(int playerID, int spriteID, string statementText)
    {
        OnStatementSubmit?.Invoke(playerID, spriteID, statementText);
    }

    private void ToggleChoices(bool choiceState, bool statementState)
    {
        _choice.SetActive(choiceState);
        _statement.SetActive(statementState);
    }

    private bool IsStageCompleted()
    {
        if(_isChoiceCompleted && _isStatementCompleted)
        {
            stageCompleted = true;
        }
        else
        {
            stageCompleted = false;
        }
        return stageCompleted;
    }
}