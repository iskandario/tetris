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
                public int score { get;  set; } = 0;
                public Figure nextFigure; 
            
                public void SetScore(int score)
                {
                    this.score = score;
                }

            
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
                        uiHandler.Update(cells, currentFigure, Height, Width);
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
            
                public Figure GetRandomFigure()
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
    }
public class GameState
{
    public GameField field;
    public Figure current;
    public DateTime lastMoveDown;
    public int score;

    public GameState(GameField field, Figure current, DateTime lastMoveDown, int score)
    {
        this.field = field;
        this.current = current;
        this.lastMoveDown = lastMoveDown;
        this.score = score;
    }
}




public class GameMenu
{  private readonly Game game;

    public GameMenu(Game game)
    {
        this.game = game;
    }

    public static string Serialize(object toSerialize)
    {
        return JsonConvert.SerializeObject(toSerialize);
    }

    public static T Deserialize<T>(string toDeserialize)
    {
        return JsonConvert.DeserializeObject<T>(toDeserialize);
    }

    public void SaveGame(Game game, string filePath)
    {
        GameState gameState = new GameState(game.field, game.current, game.lastMoveDown, game.field.score);
        var jsonString = Serialize(gameState);
        Console.WriteLine("Saving game...");
        File.WriteAllText(filePath, jsonString);
        Console.WriteLine("Game saved successfully!");
    }


    public Game LoadGameFromFile(string filePath)
    {
        try
        {
            var jsonString = File.ReadAllText(filePath);
            var loadedGameState = Deserialize<GameState>(jsonString);
            if (loadedGameState != null)
            {
                Game loadedGame = new Game(loadedGameState);
                loadedGame.uiHandler = new UIHandler(loadedGame.field.cells, loadedGame.field.currentFigure, GameField.Height, GameField.Width);
                loadedGame.field.InitializeUIHandler(loadedGame.uiHandler);
            
                if (loadedGame.field.currentFigure == null)
                {
                    loadedGame.field.SpawnNewFigure();
                }

                if (loadedGame.field.nextFigure == null)
                {
                    loadedGame.field.nextFigure = loadedGame.field.GetRandomFigure();
                }

                loadedGame.uiHandler.Update(loadedGame.field.cells, loadedGame.field.currentFigure, GameField.Height, GameField.Width);  // Обновить состояние UIHandler
            
                Console.WriteLine("Game loaded successfully!");
                return loadedGame;
            }
            else
            {
                Console.WriteLine("Failed to load game: GameState is null");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to load game: " + ex.Message);
        }
        return null;
    }




















    public bool InGameMenu(Game game, string filePath)
    {
        Console.WriteLine("Game Menu:");
        Console.WriteLine("1. Continue Game");
        Console.WriteLine("2. Save Game");
        Console.WriteLine("3. Load Game");
        Console.WriteLine("4. Exit to Main Menu");
        Console.Write("Enter your choice: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                return false; // Продолжить игру
            case "2":
                game.SaveGame(); // Сохранить игру
                return false;
            case "3":
                game.LoadGame(); // Загрузить игру
                return false;
            case "4":
                throw new ExitToMainMenuException(); // Выход в главное меню

            default:
                Console.WriteLine("Invalid choice. Please choose again.");
                return InGameMenu( game, filePath);
        }
    }


    public string MainMenu()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("1. Start Game");
        Console.WriteLine("2. Scoreboard");
        Console.WriteLine("3. Exit");
        Console.Write("Enter your choice: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                StartGameMenu();  // Вызываем метод подменю "Start Game"
                break;
            case "2":
                ScoreBoard scoreBoard = new ScoreBoard();  // Создаем объект доски счетов
                scoreBoard.LoadFromFile();  // Загружаем счета из файла
                scoreBoard.DisplayScores();  // Отображаем доску счетов
                break;
            case "3":
                System.Environment.Exit(0);  // Выходим из игры
                break;
            default:
                Console.WriteLine("Invalid choice. Please choose again.");
                MainMenu();  // Заново вызываем метод `MainMenu`, если введен неправильный выбор
                break;
        }

        return choice;
    }
    

    public void StartGameMenu()
    {
        Console.WriteLine("Start Game:");
        Console.WriteLine("1. Start New Game");
        Console.WriteLine("2. Load Game");
        Console.Write("Enter your choice: ");

        string choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Game newGame = new Game();
                newGame.StartNewGame();
                break;
            case "2":
                // Загружаем состояние игры из файла
                Game loadedGame = LoadGameFromFile("/Users/iskandargarifullin/RiderProjects/TETRIS/TETRIS/gameField.json");
                if (loadedGame != null)
                {
                    // Создаем новую игру с загруженным состоянием
                    loadedGame.PlayGame("Loaded Game").Wait();
                }
                else
                {
                    Console.WriteLine("No saved game found.");
                }
                break;
            default:
                Console.WriteLine("Invalid choice. Please choose again.");
                StartGameMenu();
                break;
        }
    }
}
public class ExitToMainMenuException : Exception
{
}

public class Game
    {
        public GameField field;
        public Figure current;
        public DateTime lastMoveDown = DateTime.Now;
        public const int MoveDownIntervalMs = 500;
        public UIHandler uiHandler;
        public GameMenu gameMenu;
        private const string filePath = "/Users/iskandargarifullin/RiderProjects/TETRIS/TETRIS/gameField.json";

        public string FilePath
        {
            get { return filePath; }
        }
      
        public Game(GameState gameState)
        {
            this.field = gameState.field;
            this.current = gameState.current;
            this.lastMoveDown = gameState.lastMoveDown;
            this.field.SetScore(gameState.score);
            this.uiHandler = new UIHandler(field.cells, field.currentFigure, GameField.Height, GameField.Width);
            this.field.InitializeUIHandler(uiHandler);
            this.gameMenu = new GameMenu(this);
        }


        public Game()
        {
            field = new GameField(uiHandler);
            uiHandler = new UIHandler(field.cells, field.currentFigure, GameField.Height, GameField.Width);
            field.InitializeUIHandler(uiHandler);
            gameMenu = new GameMenu(this);
        }


        public void SaveGame()
        {
            gameMenu.SaveGame(this, filePath);
        }

        public void LoadGame()
        {
            Game loadedGame = gameMenu.LoadGameFromFile(filePath);
            if (loadedGame != null)
            {
                // Запускаем игру с загруженными данными
                loadedGame.PlayGame("Loaded Game").Wait();
            }
            else
            {
                Console.WriteLine("No saved game found.");
            }
        }






        public void StartNewGame()
        {
            Console.WriteLine("Enter your name: ");
            var name = Console.ReadLine() ?? "Player"; // "Player" как имя по умолчанию, если введено пустое значение
            field = new GameField(uiHandler);
            current = field.currentFigure;
            uiHandler = new UIHandler(field.cells, field.currentFigure, GameField.Height, GameField.Width);
            field.InitializeUIHandler(uiHandler);
            var scoreBoard = new ScoreBoard();
            // Запускаем асинхронный метод игрового цикла
            PlayGame(name).Wait();
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
                    lastMoveDown = DateTime.Now;  // Обновляем время последнего смещения
                    break;
                case ConsoleKey.UpArrow:
                    field.RotateFigure(field.currentFigure);
                    break;
                case ConsoleKey.Z:  // Обработка клавиши Z для вызова внутриигрового меню
                    bool exitToMainMenu = gameMenu.InGameMenu(this, "gameField.json");
                    if (exitToMainMenu)
                    {
                        return;  // Возвращаемся в главное меню
                    }
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

        public async Task PlayGame(string playerName)
        {
            try
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true);
                        HandleInput(key.Key);
                        if (key.Key == ConsoleKey.Z)
                        {
                            if (gameMenu.InGameMenu(this, filePath))
                            {
                                return; // Вернуться в главное меню
                            }
                        }
                    }

                    if ((DateTime.Now - lastMoveDown).TotalMilliseconds >= MoveDownIntervalMs)
                    {
                        field.MoveFigureDown(field.currentFigure);
                        lastMoveDown = DateTime.Now;
                    }

                    Render();
                    await Task.Delay(100);
                }
            }
            catch (ExitToMainMenuException)
            {
                return; // Возвращаемся в главное меню
            }
            catch (GameOverException e)
            {
                Console.WriteLine($"Game over! Your score: {e.Score}");
                var scoreBoard = new ScoreBoard();
                scoreBoard.AddOrUpdatePlayerScore(playerName, e.Score);
                scoreBoard.DisplayScores();
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine($"Game over! Your score: {field.score}");
                var scoreBoard = new ScoreBoard();
                scoreBoard.AddOrUpdatePlayerScore(playerName, field.score);
                scoreBoard.DisplayScores();
            }
        }

        public async Task GameLoop()
        {
            while (true) // бесконечный цикл для главного меню
            {
                var option = gameMenu.MainMenu(); // вызываем результат MainMenu

                switch (option)
                {
                    case "1": // Start Game
                        gameMenu.StartGameMenu(); // вызываем подменю "Start Game"
                        break;
                    case "2": // Scoreboard
                        ScoreBoard scoreBoard = new ScoreBoard();
                        scoreBoard.LoadFromFile();
                        scoreBoard.DisplayScores();
                        break;
                    case "3": // Exit
                        Environment.Exit(0);
                        break;
                }
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

    }//over

          


  