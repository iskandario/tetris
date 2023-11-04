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


        public void SetScore(int score)
        {
            this.Score = score;
        }


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

            // Инициализируйте CurrentFigure и NextFigure сразу.
            NextFigure = GetRandomFigure();
            CurrentFigure = GetRandomFigure();
            SpawnNewFigure();
        }


        public void InitializeUiHandler(UiHandler handler)
        {
            this._uiHandler = handler;
        }

        public bool[,] ToBoolArray()
        {
            bool[,] array = new bool[Width, Height];
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    array[x, y] = Cells[x, y].IsFilled;
                }
            }

            return array;
        }



        public void MoveFigureLeft(Figure figure)
        {
            figure.X--;
            if (!FigureCanSpawn(figure))
            {
                figure.X++;
            }
        }

        public void MoveFigureRight(Figure figure)
        {
            figure.X++;
            if (!FigureCanSpawn(figure))
            {
                figure.X--;
            }
        }

        public void MoveFigureDown(Figure figure)
        {
            Figure newFigure = figure.Clone();
            newFigure.Y++;
            if (FigureCanSpawn(newFigure))
            {
                CurrentFigure = newFigure;
            }
            else
            {
                FixFigure(figure);
                CheckLines();
                _uiHandler.Update(Cells, CurrentFigure, Height, Width);
                _uiHandler.Render(this);
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

                if (FigureCanSpawn(newFigure))
                {
                    CurrentFigure = newFigure;
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

        public Figure GetRandomFigure()
        {
            int randomIndex = _rnd.Next(0, Figure.AllFigures.Count);
            Figure newFigure = Figure.AllFigures[randomIndex].Clone();
            newFigure.X = Width / 2;
            newFigure.Y = -2;

            return newFigure;
        }

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
                        Cells[figure.X + x, figure.Y + y].IsFilled = true;
                        Cells[figure.X + x, figure.Y + y].Color = figure.Color;
                    }
                }
            }
        }

        private bool FigureCanSpawn(Figure figure)
        {
            for (int x = 0; x < figure.SizeX; x++)
            {
                for (int y = 0; y < figure.SizeY; y++)
                {
                    if (figure.Shape[y, x]) // Убрано сравнение с true
                    {
                        if (figure.X + x < 0 || figure.X + x >= Width || figure.Y + y >= Height)
                        {
                            return false;
                        }

                        if (figure.Y + y >= 0 && Cells[figure.X + x, figure.Y + y].IsFilled)
                        {
                            return false; // Ячейка, которую фигура собирается занять, уже заполнена
                        }
                    }
                }
            }
            return true; // Все ячейки, которые фигура собирается занять, свободны
        }


    }
