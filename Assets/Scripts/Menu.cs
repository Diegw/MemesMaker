using System;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    // public static Action OnLocalSelectedEvent;
    // public static Action OnOnlineSelectedEvent;
    public static Action<bool> OnConectivitySelectedEvent;
    
    [SerializeField] private Button _localButton = null;
    [SerializeField] private Button _onlineButton = null;
    private Button[] buttons = null;

    private void Awake()
    {
        buttons = new Button[2] {_localButton, _onlineButton};
        ObjectSelect.Instance.SelectObject(_localButton.gameObject);
    }

    private void OnEnable()
    {
        ButtonsAddListener();
    }

    private void OnDisable()
    {
        ButtonsRemoveListener();
    }

    private void ButtonsAddListener()
    {
        _localButton.onClick.AddListener(LocalButton);
        _onlineButton.onClick.AddListener(OnlineButton);
    }

    private void ButtonsRemoveListener()
    {
        _localButton.onClick.RemoveListener(LocalButton);
        _onlineButton.onClick.RemoveListener(OnlineButton);
    }

    public void LocalButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(true);
        // OnLocalSelectedEvent?.Invoke();
    }

    private void OnlineButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(false);
        // OnOnlineSelectedEvent?.Invoke();
    }

    private void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}