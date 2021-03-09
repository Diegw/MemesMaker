using System;
using UnityEngine;

[Serializable] public class Meme
{
    public Meme(int spriteID, Sprite sprite, int playerID = -1, int statementID = -1, string statement = "", int remateID = -1, string remate = "")
    {
        _spriteID = spriteID;
        _sprite = sprite;
        _playerID = playerID;
        _statementID = statementID;
        _statement = statement;
        _remateID = remateID;
        _remate = remate;
    }

    public int PlayerID { get => _playerID; set => _playerID = value; }
    public int SpriteID { get => _spriteID; set => _spriteID = value; }
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public int StatementID { get => _statementID; set => _statementID = value; }
    public string Statement { get => _statement; set => _statement = value; }
    public int RemateID { get => _remateID; set => _remateID = value; }
    public string Remate { get => _remate; set => _remate = value; }

    [SerializeField] private int _playerID = -1;
    [SerializeField] private int _spriteID = -1;
    [SerializeField] private Sprite _sprite = null;
    [SerializeField] private int _statementID = -1;
    [SerializeField] private string _statement = string.Empty;
    [SerializeField] private int _remateID = -1;
    [SerializeField] private string _remate = string.Empty;
}