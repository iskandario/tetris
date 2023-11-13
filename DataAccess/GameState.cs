
using TETRIS.BusinessLogic;

namespace TETRIS.DataAccess;


// конструктор класса GameState, принимающий игровое поле, текущую фигуру, время последнего смещения, счет и имя игрока
public class GameState(GameField field, Figure current, DateTime lastMoveDown, int score, string playerName)
{
    
    public GameField Field { get; } = field;


    public Figure Current { get; } = current;


    public DateTime LastMoveDown { get; } = lastMoveDown;


    public int Score { get; } = score;


    public string PlayerName { get; } = playerName;

}