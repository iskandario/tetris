namespace TETRIS.BusinessLogic;

public class Player
{
    public string Name { get; set; }
    public int Score { get; set; }

    public Player(string name, int score)
    {
        Name = name;
        Score = score;
    }
}