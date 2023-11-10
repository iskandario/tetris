using TETRIS.BusinessLogic;
using Newtonsoft.Json;


namespace TETRIS.DataAccess
{
    public class ScoreBoard
    {
        private List<Player> _players = new List<Player>();
        private string _filePath = "/Users/iskandargarifullin/RiderProjects/TETRIS/TETRIS/Assets/scores.json"; // Путь и имя файла JSON

        // Метод для добавления или обновления счета игрока.
        public void AddOrUpdatePlayerScore(string name, int score)
        {
            var existingPlayer = _players.FirstOrDefault(p => p.Name == name);

            if (existingPlayer == null)
            {
                _players.Add(new Player(name, score));
            }
            else
            {
                existingPlayer.Score = Math.Max(existingPlayer.Score, score);
            }

            _players = _players.OrderByDescending(p => p.Score).ToList();

            if (_players.Count > 10) // Если количество игроков в таблице больше 10, удаляем лишние записи.
            {
                _players.RemoveAt(10);
            }

            SaveToFile();
        }

        // Метод для сохранения данных в файл JSON.
        public void SaveToFile()
        {
            var jsonData = JsonConvert.SerializeObject(_players);
            File.WriteAllText(_filePath, jsonData);
        }

        // Метод для загрузки данных из файла JSON.
        public void LoadFromFile()
        {
            if (!File.Exists(_filePath)) return;

            var json = File.ReadAllText(_filePath);
            _players = JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
        }

        // Метод для отображения таблицы рекордов в консоли.
        public void DisplayScores()
        {
            Console.WriteLine("Scoreboard:");
            foreach (var player in _players)
            {
                Console.WriteLine($"{player.Name}: {player.Score}");
            }
        }

        // Метод для загрузки списка игроков из файла при создании экземпляра класса.
        public ScoreBoard()
        {
            if (File.Exists(_filePath))
            {
                var jsonData = File.ReadAllText(_filePath);
                _players = JsonConvert.DeserializeObject<List<Player>>(jsonData) ?? new List<Player>();
            }
        }
    }
}
