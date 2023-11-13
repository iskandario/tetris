
using TETRIS.BusinessLogic;

namespace TETRIS.DataAccess;


// конструктор класса GameState, принимающий игровое поле, текущую фигуру, время последнего смещения, счет и имя игрока
public class GameState(GameField field, Figure current, DateTime lastMoveDown, int score, string playerName)
{
    
    public GameField Field { get; set; } = field;


    public Figure Current { get; set; } = current;


    public DateTime LastMoveDown { get; set; } = lastMoveDown;


    public int Score { get; set; } = score;


    public string PlayerName { get; set; } = playerName;

}