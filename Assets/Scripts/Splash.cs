using UnityEngine;
using UnityEngine.InputSystem;

public class Splash : MonoBehaviour
{
    [SerializeField] private PlayerInputManager _playerInputManager = null;

    private void Awake()
    {
        if(_playerInputManager != null)
        {
            return;
        }
        _playerInputManager = FindObjectOfType<PlayerInputManager>();
    }

    private void OnEnable()
    {
        if(_playerInputManager == null)
        {
            Debug.LogError("There isnt any PlayerInputManager");
            return;
        }
        _playerInputManager.onPlayerJoined += Continue;
    }

    private void OnDisable()
    {
        _playerInputManager.onPlayerJoined -= Continue;
    }

    private void Continue(PlayerInput playerInput)
    {
        ScenesManager.Instance.LoadScene(ScenesManager.MENU_SCENE);
    }

    private void OnDestroy()
    {
        OnDisable();
    }
}