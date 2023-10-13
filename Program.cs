namespace Tetris_game
{
    public class Cell // класс ячейка
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsFilled { get; set; }
        public string Color { get; set; }

        public Cell(int x, int y, bool isFilled, string color) 
        {
            X = x;
            Y = y;
            IsFilled = false;
            Color = color;
        }
    }

    public class GameField
    {
        public const int Width = 20;
        public const int Height = 20;
        private Cell[,] cells;
        public int score { get; private set; } = 0;



        public GameField()
        {
            cells = new Cell[Width, Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    cells[x, y] = new Cell(x, y, false, "Empty");
                    
                }
            }
            SpawnNewFigure();
        }
        public bool[,] ToBoolArray()
        {
            bool[,] array = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    array[x, y] = cells[x, y].IsFilled;
                }
            }
            return array;
        }
        public void Render()
        {
            Console.Clear(); 
           
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    bool filled = cells[x, y].IsFilled;
                    // Проверка занимает текущая фигура данный квадрат или нет
                    if (currentFigure != null && x >= currentFigure.X && y >= currentFigure.Y 
                        && x < currentFigure.X + currentFigure.BorderSizeX && y < currentFigure.Y + currentFigure.BorderSizeY)
                    {
                        filled = filled || currentFigure.Shape[y - currentFigure.Y, x - currentFigure.X];
                    }

                    if (filled)
                    {
                        Console.ForegroundColor = GetColor(cells[x, y].Color); 
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
           
            Console.WriteLine();
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
        public void GameOver()
        {
            Console.WriteLine("Game Over! Your score is: " + score);
  
        }
    
        public void MoveFigureLeft(Figure figure)
        {
            figure.X--;
            if (!FigureCanSpawn(figure, this))
            {
                figure.X++;
            }
        }

        public void MoveFigureRight(Figure figure)
        {
            figure.X++;
            if (!FigureCanSpawn(figure, this))
            {
                figure.X--;
            }
        }

        public void MoveFigureDown(Figure figure)
        {
            Figure newFigure = figure.Clone();
            newFigure.Y++;
            if (FigureCanSpawn(newFigure, this))
            {
                currentFigure = newFigure;
            }
            else
            {
                FixFigure(figure);
                CheckLines();
                Render();
                SpawnNewFigure();
            }
        
        }
        public void RotateFigure(Figure figure)
        {
            if (figure.CanRotateRight(ToBoolArray()))
            {
                Figure newFigure = figure.RotateRight();

               
                if (newFigure.X + newFigure.Shape.GetLength(1) > Width)
                {
                    newFigure.X = Width - newFigure.Shape.GetLength(1);
                }
                else if (newFigure.X < 0)
                {
                    newFigure.X = 0;
                }

                if (FigureCanSpawn(newFigure, this))
                {
                    currentFigure = newFigure;
                }
            }
        }
        private void CheckLines()
        {
            for (int y = 0; y < GameField.Height; y++)
            {
                bool lineIsFilled = true;
                for (int x = 0; x < GameField.Width; x++)
                {
                    if (!cells[x, y].IsFilled)
                    {
                        lineIsFilled = false;
                        break;
                    }
                }
                if (lineIsFilled)
                {
                    DeleteLine(y);
                    score += 100; // увеличиваем счёт, когда строка удалена
                }
            }        
        }

        private void DeleteLine(int line)
        {
            for (int y = line; y > 0; y--)
            {
                for (int x = 0; x < GameField.Width; x++)
                {
                    cells[x, y].IsFilled = cells[x, y-1].IsFilled;
                    cells[x, y].Color = cells[x, y-1].Color;
                }
            }
            for (int x = 0; x < GameField.Width; x++)
            {
                // Очищаем самую верхнюю строку
                cells[x, 0].IsFilled = false;
                cells[x, 0].Color = "white";
            }
        }

        private Random rnd = new Random();
        public Figure currentFigure;
        private Figure GetRandomFigure()
        {
            int randomIndex = rnd.Next(0, Figure.AllFigures.Count);
            Figure newFigure = Figure.AllFigures[randomIndex].Clone();
            newFigure.X = Width / 2;
            newFigure.Y = -2;

            return newFigure;
        }
        public void SpawnNewFigure()
        {
            Figure newFigure = GetRandomFigure();
            if (FigureCanSpawn(newFigure, this))
            {
                currentFigure = newFigure;
            }
            else
            {
                throw new GameOverException(score);
            }
        }

        public void FixFigure(Figure figure)
        {
            for (int x = 0; x < figure.SizeX; x++)
            {
                for (int y = 0; y < figure.SizeY; y++)
                {
                    if (figure.Shape[y, x])
                    {
                        // Заполняем соответствующую клетку на игровом поле. 
                        // Принимаем, что фигура при "остановке" становится синей.
                        cells[figure.X + x, figure.Y + y].IsFilled = true;
                        cells[figure.X + x, figure.Y + y].Color = figure.Color;
                    }  
                }
            }
        }
        private bool FigureCanSpawn(Figure figure, GameField gameField)
        {
            for (int x = 0; x < figure.SizeX; x++)
            {
                for (int y = 0; y < figure.SizeY; y++)
                {
                    if (figure.Shape[y, x] == true)
                    {
                        if (figure.X + x < 0 || figure.X + x >= Width || figure.Y + y >= Height)
                        {
                            return false;
                        }
                        if (figure.Y + y >= 0 && cells[figure.X + x, figure.Y + y].IsFilled)
                        {
                            return false; // Ячейка, которую фигура собирается занять, уже заполнена
                        }
                    }
                }
            }
            return true; // Все ячейки, которые фигура собирается занять, свободны
        }
         
    }

    public class Figure
    {
        public int SizeX
        {
            get { return Shape.GetLength(1); }  // Принимаем размер как длину внешнего измерения массива Shape
        }
        public int SizeY
        {
            get { return Shape.GetLength(0); }  // Принимаем размер как длину внешнего измерения массива Shape
        }
        
        public int BorderSizeX
        {
            get
            {
                for(int x = SizeX-1; x >= 0; x--)
                {
                    for(int y = 0; y < SizeY; y++)
                    {
                        if (Shape[y, x]) return x+1;
                    }
                }
                return 0;
            }
        }

        public int BorderSizeY
        {
            get
            {
                for(int y = SizeY-1; y >= 0; y--)
                {
                    for(int x = 0; x < SizeX; x++)
                    {
                        if (Shape[y, x]) return y+1;
                    }
                }
                return 0;
            }
        }

     


        public int X { get; set; }
        public int Y { get; set; }
        public bool[,] Shape { get; set; }
        public string Color { get; set; }

        public Figure(int x, int y, bool[,] shape, string color)
        {
            X = x;
            Y = y;
            Shape = shape;
            Color = color;
        }

        public static List<Figure> AllFigures = new List<Figure>
        {
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { false, false, false, false },
                { true, true, true, true },
                { false, false, false, false },
                { false, false, false, false }
            }, "red"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { false, true, false },
                { true, true, true },
                { false, false, false }

            }, "blue"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { true, true },
                { true, true }

            }, "orange"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { true, false, false },
                { true, true, true },
                { false, false, false },

            }, "green"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { false, false, true },
                { true, true, true },
                { false, false, false },

            }, "yellow"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { false, true, true },
                { true, true, false },
                { false, false, false },

            }, "purple"),
            new Figure(GameField.Width / 2, -2, new bool[,]
            {
                { true, true, false },
                { false, true, true },
                { false, false, false },

            }, "pink")

        };
        public Figure Clone()
        {
            bool[,] newShape = new bool[Shape.GetLength(0), Shape.GetLength(1)];
            Array.Copy(Shape, newShape, Shape.Length);

            return new Figure(X, Y, newShape, Color);
        }
        
        public bool CanRotateRight(bool[,] filledCells)
        {
            int newSizeX = SizeY;
            int newSizeY = SizeX;
            bool[,] newShape = new bool[newSizeY, newSizeX];

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    newShape[x, SizeY - 1 - y] = Shape[y, x];
                }
            }

            
            return CheckPositionValid(newShape, X, Y, filledCells);
        }

        private bool CheckPositionValid(bool[,] shape, int shapeX, int shapeY, bool[,] filledCells)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int boardX = shapeX + x;
                    int boardY = shapeY + y;

                    // Проверяем, что клетка находится в рамках поля
                    if (boardX < 0 || boardX >= filledCells.GetLength(0) || boardY >= filledCells.GetLength(1))
                    {
                        return false;
                    }

                    // Пропускаем строки, которые еще не вошли в игровое поле
                    if (boardY < 0)
                    {
                        continue;
                    }

                    // Проверяем, что фигура не пересекается с другими заполненными клетками
                    if (shape[y, x] && filledCells[boardX, boardY])
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        public Figure RotateRight()
        {
            int newSizeX = SizeY;
            int newSizeY = SizeX;
            bool[,] newShape = new bool[newSizeY, newSizeX];

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    newShape[x, SizeY - 1 - y] = Shape[y, x];
                }
            }

            return new Figure(X, Y, newShape, Color);
        }
    }
    
    public class GameOverException : Exception
    {
        public int Score { get; }

        public GameOverException(int score)
        {
            Score = score;
        }
    }
    public class Game
    {
        private GameField field;
        private Figure current;
        private DateTime lastMoveDown = DateTime.Now;
        private const int MoveDownIntervalMs = 500;

        public Game()
        {
            field = new GameField();
            current = field.currentFigure;
            
        }

        public void HandleInput(ConsoleKey key)
        {
            switch (key)
            {
                case ConsoleKey.LeftArrow:
                    field.MoveFigureLeft(field.currentFigure);
                    break;
                case ConsoleKey.RightArrow:
                    field.MoveFigureRight(field.currentFigure);
                    break;
                case ConsoleKey.DownArrow:
                    field.MoveFigureDown(field.currentFigure);
                    lastMoveDown = DateTime.Now; // Обновляем время последнего смещения
                    break;
                case ConsoleKey.UpArrow:
                    field.RotateFigure(field.currentFigure);
                    break;
                default:
                    break;
            }
        }

        public void Render()
        {
            field.Render();
        }

      

        public async Task GameLoop()
        {
            try
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        HandleInput(key.Key);
                    }

                    // Проверяем, прошло ли достаточно времени с последнего смещения вниз
                    if ((DateTime.Now - lastMoveDown).TotalMilliseconds >= MoveDownIntervalMs)
                    {
                        field.MoveFigureDown(field.currentFigure);
                        lastMoveDown = DateTime.Now; // Обновляем время последнего смещения
                    }

                    Render();

                    await Task.Delay(100); // Более маленькая задержка для более быстрого реагирования на ввод
                }
            }
            catch (GameOverException e)
            {
                Console.WriteLine($"Game over! Your score: {e.Score}");
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Game over! Your score: {field.score}");
            }
        }
    } 
    public class Program
    {
        public static async Task Main() 
        {
            Game game = new Game();
            await game.GameLoop();
        }
    }
    
}