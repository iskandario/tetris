
using TETRIS.BusinessLogic;

namespace TETRIS.DataAccess;

public class GameState
{
    // Игровое поле, включая заполненные и пустые ячейки.
    public GameField Field { get; set; }

    // Текущая игровая фигура.
    public Figure Current { get; set; }

    // Время последнего смещения фигуры вниз.
    public DateTime LastMoveDown { get; set; }

    // Счет игры.
    public int Score { get; set; }

    // Имя игрока.
    public string PlayerName { get; set; }

    // Конструктор класса GameState, принимающий игровое поле, текущую фигуру, время, счет и имя игрока.
    public GameState(GameField field, Figure current, DateTime lastMoveDown, int score, string playerName)
    {
        Field = field;
        Current = current;
        LastMoveDown = lastMoveDown;
        Score = score;
        PlayerName = playerName;
    }
}