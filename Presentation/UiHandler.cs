
using TETRIS.BusinessLogic;

namespace TETRIS.Presentation
{
    public class UiHandler
    {
        private int Height { get; set; } // Высота игрового поля
        private int Width { get; set; } // Ширина игрового поля
        private Figure CurrentFigure { get; set; } // Текущая фигура
        private Cell[,] Cells { get; set; } // Массив клеток игрового поля

        // Конструктор класса UiHandler, принимающий массив клеток, текущую фигуру, высоту и ширину игрового поля.
        public UiHandler(Cell[,] cells, Figure currentFigure, int height, int width)
        {
            Cells = cells;
            CurrentFigure = currentFigure;
            Height = height;
            Width = width;
        }

        // Метод Update обновляет данные текущего объекта UiHandler.
        public void Update(Cell[,] cells, Figure currentFigure, int height, int width)
        {
            Cells = cells;
            CurrentFigure = currentFigure;
            Height = height;
            Width = width;
        }

        // Метод GetColor возвращает ConsoleColor на основе строки цвета.
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

        // Метод Render отображает игровое поле, текущую фигуру и следующую фигуру.
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
                        Console.Write("\u2593\u2593 "); // Отображаем заполненную клетку
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write("\u2591\u2591 "); // Отображаем пустую клетку
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine("Score: " + field.Score); // Отображаем счет игры

            Console.WriteLine("Next Figure: ");
            for (int y = 0; y < field.NextFigure.SizeY; y++)
            {
                if (y >= field.NextFigure.SizeY)
                {
                    Console.WriteLine("  "); // Добавляем пустые строки для выравнивания размера
                }
                else
                {
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
}
