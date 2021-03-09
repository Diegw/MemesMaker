using System;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Action<bool> OnConectivitySelectedEvent;
    
    [SerializeField] private Button _onlineButton = null;
    [SerializeField] private Button _localButton = null;
    private Button[] _buttons = null;

    private void Awake()
    {
        _buttons = new Button[2] {_onlineButton, _localButton};
        ObjectSelect.Instance.SelectObject(_onlineButton.gameObject);
    }

    private void OnEnable()
    {
        _onlineButton.onClick.AddListener(OnlineButton);
        _localButton.onClick.AddListener(LocalButton);
    }

    private void OnDisable()
    {
        _onlineButton.onClick.RemoveListener(OnlineButton);
        _localButton.onClick.RemoveListener(LocalButton);
    }

    private void OnlineButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(false);
    }

    public void LocalButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(true);
    }

    private void DisableButtons()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = false;
        }
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}