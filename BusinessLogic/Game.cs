using TETRIS.Presentation;
using TETRIS.Exceptions;
using TETRIS.DataAccess;
namespace TETRIS.BusinessLogic;

public class Game
{
    private GameField _field;
    private Figure? _current;
    private DateTime _lastMoveDown = DateTime.Now;
    private const int MoveDownIntervalMs = 300;
    private UiHandler _uiHandler;
    private readonly GameMenu _gameMenu;
    public string PlayerName;
    private readonly GameStateSaver _gameStateSaver;
    


    public Game()
    {
        // Создание временной фигуры с минимально допустимым массивом
        bool[,] tempShape = new bool[,] { { true } };
        Figure tempFigure = new Figure(0, 0, tempShape, "TemporaryColor");

        // Создание временного UiHandler с временным объектом Figure
        UiHandler tempUiHandler = new UiHandler(new Cell[GameField.Width, GameField.Height], tempFigure,
            GameField.Height, GameField.Width);

        // Инициализация GameField с временным UiHandler
        _field = new GameField(tempUiHandler);

        // Создание и инициализация реального UiHandler теперь, когда Cells и CurrentFigure инициализированы
        _uiHandler = new UiHandler(_field.Cells, _field.CurrentFigure, GameField.Height, GameField.Width);

        // Обновление UiHandler внутри GameField
        _field.InitializeUiHandler(_uiHandler);

        // Инициализация остальной части игры
        _gameMenu = new GameMenu(this);
        _gameStateSaver = new GameStateSaver();

        // Установка начальных значений для имени игрока и текущей фигуры
        PlayerName = "Player"; // Установим имя по умолчанию, которое можно будет изменить позже
        _current = _field.CurrentFigure; // Установим текущую фигуру из GameField
    }


    

    // Начать новую игру, запрашивая имя игрока.
    public void StartNewGame()
    {
        Console.WriteLine("Enter your name: ");
        var name = Console.ReadLine();
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name; 

        _field = new GameField(_uiHandler);
        _current = _field.CurrentFigure; 
        _uiHandler = new UiHandler(_field.Cells, _field.CurrentFigure, GameField.Height, GameField.Width);
        _field.InitializeUiHandler(_uiHandler);
        PlayGame(PlayerName).Wait(); 
    }

    
    
    public void RestartGame()
    {
        var name = this.PlayerName;
        PlayerName = string.IsNullOrWhiteSpace(name) ? "Player" : name; 
        _field = new GameField(_uiHandler);
        _current = _field.CurrentFigure; 
        _uiHandler = new UiHandler(_field.Cells, _field.CurrentFigure, GameField.Height, GameField.Width);
        _field.InitializeUiHandler(_uiHandler);
        PlayGame(PlayerName).Wait(); 

    }



    // Сохранить текущее игровое состояние.
    public void SaveGame()
    {
        if (_current == null ) // Проверка наличия текущей фигуры и имени игрока, сохранение состояния и вывод сообщения.

        {
            Console.WriteLine("Cannot save game: Current figure or player name is not set.");
            return;
        }

        var gameState = new GameState(_field, _current, _lastMoveDown, _field.Score, PlayerName);
        _gameStateSaver.SaveGame(gameState);
        Console.WriteLine("Game saved successfully.");
    }



    // Загрузить ранее сохраненное игровое состояние.
    public void LoadGame()
    {
        var gameState = _gameStateSaver.LoadGame();
        if (gameState != null)
        {
            _field = gameState.Field;
            _current = gameState.Current;
            _lastMoveDown = gameState.LastMoveDown;
            _field.SetScore(gameState.Score);
            _uiHandler = new UiHandler(_field.Cells, _field.CurrentFigure, GameField.Height, GameField.Width);
            _field.InitializeUiHandler(_uiHandler);
            PlayerName = gameState.PlayerName; 
            Console.WriteLine("Game loaded successfully.");
            PlayGame(PlayerName).Wait(); 
        }
    }



    // Обработка пользовательского ввода (нажатий клавиш).
    private void HandleInput(ConsoleKey key)
    {
        switch (key)
        {
            case ConsoleKey.LeftArrow:
                _field.MoveFigureLeft(_field.CurrentFigure);
                break;
            case ConsoleKey.RightArrow:
                _field.MoveFigureRight(_field.CurrentFigure);
                break;
            case ConsoleKey.DownArrow:
                _field.MoveFigureDown(_field.CurrentFigure);
                _lastMoveDown = DateTime.Now; // Обновляем время последнего смещения
                break;
            case ConsoleKey.UpArrow:
                _field.RotateFigure(_field.CurrentFigure);
                break;
            case ConsoleKey.Spacebar: // Обработка клавиши Z для вызова внутриигрового меню
                if (_gameMenu.InGameMenu())
                {
                    throw new ExitToInGameMenuException(); // Возвращаемся в главное меню
                }

                break;

        }
    }

    // Отрисовка текущего состояния игры.
    private void Render()
    {
        _uiHandler.Update(_field.Cells, _field.CurrentFigure, GameField.Height, GameField.Width);
        _uiHandler.Render(_field);

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
                        if (_gameMenu.InGameMenu())
                        {
                            var scoreBoard = new ScoreBoard();
                            scoreBoard.AddOrUpdatePlayerScore(playerName, _field.Score);
                            scoreBoard.DisplayScores();
                            return;
                        }
                    }
                }

                if ((DateTime.Now - _lastMoveDown).TotalMilliseconds >= MoveDownIntervalMs)
                {
                    _field.MoveFigureDown(_field.CurrentFigure);
                    _lastMoveDown = DateTime.Now;
                }

                Render();
                await Task.Delay(30);
            }
        }
        catch (ExitToInGameMenuException)
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
            Console.WriteLine($"Game over! Your score: {_field.Score}");
            var scoreBoard = new ScoreBoard();
            scoreBoard.AddOrUpdatePlayerScore(playerName, _field.Score);
            scoreBoard.DisplayScores();
        }
    }


    public void GameLoop()    // Главный цикл игры, управляющий меню и игровым процессом.

    {
        bool isRunning = true;
        while (isRunning) // цикл, пока isRunning истинно
        {
            isRunning = _gameMenu.MainMenu(); // MainMenu возвращает false, если нужно выйти
        }
    }


    


 
}