using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputReceptor : MonoBehaviour
{
    public static Action OnAcceptEvent;
    public static Action OnDeclineEvent;
    public static Action OnAnyEvent;

    private Controls _controls = null;
    private PlayerInput _playerInput = null;

    private InputAction _directionAction = null;
    private InputAction _acceptAction = null;
    private InputAction _declineAction = null;
    private InputAction _anyAction = null;

    private const string DIRECTION = "Direction";
    private const string ACCEPT = "Accept";
    private const string DECLINE = "Decline";
    private const string ANY = "Any";

    private void Awake()
    {
        DontDestroyOnLoad(this);

        _playerInput = GetComponent<PlayerInput>();
        if(_playerInput == null)
        {
            Debug.LogError("Couldnt find PlayerInput Component in PlayerInput Prefab");
            return;
        }
        _controls = new Controls();
    }

    private void Start()
    {
        _playerInput.currentActionMap = _controls.Player;
        _playerInput.actions.Enable();

        _directionAction = _playerInput.actions[DIRECTION];
        _acceptAction = _playerInput.actions[ACCEPT];
        _declineAction = _playerInput.actions[DECLINE];
        _anyAction = _playerInput.actions[ANY];

        SubscribeToPlayerInput();
    }

    private void SubscribeToPlayerInput()
    {
        if (_playerInput == null)
        {
            return;
        }

        _acceptAction.started += AcceptEvent;
        _declineAction.started += DeclineEvent;
        _anyAction.started += AnyEvent;
    }

    private void UnsubscribeToPlayerInput()
    {
        if (_playerInput == null)
        {
            return;
        }

        _acceptAction.started += AcceptEvent;
        _declineAction.started += DeclineEvent;
        _anyAction.started += AnyEvent;
    }

    private void AcceptEvent(InputAction.CallbackContext context)
    {
        OnAcceptEvent?.Invoke();
    }

    private void DeclineEvent(InputAction.CallbackContext context)
    {
        OnDeclineEvent?.Invoke();
    }

    private void AnyEvent(InputAction.CallbackContext context)
    {
        OnAnyEvent?.Invoke();
    }

    public Vector2 GetDirection()
    {
        return _directionAction.ReadValue<Vector2>();
    }

    private void OnDestroy()
    {
        UnsubscribeToPlayerInput();
    }
}