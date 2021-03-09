using System;
using UnityEngine;
using Photon.Pun;
using Sirenix.OdinInspector;

public class Stage : SerializedMonoBehaviour, IPunInstantiateMagicCallback
{
    public static Action<int, Meme> OnMemeSelected;
    public static Action<int, Meme> OnMemeChoosen;
    public static Action<int, int, string> OnStatementSubmit;
    public static Action<int, int, string> OnRemateSubmit;
    public static Action OnVsDisplayHide;

    [TabGroup("DEBUG")][SerializeField] protected int stageNumber = 0;
    [TabGroup("DEBUG")][SerializeField] protected bool stageCompleted = false;
    protected PhotonView photonView = null;

    public virtual void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        Match.Instance.SetParent(this);
        stageNumber = (int)info.photonView.InstantiationData[0];
        photonView = info.photonView;
        if (photonView == null)
        {
            Debug.LogError("There isnt PhotonView on Match Prefab");
            return;
        }
        SetRectTransform();
        GetReferences();
        CheckReferences();
        Initialize();
        Activate();
        gameObject.SetActive(false);
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

    protected virtual void GetReferences(){}

    protected virtual void CheckReferences(){}

    protected virtual void Initialize(){}

    protected virtual void Activate()
    {
        Match.OnStageTimeStarted += EnableStage;
        Match.OnStageTimeEnded += DisableStage;
    }

    protected virtual void Desactivate()
    {
        Match.OnStageTimeStarted -= EnableStage;
        Match.OnStageTimeEnded -= DisableStage;
    }

    private void EnableStage(int stage)
    {
        if(stageNumber != stage)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        StartStage();
    }

    protected virtual void StartStage(){}

    private void DisableStage(int stage)
    {
        if(stageNumber != stage)
        {
            return;
        }
        EndStage();
        Desactivate();
        gameObject.SetActive(false);
    }

    protected virtual void EndStage(){}
}