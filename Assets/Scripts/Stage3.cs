using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Sirenix.OdinInspector;

public class Stage3 : Stage
{
    [TabGroup("DEBUG")][SerializeField] private bool _isRemateCompleted = false;
    [TabGroup("REFERENCES")][SerializeField] private GameObject _remate = null;
    [TabGroup("REFERENCES")][SerializeField] private Image _remateImage = null;
    [TabGroup("REFERENCES")][SerializeField] private TextMeshProUGUI _statementDisplay = null;
    [TabGroup("REFERENCES")][SerializeField] private TMP_InputField _remateInputField = null;
    private int _remateSpriteID = -1;

    protected override void CheckReferences()
    {
        if(_remate == null) Debug.LogError("Remate Reference is null. Check Stage3");
        if(_remateInputField == null) Debug.LogError("RemateInputField Reference is null. Check Stage3");
    }

    protected override void Initialize(){}

    protected override void Activate()
    {
        base.Activate();
        Match.OnMemesStage3 += ShowMemeStage3;
        _remateInputField.onValueChanged.AddListener(SetTextToUpperCase);
    }

    protected override void Desactivate()
    {
        base.Desactivate();
        Match.OnMemesStage3 -= ShowMemeStage3;
        _remateInputField.onValueChanged.RemoveListener(SetTextToUpperCase);
    }
    
    protected override void StartStage()
    {
        ToggleChoices(true);
    }

    protected override void EndStage()
    {
        ToggleChoices(false);
        Remate();
        IsStageCompleted();
    }

    private void ShowMemeStage3(Meme meme)
    {
        _remateImage.sprite = meme.Sprite;
        _remateSpriteID = meme.SpriteID;
        _statementDisplay.text = meme.Statement;
        ObjectSelect.Instance.SelectObject(_remateInputField.gameObject);
    }

    private void SetTextToUpperCase(string currentStatementText)
    {
        _remateInputField.text = currentStatementText.ToUpper();
    }

    private void Remate()
    {
        _isRemateCompleted = true;
        string remate = string.Empty;
        if(_remateInputField.text.Length <= 1)
        {
            remate = "NO REMATE";
        }
        else
        {
            remate = _remateInputField.text;
        }
        photonView.RPC(nameof(Remate_RPC), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, _remateSpriteID, remate);
    }

    [PunRPC] private void Remate_RPC(int playerID, int spriteID, string statementText)
    {
        OnRemateSubmit?.Invoke(playerID, spriteID, statementText);
    }

    private void ToggleChoices(bool remateState)
    {
        _remate.SetActive(remateState);
    }

    private bool IsStageCompleted()
    {
        stageCompleted = _isRemateCompleted;
        return stageCompleted;
    }
}