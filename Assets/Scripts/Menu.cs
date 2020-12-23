using System;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public static Action OnLocalSelectedEvent;
    public static Action OnOnlineSelectedEvent;
    public static Action<bool> OnConectivitySelectedEvent;
    
    [SerializeField] private Button _singlePlayerButton = null;
    [SerializeField] private Button _multiPlayerButton = null;
    private Button[] buttons = null;

    private void Awake()
    {
        buttons = FindObjectsOfType<Button>();
        ObjectSelect.Instance.SelectObject(_singlePlayerButton.gameObject);
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
        _singlePlayerButton.onClick.AddListener(LocalButton);
        _multiPlayerButton.onClick.AddListener(OnlineButton);
    }

    private void ButtonsRemoveListener()
    {
        _singlePlayerButton.onClick.RemoveListener(LocalButton);
        _multiPlayerButton.onClick.RemoveListener(OnlineButton);
    }

    public void LocalButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(true);
        OnLocalSelectedEvent?.Invoke();
    }

    private void OnlineButton()
    {
        DisableButtons();
        OnConectivitySelectedEvent?.Invoke(false);
        OnOnlineSelectedEvent?.Invoke();
    }

    private void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}