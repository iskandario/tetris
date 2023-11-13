namespace TETRIS.BusinessLogic;

using JetBrains.Annotations;

public class Cell(int x, int y, bool isFilled, string color)
{
    [UsedImplicitly]

    public int X {  get; set; } = x;

    [UsedImplicitly]

    public int Y {  get; set; } = y;

    public bool IsFilled { get; set; } = isFilled;
    public string Color { get; set; } = color;
}