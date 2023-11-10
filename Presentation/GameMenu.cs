using TETRIS.DataAccess;
using TETRIS.BusinessLogic;
namespace TETRIS.Presentation;


     public class GameMenu
    {
        private readonly Game _game;

        // Конструктор класса GameMenu, принимающий экземпляр игры.
        public GameMenu(Game game)
        {
            this._game = game;
        }

        // Метод InGameMenu представляет меню, отображаемое во время игры.
        public bool InGameMenu()
        {
            Console.WriteLine("In-Game Menu:");
            Console.WriteLine("1. Resume Game");
            Console.WriteLine("2. Save Game");
            Console.WriteLine("3. Load Game");
            Console.WriteLine("4. ScoreBoard");
            Console.WriteLine("5. Exit to Main Menu");
            Console.Write("Enter your choice: ");

            string? choice = Console.ReadLine();

            if (choice == null)
            {
                Console.WriteLine("Input not provided. Please enter your choice: ");
                return InGameMenu(); // Рекурсивный вызов, если ввод отсутствует
            }

            switch (choice)
            {
                case "1":
                    return false; // Продолжить игру
                case "2":
                    _game.SaveGame();
                    return false; // Сохранить игру
                case "3":
                    _game.LoadGame();
                    return false; // Загрузить игру
                case "4":
                    DisplayScoreBoard();
                    return false; // Показать таблицу лидеров
                case "5":
                    return true; // Вернуться в главное меню
                default:
                    Console.WriteLine("Invalid choice. Please choose again.");
                    return InGameMenu(); // Рекурсивный вызов при недопустимом выборе
            }
        }

        // Метод MainMenu представляет главное меню игры.
        public bool MainMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(@"

████████╗███████╗████████╗██████╗ ██╗███████╗
╚══██╔══╝██╔════╝╚══██╔══╝██╔══██╗██║██╔════╝
   ██║   █████╗     ██║   ██████╔╝██║███████╗
   ██║   ██╔══╝     ██║   ██╔══██╗██║╚════██║
   ██║   ███████╗   ██║   ██║  ██║██║███████║
   ╚═╝   ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚══════╝");
            Console.ResetColor();
            Console.WriteLine("Main Menu:");
            Console.WriteLine("1. Start New Game");
            Console.WriteLine("2. Load Game");
            Console.WriteLine("3. ScoreBoard");
            Console.WriteLine("4. Exit");
            Console.Write("Enter your choice: ");
            string? choice = Console.ReadLine();

            if (choice == null)
            {
                Console.WriteLine("Input not provided. Please enter your choice: ");
                MainMenu(); // Рекурсивный вызов, если ввод отсутствует
            }

            switch (choice)
            {
                case "1":
                    _game.StartNewGame();
                    break;
                case "2":
                    _game.LoadGame();
                    break;
                case "3":
                    DisplayScoreBoard();
                    break;
                case "4":
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please choose again.");
                    MainMenu(); // Рекурсивный вызов при недопустимом выборе
                    break;
            }

            return true;
        }

        // Метод для отображения таблицы лидеров.
        private void DisplayScoreBoard()
        {
            ScoreBoard scoreBoard = new ScoreBoard();
            scoreBoard.LoadFromFile();
            scoreBoard.DisplayScores();
        }
    }
