using UnityEngine;

public class GameSettings : ScriptableObject
{
    public bool IsOffline { get => _isOffline; set => _isOffline = value; }
    
    [SerializeField] private bool _isOffline = false;
}