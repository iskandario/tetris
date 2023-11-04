namespace TETRIS.BusinessLogic;

using JetBrains.Annotations;

public class Cell // класс ячейка
{
    [UsedImplicitly]

    public int X {  get; set; }
    [UsedImplicitly]

    public int Y {  get; set; }
    public bool IsFilled { get; set; }
    public string Color { get; set; }

    public Cell(int x, int y, bool isFilled, string color)
    {
        X = x;
        Y = y;
        IsFilled = isFilled;
        Color = color;
    }
}