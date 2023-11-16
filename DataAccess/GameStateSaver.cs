using Newtonsoft.Json;

namespace TETRIS.DataAccess
{
    public class GameStateSaver
    {
        // путь к файлу сохранения
        private string _filePath = "/Users/iskandargarifullin/RiderProjects/tetrisok/Assets/gameField.json";

        // метод для сохранения состояния игры
        public bool SaveGame(GameState gameState)
        {
            try
            {
                // сериализация состояния игры в json
                var json = JsonConvert.SerializeObject(gameState);
                // запись json в файл
                File.WriteAllText(_filePath, json);
                
                return true; 
            }
            catch
            {
               
                return false;
            }
        }

        // метод для загрузки сохраненного состояния игры
        public GameState? LoadGame()
        {
            // проверка существования файла сохранения
            if (!File.Exists(_filePath))
            {
                return null; 
            }

            try
            {
                // чтение json из файла
                var json = File.ReadAllText(_filePath);
                // десериализация json в объект GameState
                var gameState = JsonConvert.DeserializeObject<GameState>(json);
                
                return gameState; 
            }
            catch
            {
                
                return null;
            }
        }
    }
}