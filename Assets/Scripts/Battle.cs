public class Battle
{
    public Battle(Meme[] memesToBattle)
    {
        _memesToBattle = memesToBattle;
    }

    public Meme[] MemesToBattle => _memesToBattle;

    Meme[] _memesToBattle = new Meme[2];
}