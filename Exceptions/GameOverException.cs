namespace TETRIS.Exceptions;

public class GameOverException : Exception
{
    public int Score { get; }

    public GameOverException(int score)
    {
        Score = score;
    }
}