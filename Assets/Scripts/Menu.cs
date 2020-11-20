using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private Button _singlePlayerButton = null;
    [SerializeField] private Button _multiPlayerButton = null;
    private Button[] buttons = null;

    private void Awake()
    {
        ButtonsAddListener();
        buttons = FindObjectsOfType<Button>();
        ObjectSelect.Instance.SelectObject(_singlePlayerButton.gameObject);
    }

    private void ButtonsAddListener()
    {
        _singlePlayerButton.onClick.AddListener(SinglePlayerButton);
        _multiPlayerButton.onClick.AddListener(MultiPlayerButton);
    }

    public void SinglePlayerButton()
    {
        DisableButtons();
    }

    private void MultiPlayerButton()
    {
        DisableButtons();
    }

    private void DisableButtons()
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }
    }
}