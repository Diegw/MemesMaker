using System.Collections.Generic;
using UnityEngine;

public class GameSettings : ScriptableObject
{
    public bool IsOffline { get => _isOffline; set => _isOffline = value; }
    public List<Sprite> MemesSprites { get => _memesSprites; set => _memesSprites = value; }

    [SerializeField] private bool _isOffline = false;
    [SerializeField] private List<Sprite> _memesSprites = new List<Sprite>();
}