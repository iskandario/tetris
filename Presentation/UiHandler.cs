using TETRIS.BusinessLogic;

namespace TETRIS.Presentation
{
    public class UiHandler(Cell[,] cells, Figure currentFigure, int height, int width)
    {
        // свойства для хранения размеров поля и текущей фигуры
        private int Height { get; set; } = height;
        private int Width { get; set; } = width;
        private Figure CurrentFigure { get; set; } = currentFigure;
        private Cell[,] Cells { get; set; } = cells;


        // обновляет текущее состояние игры
        public void Update(Cell[,] cells, Figure currentFigure, int height, int width)
        {
            Console.Clear(); // очистка консоли для нового рендера
            Cells = cells;
            CurrentFigure = currentFigure;
            Height = height;
            Width = width;
        }

        // выбор цвета в зависимости от цвета фигуры
        public ConsoleColor GetColor(string color)
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

        // основной метод для рендеринга игры
        public void Render(GameField field)
        {
            const int topPadding = 5; // отступ сверху для поля

            // создаем отступы в начале рендера
            for (int i = 0; i < topPadding; i++)
            {
                Console.WriteLine();
            }

            int startX = 0; // начальная позиция по X
            int startY = topPadding; // начальная позиция по Y

            // рендеринг игрового поля
            for (int y = 0; y < Height; y++)
            {
                Console.SetCursorPosition(startX, startY + y);

                for (int x = 0; x < Width; x++)
                {
                    bool filled = Cells[x, y].IsFilled;

                    // проверка наличия фигуры в текущей клетке
                    if (CurrentFigure != null && x >= CurrentFigure.X && y >= CurrentFigure.Y
                        && x < CurrentFigure.X + CurrentFigure.BorderSizeX &&
                        y < CurrentFigure.Y + CurrentFigure.BorderSizeY)
                    {
                        filled = filled || CurrentFigure.Shape[y - CurrentFigure.Y, x - CurrentFigure.X];
                    }

                    // вывод клетки в зависимости от ее состояния
                    if (filled)
                    {
                        Console.ForegroundColor = GetColor(Cells[x, y].Color);
                        Console.Write("\u2593\u2593 "); // заполненная клетка
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("\u2591\u2591 "); // пустая клетка
                    }
                }

                Console.WriteLine();
            }

            // вывод счета и следующей фигуры
            Console.WriteLine("Score: " + field.Score);
            Console.WriteLine("Next Figure: ");
            for (int y = 0; y < field.NextFigure.SizeY; y++)
            {
                // вывод следующей фигуры
                for (int x = 0; x < field.NextFigure.SizeX; x++)
                {
                    Console.ForegroundColor = GetColor(field.NextFigure.Color);
                    Console.Write(field.NextFigure.Shape[y, x] ? "▓▓ " : "   ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }
    }
}
