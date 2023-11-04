
using TETRIS.BusinessLogic;

namespace TETRIS.DataAccess;

public class GameState
{
    public GameField Field { get; set; }
    public Figure Current { get; set; }
    public DateTime LastMoveDown { get; set; }
    public int Score { get; set; }
    public string PlayerName { get; set; }


    public GameState(GameField field, Figure current, DateTime lastMoveDown, int score, string playerName)
    {
        this.Field = field;
        this.Current = current;
        this.LastMoveDown = lastMoveDown;
        this.Score = score;
        this.PlayerName = playerName;

    }
}