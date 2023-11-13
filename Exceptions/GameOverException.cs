

namespace TETRIS.Exceptions
{
    public class GameOverException(int score) : Exception
    {
        public int Score { get; } = score; // Свойство, содержащее счет игрока при завершении игры.

        // Конструктор исключения GameOverException.
    }
}