using TETRIS.Presentation;
using TETRIS.Exceptions;
namespace TETRIS.BusinessLogic;

public class GameField
    {
        public const int Width = 10;
        public const int Height = 20;
        public Cell[,] Cells;
        private UiHandler _uiHandler;
        
        
        public int Score { get; set; } 
        public Figure NextFigure { get; private set; } 
        public Figure CurrentFigure { get; private set; }

        // Метод для установки счета.
        public void SetScore(int score)
        {
            this.Score = score;
        }

        // Конструктор, инициализирующий игровое поле.
        public GameField(UiHandler uiHandler)
        {
            this._uiHandler = uiHandler;
            Cells = new Cell[Width, Height];
            for (var y = 0; y < Height; y++)
            {
                for (var x = 0; x < Width; x++)
                {
                    Cells[x, y] = new Cell(x, y, false, "Empty");
                }
            }

            NextFigure = GetRandomFigure();
            CurrentFigure = GetRandomFigure();
            SpawnNewFigure();
        }

        // Метод для связи с объектом обработчика пользовательского интерфейса.
        public void InitializeUiHandler(UiHandler handler)
        {
            this._uiHandler = handler;
        }
        
        // Метод для преобразования состояния игрового поля в двумерный массив булевых значений.
        public bool[,] ToBoolArray()
        {
            bool[,] array = new bool[Width, Height];
            for (int x = 0; x < Width; x++)   // Преобразование игрового поля в двумерный массив булевых значений.

            {
                for (int y = 0; y < Height; y++)
                {
                    array[x, y] = Cells[x, y].IsFilled;
                }
            }

            return array;
        }


        // Метод для смещения фигуры влево.
        public void MoveFigureLeft(Figure figure)
        {
            figure.X--;
            if (!FigureCanSpawn(figure))
            {
                figure.X++;
            }
        }

        // Метод для смещения фигуры вправо.
        public void MoveFigureRight(Figure figure)
        {
            figure.X++;
            if (!FigureCanSpawn(figure))
            {
                figure.X--;
            }
        }

        
        // Метод для смещения фигуры вниз.
        public void MoveFigureDown(Figure figure)
        {
            Figure newFigure = figure.Clone();
            newFigure.Y++;
            if (FigureCanSpawn(newFigure))
            {
                CurrentFigure = newFigure;
            }
            else   // Смещение фигуры вниз и обработка возможных событий при достижении дна.

            {
                FixFigure(figure);
                CheckLines();
                _uiHandler.Update(Cells, CurrentFigure, Height, Width);
                _uiHandler.Render(this);
                SpawnNewFigure();
            }

        }
        
        // Метод для вращения фигуры.
        public void RotateFigure(Figure figure)
        {
            if (figure.CanRotateRight(ToBoolArray()))
            {
                Figure newFigure = figure.RotateRight();

                if (newFigure.X + newFigure.SizeX > Width)
                {
                    newFigure.X = Width - newFigure.SizeX;
                }
                else if (newFigure.X < 0)
                {
                    newFigure.X = 0;
                }

                if (FigureCanSpawn(newFigure))
                {
                    CurrentFigure = newFigure;
                }
            }
        }


        
        // Метод для проверки и удаления заполненных строк на игровом поле.
        private void CheckLines()
        {
            for (int y = 0; y < GameField.Height; y++)
            {
                bool lineIsFilled = true;
                for (int x = 0; x < GameField.Width; x++)
                {
                    if (!Cells[x, y].IsFilled)
                    {
                        lineIsFilled = false;
                        break;
                    }
                }

                if (lineIsFilled)
                {
                    DeleteLine(y);
                    Score += 100; // увеличиваем счёт, когда строка удалена
                }
            }
        }

        
        // Метод для удаления конкретной строки на игровом поле.
        private void DeleteLine(int line)
        {
            for (int y = line; y > 0; y--)
            {
                for (int x = 0; x < GameField.Width; x++)
                {
                    Cells[x, y].IsFilled = Cells[x, y - 1].IsFilled;
                    Cells[x, y].Color = Cells[x, y - 1].Color;
                }
            }

            for (int x = 0; x < GameField.Width; x++)
            {
                // Очищаем самую верхнюю строку
                Cells[x, 0].IsFilled = false;
                Cells[x, 0].Color = "white";
            }
        }

        private Random _rnd = new Random();

        
        // Метод для получения случайной фигуры.
        public Figure GetRandomFigure()
        {
            int randomIndex = _rnd.Next(0, Figure.AllFigures.Count);
            Figure newFigure = Figure.AllFigures[randomIndex].Clone();
            newFigure.X = Width / 2 - 2;
            newFigure.Y = 1;

            return newFigure;
        }

        
        
        // Метод для спавна новой фигуры на игровом поле.
        public void SpawnNewFigure()
        {
        

            if (FigureCanSpawn(NextFigure))
            {
                CurrentFigure = NextFigure;
                NextFigure = GetRandomFigure(); // генерируем новую следующую фигуру после спавна текущей
            }
            else
            {
                throw new GameOverException(Score);
            }
        }


        
        // Метод для "зафиксирования" фигуры на игровом поле.

        public void FixFigure(Figure figure)
        {
            for (int x = 0; x < figure.SizeX; x++)
            {
                for (int y = 0; y < figure.SizeY; y++)
                {
                    if (figure.Shape[y, x])
                    {
                        // Заполняем соответствующую клетку на игровом поле. 
                        Cells[figure.X + x, figure.Y + y].IsFilled = true;
                        Cells[figure.X + x, figure.Y + y].Color = figure.Color;
                    }
                }
            }
        }

        
        // Метод для проверки, может ли фигура быть размещена на игровом поле.
        private bool FigureCanSpawn(Figure figure)
        {
            for (int x = 0; x < figure.SizeX; x++)
            {
                for (int y = 0; y < figure.SizeY; y++)
                {
                    if (figure.Shape[y, x]) 
                    {
                        if (figure.X + x < 0 || figure.X + x >= Width || figure.Y + y >= Height)
                        {
                            return false;
                        }

                        if (figure.Y + y >= 0 && Cells[figure.X + x, figure.Y + y].IsFilled)
                        {
                            return false; 
                        }
                    }
                }
            }
            return true; // Все ячейки, которые фигура собирается занять, свободны
        }


    }
