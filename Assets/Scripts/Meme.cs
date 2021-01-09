using UnityEngine;

public class Meme
{
    public Meme(int ID, Sprite sprite)
    {
        _ID = ID;
        _sprite = sprite;
    }

    public int ID { get => _ID; set => _ID = value; }
    public Sprite Sprite { get => _sprite; set => _sprite = value; }
    public string Statement { get => _statement; set => _statement = value; }
    public string Remate { get => _remate; set => _remate = value; }

    [SerializeField] private int _ID = -1;
    [SerializeField] private Sprite _sprite = null;
    [SerializeField] private string _statement = string.Empty;
    [SerializeField] private string _remate = string.Empty;
}