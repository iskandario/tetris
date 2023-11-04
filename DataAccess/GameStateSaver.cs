using Newtonsoft.Json;
using System;
using System.IO;

namespace TETRIS.DataAccess
{
    public class GameStateSaver
    {
        private readonly string _filePath;

        // Если не передаем путь в конструкторе, используем стандартное местоположение
        public GameStateSaver()
        {
            _filePath = "/Users/iskandargarifullin/RiderProjects/TETRIS/TETRIS/Assets/gameField.json";
        }

        public GameStateSaver(string filePath)
        {
            _filePath = filePath;
        }

        public void SaveGame(GameState gameState)
        {
            try
            {
                var json = JsonConvert.SerializeObject(gameState);
                File.WriteAllText(_filePath, json);
                Console.WriteLine("Saved game state:");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось сохранить игру. Ошибка: {e.Message}");
            }
        }

        public GameState? LoadGame()
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine("Файл сохранения не найден.");
                return null;
            }

            try
            {
                var json = File.ReadAllText(_filePath);
                var gameState = JsonConvert.DeserializeObject<GameState>(json);
                Console.WriteLine("Loaded game state:");
                return gameState;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Не удалось загрузить игру. Ошибка: {e.Message}");
                return null;
            }
        }
    }
}