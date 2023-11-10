

namespace TETRIS.Exceptions
{
    public class GameOverException : Exception
    {
        public int Score { get; } // Свойство, содержащее счет игрока при завершении игры.

        // Конструктор исключения GameOverException.
        public GameOverException(int score)
        {
            Score = score;
        }
    }
}