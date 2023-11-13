using TETRIS.BusinessLogic;
using Newtonsoft.Json;

namespace TETRIS.DataAccess
{
    public class ScoreBoard
    {
        // список игроков для таблицы лидеров
        private List<Player> _players = new List<Player>();
        // путь к файлу с таблицей лидеров
        private string _filePath = "Assets/scores.json"; 

        public ScoreBoard()
        {
            LoadFromFile(); 
            SaveToFile();
        }

        // добавление или обновление рекорда игрока
        public void AddOrUpdatePlayerScore(string name, int score)
        {
            // поиск существующего игрока в списке
            var existingPlayer = _players.FirstOrDefault(p => p.Name == name);

            // если игрока нет в списке, добавляем его
            if (existingPlayer == null)
            {
                _players.Add(new Player(name, score));
            }
            else
            {
                // если игрок уже есть, обновляем его счёт
                existingPlayer.Score = Math.Max(existingPlayer.Score, score);
            }

            // сортировка списка игроков по убыванию очков
            _players = _players.OrderByDescending(p => p.Score).ToList();

            // ограничение списка до 10 игроков
            if (_players.Count > 10)
            {
                _players = _players.Take(10).ToList(); 
            }

            // сохранение списка в файл
            SaveToFile();
        }

        // метод для сохранения списка игроков в файл
        public void SaveToFile()
        {
            // сериализация списка игроков в json
            var jsonData = JsonConvert.SerializeObject(_players, Formatting.Indented); 
            // запись json в файл
            File.WriteAllText(_filePath, jsonData);
        }

        // метод для загрузки списка игроков из файла
        public void LoadFromFile()
        {
            // проверка наличия файла
            if (File.Exists(_filePath))
            {
                // чтение данных из файла
                var json = File.ReadAllText(_filePath);
                // десериализация данных в список игроков
                _players = JsonConvert.DeserializeObject<List<Player>>(json) ?? new List<Player>();
            }
        }

        // метод для отображения таблицы лидеров
        public void DisplayScores()
        {
            Console.WriteLine("Scoreboard:");
            foreach (var player in _players)
            {
                Console.WriteLine($"{player.Name}: {player.Score}");
            }
        }
    }
}
