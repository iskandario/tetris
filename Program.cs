using Newtonsoft.Json;

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
                public const int Width = 10;
                public const int Height = 20;
                public Cell[,] cells;
                private UIHandler uiHandler;
                public int score { get; private set; } = 0;
                public Figure nextFigure; 
            
            
            
                public GameField(UIHandler uiHandler)
                {
                    this.uiHandler = uiHandler;
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
                public void InitializeUIHandler(UIHandler handler)
                {
                    this.uiHandler = handler;
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
                
                        uiHandler.Render(this);
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
                            cells[x, y].IsFilled = cells[x, y - 1].IsFilled;
                            cells[x, y].Color = cells[x, y - 1].Color;
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
                    if (nextFigure == null)
                    {
                        nextFigure = GetRandomFigure();
                    }
            
                    if (FigureCanSpawn(nextFigure, this))
                    {
                        currentFigure = nextFigure;
                        nextFigure = GetRandomFigure(); // генерируем новую следующую фигуру после спавна текущей
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
            
            public class Player
            {
                public string Name { get; set; }
                public int Score { get; set; }
            
                public Player(string name, int score)
                {
                    Name = name;
                    Score = score;
                }
            }
            public class ScoreBoard 
            {
                private List<Player> players = new List<Player>();
                private string filePath = "./scores.json"; // путь и имя файла JSON
            
                public void AddOrUpdatePlayerScore(string name, int score)
                {
                    var existingPlayer = players.FirstOrDefault(p => p.Name == name);
            
                    if(existingPlayer == null)
                    {
                        players.Add(new Player(name, score));
                    }
                    else
                    {
                        existingPlayer.Score = Math.Max(existingPlayer.Score, score);
                    }
            
                    players = players.OrderByDescending(p => p.Score).ToList();
            
                    if(players.Count > 10) // if list exceed 10 players
                    {
                        players.RemoveAt(10); // remove last one
                    }
                    SaveToFile();
                }
            
                public void SaveToFile()
                {
                    var jsonData = JsonConvert.SerializeObject(players); 
                    File.WriteAllText(filePath, jsonData);
                }
                public void LoadFromFile()
                {
                    if (!File.Exists("scores.json")) return;
            
                    var json = File.ReadAllText("scores.json");
            
                    players = JsonConvert.DeserializeObject<List<Player>>(json);
                }
                public void DisplayScores()
                {
                    Console.WriteLine("Scoreboard:");
                    foreach (var player in players)
                    {
                        Console.WriteLine($"{player.Name}: {player.Score}");
                    }
                }
            
                // метод для загрузки списка игроков из файла при создании экземпляра класса 
                public ScoreBoard()
                {
                    if (File.Exists(filePath))
                    {
                        var jsonData = File.ReadAllText(filePath);
                        players = JsonConvert.DeserializeObject<List<Player>>(jsonData) ?? new List<Player>();
                    }
                }
            }
            public class Figure
            {
                public int SizeX
                {
                    get { return Shape.GetLength(1); } // Принимаем размер как длину внешнего измерения массива Shape
                }
            
                public int SizeY
                {
                    get { return Shape.GetLength(0); } // Принимаем размер как длину внешнего измерения массива Shape
                }
            
                public int BorderSizeX
                {
                    get
                    {
                        for (int x = SizeX - 1; x >= 0; x--)
                        {
                            for (int y = 0; y < SizeY; y++)
                            {
                                if (Shape[y, x]) return x + 1;
                            }
                        }
            
                        return 0;
                    }
                }
            
                public int BorderSizeY
                {
                    get
                    {
                        for (int y = SizeY - 1; y >= 0; y--)
                        {
                            for (int x = 0; x < SizeX; x++)
                            {
                                if (Shape[y, x]) return y + 1;
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
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { false, false, false, false },
                        { true, true, true, true },
                        { false, false, false, false },
                        { false, false, false, false }
                    }, "red"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { false, true, false },
                        { true, true, true },
                        { false, false, false }
            
                    }, "blue"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { true, true },
                        { true, true }
            
                    }, "orange"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { true, false, false },
                        { true, true, true },
                        { false, false, false },
            
                    }, "green"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { false, false, true },
                        { true, true, true },
                        { false, false, false },
            
                    }, "yellow"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
                    {
                        { false, true, true },
                        { true, true, false },
                        { false, false, false },
            
                    }, "purple"),
                    new Figure(GameField.Width / 2 - 2, -2, new bool[,]
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
                private UIHandler uiHandler;
            
                public Game()
                {
                    field = new GameField(uiHandler);
                    current = field.currentFigure;
                    uiHandler = new UIHandler(field.cells, field.currentFigure, GameField.Height, GameField.Width);
                    field.InitializeUIHandler(uiHandler);
            
            
            
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
                    uiHandler.Update(field.cells, field.currentFigure, GameField.Height, GameField.Width);
                    uiHandler.Render(field);
            
                }
            
            
            
                public async Task GameLoop()
                {
                    var name = string.Empty;
                    try
                    {
                        Console.WriteLine("Enter your name: ");
                        name = Console.ReadLine();
                        var scoreBoard = new ScoreBoard();
                        scoreBoard.LoadFromFile();
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
                        var scoreBoard = new ScoreBoard();
                        scoreBoard.AddOrUpdatePlayerScore(name, e.Score);
                        scoreBoard.DisplayScores(); // отображение доски счетов
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.WriteLine($"Game over! Your score: {field.score}");
                        var scoreBoard = new ScoreBoard();
                        scoreBoard.AddOrUpdatePlayerScore(name, field.score);
                        scoreBoard.DisplayScores(); // отображение доски счетов
                    }
                }
            }
            
            public class UIHandler
            {
                private int Height { get; set; }
                private int Width { get; set; }
                private Figure CurrentFigure { get; set; }
                private Cell[,] Cells { get; set; }
            
                public UIHandler(Cell[,] cells, Figure currentFigure, int height, int width)
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
                    Console.SetCursorPosition(0, 0);  // Устанавливаем курсор в начало
            
            
                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            bool filled = Cells[x, y].IsFilled;
            
                            if (CurrentFigure != null && x >= CurrentFigure.X && y >= CurrentFigure.Y
                                && x < CurrentFigure.X + CurrentFigure.BorderSizeX && y < CurrentFigure.Y + CurrentFigure.BorderSizeY)
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
            
                    Console.WriteLine("Score: " + field.score);
            
                    Console.WriteLine("Next Figure: ");
                    for (int y = 0; y < field.nextFigure.SizeY; y++)
                    {
                        for (int x = 0; x < field.nextFigure.SizeX; x++)
                        {
                            Console.ForegroundColor = GetColor(field.nextFigure.Color);
                            Console.Write(field.nextFigure.Shape[y, x] ? "\u2593\u2593 " : "   ");
                            Console.ResetColor();
                        }
            
                        Console.WriteLine();
                    }
            
                    // create empty lines under the "Next Figure" for consistent interface
                    for (int i = field.nextFigure.SizeY; i < 4; i++) // assuming 4 as the maximum height of a figure
                    {
                        Console.WriteLine("  ");
                    }
                }
            
                public class Program
                {
                    public static async Task Main()
                    {
                        Console.CursorVisible = false; // Отключить видимость курсора
                        Game game = new Game();
                        await game.GameLoop();
                    }
                }
            
            }
            


  