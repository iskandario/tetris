namespace TETRIS.BusinessLogic;

public class Player(string name, int score)
{
    public string Name { get; } = name;
    public int Score { get; set; } = score;
}