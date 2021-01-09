using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Stage : MonoBehaviour
{
    public static Action<Meme> OnButtonPressedEvent;
    public GameObject FirstChoice { get => _firstChoice; set => _firstChoice = value; }
    public GameObject SecondChoice { get => _secondChoice; set => _secondChoice = value; }
    public List<Button> Buttons { get => _buttons; set => _buttons= value; }
    public Dictionary<Button, Meme> MemesToSelect { get => _memesToSelect; set => _memesToSelect = value; }

    [SerializeField] private GameObject _firstChoice = null;
    [SerializeField] private GameObject _secondChoice = null;
    [SerializeField] private List<Button> _buttons = new List<Button>();
    [SerializeField] private Dictionary<Button, Meme> _memesToSelect = new Dictionary<Button, Meme>();

    private void Awake()
    {
        MatchUI.Instance.SetParent(this);

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = Vector3.one;
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;

        CheckReferences();
        foreach (Button button in _buttons)
        {
            _memesToSelect.Add(button, new Meme(-1, null));
        }
        Initialize(); 
    }

    private void CheckReferences()
    {
        if(_firstChoice == null)
        {
            Debug.LogError("Buttons1And2 Reference isnt assign. Check MatchUI");
            return;
        }
        if(_secondChoice == null)
        {
            Debug.LogError("Buttons3And4 Reference isnt assign. Check MatchUI");
            return;
        }
        foreach (Button button in _buttons)
        {
            if(button == null)
            {
                Debug.LogError("Some Button Reference couldnt be found. Check MatchUI");
                return;
            }
        }
    }

    private void Initialize()
    {
        ToogleChoices(true, false);
        ObjectSelect.Instance.SelectObject(_buttons[0].gameObject);
    }

    private void OnEnable()
    {
        Match.OnSpritesReadyToShow += ShowImages;
        for (int i = 0; i < _buttons.Count; i++)
        {
            if(i < 2)
            {
                _buttons[i].onClick.AddListener(ButtonFirstChoice);
            }
            else
            {
                _buttons[i].onClick.AddListener(ButtonSecondChoice);
            }
        }
    }

    private void OnDisable()
    {
        Match.OnSpritesReadyToShow -= ShowImages;
        for (int i = 0; i < _buttons.Count; i++)
        {
            if(i < 2)
            {
                _buttons[i].onClick.RemoveListener(ButtonFirstChoice);
            }
            else
            {
                _buttons[i].onClick.RemoveListener(ButtonSecondChoice);
            }
        }
    }

    private void ShowImages(List<Meme> memes)
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
    }

    private void ButtonFirstChoice()
    {
        ToogleChoices(false, true);
        Button buttonPressed = ObjectSelect.Instance.ObjectSelected.GetComponent<Button>();
        Meme memeSelected = _memesToSelect[buttonPressed];
        OnButtonPressedEvent?.Invoke(memeSelected);
        ObjectSelect.Instance.SelectObject(_buttons[2].gameObject);
    }

    private void ButtonSecondChoice()
    {
        ToogleChoices(false, false);
        Button buttonPressed = ObjectSelect.Instance.ObjectSelected.GetComponent<Button>();
        Meme memeSelected = _memesToSelect[buttonPressed];
        OnButtonPressedEvent?.Invoke(memeSelected);
    }

    private void ToogleChoices(bool firstChoiceState, bool secondChoiceState)
    {
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
        _firstChoice?.SetActive(firstChoiceState);
        _secondChoice?.SetActive(secondChoiceState);
    }
}