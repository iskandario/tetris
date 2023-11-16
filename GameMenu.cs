using TETRIS.DataAccess;
using TETRIS.BusinessLogic;
namespace TETRIS.Presentation;

public class GameMenu
{
    private readonly Game _game;

    // конструктор класса принимает экземпляр игры
    public GameMenu(Game game)
    {
        this._game = game;
    }

    // представляет меню во время игры
    public bool InGameMenu()
    {
        bool inMenu = true;
        bool isPaused = false;
       
        // цикл меню
        while (inMenu)
        {
            // отображение меню, если игра не на паузе
            if (!isPaused)
            {
                Console.WriteLine("In-Game Menu:");
                Console.WriteLine("1. Restart Game");
                Console.WriteLine("2. Resume Game");
                Console.WriteLine("3. Save Game");
                Console.WriteLine("4. Load Game");
                Console.WriteLine("5. ScoreBoard");
                Console.WriteLine("6. Exit to Main Menu");
                Console.Write("Enter your choice: ");
            }

            string? choice = Console.ReadLine();

            // обработка выбора пользователя
            if (!isPaused)
            {
                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        _game.RestartGame();
                        inMenu = false;
                        break;
                    case "2":
                        Console.Clear();
                        _game.PlayGame(_game.PlayerName).Wait();
                        return false;
                    case "3":
                    
                        Console.Clear();
                        _game.SaveGame();
                        Console.WriteLine("Game saved successfully. Press any key to return to the menu...");
                        Console.ReadKey();
                        return true;
                    case "4":
                        Console.Clear();
                        _game.LoadGame();
                        inMenu = false;
                        break;
                    case "5":
                        Console.Clear();
                        DisplayScoreBoard();
                        break;
                    case "6":
                        Console.Clear();
                        return true;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid choice. Please choose again.");
                        break;
                }
            }
            else
            {
                // обработка паузы
                if (choice == "2")
                {
                    isPaused = false; // продолжение игры
                    Console.Clear();
                }
            }
        }

        return false;
    }

    // представляет главное меню игры
    public bool MainMenu()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("d888888b d88888b d888888b d8888b. d888888b .d8888. \n`~~88~~' 88'     `~~88~~' 88  `8D   `88'   88'  YP \n   88    88ooooo    88    88oobY'    88    `8bo.   \n   88    88~~~~~    88    88`8b      88      `Y8b. \n   88    88.        88    88 `88.   .88.   db   8D \n   YP    Y88888P    YP    88   YD Y888888P `8888Y' ");
        Console.ResetColor();
        Console.WriteLine("Main Menu:");
        Console.WriteLine("1. Start New Game");
        Console.WriteLine("2. Load Game");
        Console.WriteLine("3. ScoreBoard");
        Console.WriteLine("4. Exit");
        Console.Write("Enter your choice: ");
        string? choice = Console.ReadLine();

        // обработка ввода пользователя
        if (choice == null)
        {
            Console.WriteLine("Input not provided. Please enter your choice: ");
            return MainMenu(); // повторный вызов при отсутствии ввода
        }

        // действия в зависимости от выбора пользователя
        switch (choice)
        {
            case "1":
                _game.StartNewGame();
                break;
            case "2":
                _game.LoadGame();
                break;
            case "3":
                Console.Clear();
                DisplayScoreBoard();
                break;
            case "4":
                Environment.Exit(0);
                break;
            default:
                Console.Clear();
                Console.WriteLine("Invalid choice. Please choose again.");
                return MainMenu(); // повторный вызов при неверном выборе
        }

        return true;
    }

    // отображает таблицу лидеров
    private void DisplayScoreBoard()
    {
        ScoreBoard scoreBoard = new ScoreBoard(); 
        scoreBoard.DisplayScores();
        Console.WriteLine("Press any key to return to the menu...");
        Console.ReadKey(); // Ожидание нажатия клавиши
    }
}
 
   
   