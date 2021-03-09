using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Photon.Pun;
using UnityEngine.UI;

public class Stage1 : Stage
{   
    [TabGroup("DEBUG")][SerializeField] private bool _isFirstChoiceCompleted = false;
    [TabGroup("DEBUG")][SerializeField] private bool _isSecondChoiceCompleted = false;
    [TabGroup("DEBUG")][SerializeField] protected Dictionary<Button, Meme> _memesToSelect = new Dictionary<Button, Meme>();
    [TabGroup("REFERENCES")][SerializeField] private GameObject _firstChoice = null;
    [TabGroup("REFERENCES")][SerializeField] private GameObject _secondChoice = null;
    [TabGroup("REFERENCES")][SerializeField] private List<Button> _buttons = new List<Button>();

    protected override void CheckReferences()
    {
        if(_firstChoice == null) Debug.LogError("FirstChoice Reference is null. Check Stage1");
        if(_secondChoice == null) Debug.LogError("SecondChoice Reference is null. Check Stage1");
        foreach (Button button in _buttons)
        {
            if(button == null) Debug.LogError("Some Button Reference couldnt be found. Check MatchUI");
        }
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
        Match.OnMemesStage1 += ShowMemesStage1;
        for (int i = 0; i < _buttons.Count; i++)
        {
            if(i < 2)
            {
                _buttons[i].onClick.AddListener(FirstChoice);
            }
            else
            {
                _buttons[i].onClick.AddListener(SecondChoice);
            }
        }
    }

    protected override void Desactivate()
    {
        base.Desactivate();
        Match.OnMemesStage1 -= ShowMemesStage1;
        for (int i = 0; i < _buttons.Count; i++)
        {
            if(i < 2)
            {
                _buttons[i].onClick.RemoveListener(FirstChoice);
            }
            else
            {
                _buttons[i].onClick.RemoveListener(SecondChoice);
            }
        }
    }

    protected override void StartStage()
    {
        ToggleChoices(true, false);
    }

    protected override void EndStage()
    {
        ToggleChoices(false, false);
        if(IsStageCompleted())
        {
            return;
        }
        CheckStage1();
    }

    private void CheckStage1()
    {
        int startIndex = 0;
        int choicesQuantity = 0;
        if (_isFirstChoiceCompleted == false && _isSecondChoiceCompleted == false)
        {
            startIndex = 0;
            choicesQuantity = 4;
        }
        else
        {
            if (_isFirstChoiceCompleted == false)
            {
                startIndex = 0;
                choicesQuantity = 2;
            }
            if (_isSecondChoiceCompleted == false)
            {
                startIndex = 2;
                choicesQuantity = 4;
            }
        }
        _isFirstChoiceCompleted = true;
        _isSecondChoiceCompleted = true;

        for (int i = startIndex; i < choicesQuantity; i++)
        {
            if (i % 2 != 0)
            {
                continue;
            }
            int randomButtonIndex = UnityEngine.Random.Range(i, i + 1);
            Button randomButton = _buttons[randomButtonIndex];
            photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, _memesToSelect[randomButton].SpriteID);
        }
    }

    private void ShowMemesStage1(List<Meme> memes)
    {
        if(memes.Count != _buttons.Count)
        {
            Debug.LogError("Sprites Length isnt equal to Images Length");
            return;
        }

        foreach (Button button in _buttons)
        {
            int buttonIndex = _buttons.IndexOf(button);
            button.image.sprite = memes[buttonIndex].Sprite;
            _memesToSelect[button] = memes[buttonIndex];
        }
        ObjectSelect.Instance.SelectObject(_buttons[0].gameObject);
    }

    private void FirstChoice()
    {
        ToggleChoices(false, true);
        _isFirstChoiceCompleted = true;
        Button buttonPressed = ObjectSelect.Instance.ObjectSelected.GetComponent<Button>();
        photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, _memesToSelect[buttonPressed].SpriteID);
        ObjectSelect.Instance.SelectObject(_buttons[2].gameObject);
    }

    private void SecondChoice()
    {
        ToggleChoices(false, false);
        _isSecondChoiceCompleted = true;
        Button buttonPressed = ObjectSelect.Instance.ObjectSelected.GetComponent<Button>();
        photonView.RPC(nameof(Choice_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, _memesToSelect[buttonPressed].SpriteID);
    }

    [PunRPC] private void Choice_RPC(int playerID, int spriteID)
    {
        Sprite sprite = GameManager.Instance.MemesSprites[spriteID];
        Meme memeSelected = new Meme(spriteID, sprite, playerID);
        OnMemeSelected?.Invoke(playerID, memeSelected);
    }

    private void ToggleChoices(bool firstChoiceState, bool secondChoiceState)
    {
        _firstChoice?.SetActive(firstChoiceState);
        _secondChoice?.SetActive(secondChoiceState);
        for (int i = 0; i < _buttons.Count; i++)
        {
            if(i < 2)
            {
                _buttons[i].interactable = firstChoiceState;
            }
            else
            {
                _buttons[i].interactable = secondChoiceState;
            }            
        }
    }

    private bool IsStageCompleted()
    {
        if(_isFirstChoiceCompleted && _isSecondChoiceCompleted)
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