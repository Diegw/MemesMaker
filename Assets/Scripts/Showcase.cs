using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using Sirenix.OdinInspector;

public class Showcase : MonoBehaviour, IPunInstantiateMagicCallback
{
    [TabGroup("REFERENCES")][SerializeField] private GameObject _memeWinner = null;
    [TabGroup("REFERENCES")][SerializeField] private Image _memeImage = null;
    [TabGroup("REFERENCES")][SerializeField] private TextMeshProUGUI _statementDisplay = null;
    [TabGroup("REFERENCES")][SerializeField] private TextMeshProUGUI _remateDisplay = null;
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
        _memeWinner.SetActive(false);
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
        if (_memeWinner == null)
        {
            Debug.LogError("Showcase Reference is not assign on Tournament Prefab");
            return false;
        }
        return true;
    }

    private void OnEnable()
    {
        Match.OnMemeWinner += ShowMemeWinner;
    }

    private void OnDisable()
    {
        Match.OnMemeWinner -= ShowMemeWinner;
    }

    private void ShowMemeWinner(Meme meme)
    {
        _photonView.RPC(nameof(ShowMemeWinner_RPC), RpcTarget.All, meme.SpriteID, meme.Statement, meme.Remate, meme.PlayerID, meme.StatementID, meme.RemateID);
    }

    [PunRPC] private void ShowMemeWinner_RPC(int spriteID, string statement, string remate, int playerID, int statementID, int remateID)
    {
        _memeImage.sprite = GameManager.Instance.MemesSprites[spriteID];
        _statementDisplay.text = statement;
        _remateDisplay.text = remate;
        //encontrar nombre de cada player y mostrarlo
        _memeWinner.SetActive(true);
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}