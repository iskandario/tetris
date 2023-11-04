
using TETRIS.BusinessLogic;
namespace TETRIS.Presentation;

public class UiHandler
    {
        private int Height { get; set; }
        private int Width { get; set; }
        private Figure CurrentFigure { get; set; }
        private Cell[,] Cells { get; set; }

        public UiHandler(Cell[,] cells, Figure currentFigure, int height, int width)
        {
            Cells = cells;
            CurrentFigure = currentFigure;
            Height = height;
            Width = width;
        }

        public void Update(Cell[,] cells, Figure currentFigure, int height, int width)
        {
            Cells = cells;
            CurrentFigure = currentFigure;
            Height = height;
            Width = width;
        }

        private ConsoleColor GetColor(string color)
        {
            switch (color)
            {
                case "red": return ConsoleColor.Red;
                case "blue": return ConsoleColor.Blue;
                case "orange": return ConsoleColor.DarkYellow;
                case "green": return ConsoleColor.Green;
                case "yellow": return ConsoleColor.Yellow;
                case "purple": return ConsoleColor.DarkMagenta;
                case "pink": return ConsoleColor.Magenta;
                default: return ConsoleColor.White;
            }
        }

        public void Render(GameField field)
        {
            Console.SetCursorPosition(0, 0); // Устанавливаем курсор в начало


            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool filled = Cells[x, y].IsFilled;

                    if (CurrentFigure != null && x >= CurrentFigure.X && y >= CurrentFigure.Y
                        && x < CurrentFigure.X + CurrentFigure.BorderSizeX &&
                        y < CurrentFigure.Y + CurrentFigure.BorderSizeY)
                    {
                        filled = filled || CurrentFigure.Shape[y - CurrentFigure.Y, x - CurrentFigure.X];
                    }

                    if (filled)
                    {
                        Console.ForegroundColor = GetColor(Cells[x, y].Color);
                        Console.Write("\u2593\u2593 ");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("\u2591\u2591 ");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("Score: " + field.Score);

            Console.WriteLine("Next Figure: ");
            for (int y = 0; y < field.NextFigure.SizeY; y++)
            {
                for (int x = 0; x < field.NextFigure.SizeX; x++)
                {
                    Console.ForegroundColor = GetColor(field.NextFigure.Color);
                    Console.Write(field.NextFigure.Shape[y, x] ? "\u2593\u2593 " : "   ");
                    Console.ResetColor();
                }

                Console.WriteLine();
            }

          
            for (int i = field.NextFigure.SizeY; i < 4; i++) 
            {
                Console.WriteLine("  ");
            }
        }
    }
