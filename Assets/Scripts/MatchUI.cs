using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MatchUI : SerializedMonoBehaviour
{
    public static MatchUI Instance => _instance;
    public static Action<Sprite, Sprite> OnImagesSelected;

    private static MatchUI _instance = null;
    [TabGroup("STAGES")][SerializeField] private Stage _stagePrefab = null;
    [TabGroup("STAGES")][SerializeField] private List<Stage> _stages = new List<Stage>();
    [TabGroup("DEBUG")][SerializeField] private List<Meme> _memesSelected = new List<Meme>();
    
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance);
        }
        _instance = this;

        for (int i = 0; i < 1; i++)
        {
            Stage newStage = Instantiate(_stagePrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<Stage>();
            _stages.Add(newStage);
        }
    }

    private void Start()
    {
        ToogleStages(0);
    }

    private void OnEnable()
    {
        Stage.OnButtonPressedEvent += AddMemeSelected;
    }

    private void OnDisable()
    {
        Stage.OnButtonPressedEvent -= AddMemeSelected;
    }

    private void AddMemeSelected(Meme memeSelected)
    {
        if(_memesSelected.Contains(memeSelected))
        {
            return;
        }
        _memesSelected.Add(memeSelected);
    }

    private void ToogleStages(int stageIndex)
    {
        for (int i = 1; i < _stages.Count; i++)
        {
            if(i == stageIndex)
            {
                continue;
            }
            _stages[i].gameObject.SetActive(false);
        }
    }

    public void SetParent(MonoBehaviour element)
    {
        element.transform.SetParent(transform);
    }
}