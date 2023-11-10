using TETRIS.Presentation;
using TETRIS.Exceptions;
using TETRIS.DataAccess;
namespace TETRIS.BusinessLogic;

public class Game
{
    public GameField Field;
    public Figure? Current;
    public DateTime LastMoveDown = DateTime.Now;
    public const int MoveDownIntervalMs = 285;
    public UiHandler UiHandler;
    public GameMenu GameMenu;
    public string? PlayerName;
    private readonly GameStateSaver _gameStateSaver;
    

    // Конструктор по умолчанию, инициализирующий новую игру.
    public Game()
    {
        // Создание временной фигуры с минимально допустимым массивом
        bool[,] tempShape = new bool[,] { { true } };
        Figure tempFigure = new Figure(0, 0, tempShape, "TemporaryColor");

        // Создание временного UiHandler с временным объектом Figure
        UiHandler tempUiHandler = new UiHandler(new Cell[GameField.Width, GameField.Height], tempFigure,
            GameField.Height, GameField.Width);

        // Инициализация GameField с временным UiHandler
        Field = new GameField(tempUiHandler);

        // Создание и инициализация реального UiHandler теперь, когда Cells и CurrentFigure инициализированы
        UiHandler = new UiHandler(Field.Cells, Field.CurrentFigure, GameField.Height, GameField.Width);

        // Обновление UiHandler внутри GameField
        Field.InitializeUiHandler(UiHandler);

        // Инициализация остальной части игры
        GameMenu = new GameMenu(this);
        _gameStateSaver = new GameStateSaver();

        // Установка начальных значений для имени игрока и текущей фигуры
        PlayerName = "Player"; // Установим имя по умолчанию, которое можно будет изменить позже
        Current = Field.CurrentFigure; // Установим текущую фигуру из GameField
    }
    


    // Начать новую игру, запрашивая имя игрока.
    public void StartNewGame()
    {
        Console.WriteLine("Enter your name: ");
        var name = Console.ReadLine();
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name; 

        Field = new GameField(UiHandler);
        Current = Field.CurrentFigure; 
        UiHandler = new UiHandler(Field.Cells, Field.CurrentFigure, GameField.Height, GameField.Width);
        Field.InitializeUiHandler(UiHandler);
        PlayGame(PlayerName).Wait(); 
    }



    // Сохранить текущее игровое состояние.
    public void SaveGame()
    {
        if (Current == null || PlayerName == null) // Проверка наличия текущей фигуры и имени игрока, сохранение состояния и вывод сообщения.

        {
            Console.WriteLine("Cannot save game: Current figure or player name is not set.");
            return;
        }

        var gameState = new GameState(Field, Current, LastMoveDown, Field.Score, PlayerName);
        _gameStateSaver.SaveGame(gameState);
        Console.WriteLine("Game saved successfully.");
    }



    // Загрузить ранее сохраненное игровое состояние.
    public void LoadGame()
    {
        var gameState = _gameStateSaver.LoadGame();
        if (gameState != null)
        {
            Field = gameState.Field;
            Current = gameState.Current;
            LastMoveDown = gameState.LastMoveDown;
            Field.SetScore(gameState.Score);
            UiHandler = new UiHandler(Field.Cells, Field.CurrentFigure, GameField.Height, GameField.Width);
            Field.InitializeUiHandler(UiHandler);
            PlayerName = gameState.PlayerName; 
            Console.WriteLine("Game loaded successfully.");
            PlayGame(PlayerName).Wait(); 
        }
    }



    // Обработка пользовательского ввода (нажатий клавиш).
    public void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                Field.MoveFigureLeft(Field.CurrentFigure);
                break;
            case ConsoleKey.RightArrow:
                Field.MoveFigureRight(Field.CurrentFigure);
                break;
            case ConsoleKey.DownArrow:
                Field.MoveFigureDown(Field.CurrentFigure);
                LastMoveDown = DateTime.Now; // Обновляем время последнего смещения
                break;
            case ConsoleKey.UpArrow:
                Field.RotateFigure(Field.CurrentFigure);
                break;
            case ConsoleKey.Z: // Обработка клавиши Z для вызова внутриигрового меню
                if (GameMenu.InGameMenu())
                {
                    throw new ExitToMainMenuException(); // Возвращаемся в главное меню
                }

                break;

        }
    }

    // Отрисовка текущего состояния игры.
    public void Render()
    {
        UiHandler.Update(Field.Cells, Field.CurrentFigure, GameField.Height, GameField.Width);
        UiHandler.Render(Field);

    }
    
    // Основной игровой цикл.
    public async Task PlayGame(string playerName)
    {
        Console.Clear();
        try
        {
            while (true)   // Цикл обработки игровых событий, ввода и автоматического смещения фигур.

            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    HandleInput(key.Key);
                    if (key.Key == ConsoleKey.Z)
                    {
                        if (GameMenu.InGameMenu())
                        {
                            var scoreBoard = new ScoreBoard();
                            scoreBoard.AddOrUpdatePlayerScore(playerName, Field.Score);
                            scoreBoard.DisplayScores();
                            return; // Вернуться в главное меню
                        }
                    }
                }

                if ((DateTime.Now - LastMoveDown).TotalMilliseconds >= MoveDownIntervalMs)
                {
                    Field.MoveFigureDown(Field.CurrentFigure);
                    LastMoveDown = DateTime.Now;
                }

                Render();
                await Task.Delay(30);
            }
        }
        catch (ExitToMainMenuException)
        {

        }
        catch (GameOverException e)         // Обработка исключений при завершении игры.

        {
            Console.Clear();
            Console.WriteLine($"Game over! Your score: {e.Score}");
            var scoreBoard = new ScoreBoard();
            scoreBoard.AddOrUpdatePlayerScore(playerName, e.Score);
            scoreBoard.DisplayScores();
        }
        catch (IndexOutOfRangeException)
        {
            Console.Clear();
            Console.WriteLine($"Game over! Your score: {Field.Score}");
            var scoreBoard = new ScoreBoard();
            scoreBoard.AddOrUpdatePlayerScore(playerName, Field.Score);
            scoreBoard.DisplayScores();
        }
    }


    public void GameLoop()    // Главный цикл игры, управляющий меню и игровым процессом.

    {
        bool isRunning = true;
        while (isRunning) // цикл, пока isRunning истинно
        {
            isRunning = GameMenu.MainMenu(); // MainMenu возвращает false, если нужно выйти
        }
    }


    


 
}