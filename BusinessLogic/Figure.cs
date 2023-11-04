namespace TETRIS.BusinessLogic;

public class Figure
    {
        public int SizeX
        {
            get { return Shape.GetLength(1); } // Принимаем размер как длину внешнего измерения массива Shape
        }

        public int SizeY
        {
            get { return Shape.GetLength(0); } // Принимаем размер как длину внешнего измерения массива Shape
        }

        public int BorderSizeX
        {
            get
            {
                for (int x = SizeX - 1; x >= 0; x--)
                {
                    for (int y = 0; y < SizeY; y++)
                    {
                        if (Shape[y, x]) return x + 1;
                    }
                }

                return 0;
            }
        }

        public int BorderSizeY
        {
            get
            {
                for (int y = SizeY - 1; y >= 0; y--)
                {
                    for (int x = 0; x < SizeX; x++)
                    {
                        if (Shape[y, x]) return y + 1;
                    }
                }

                return 0;
            }
        }




        public int X { get; set; }
        public int Y { get; set; }
        public bool[,] Shape { get; set; }
        public string Color { get; set; }

        public Figure(int x, int y, bool[,] shape, string color)
        {
            X = x;
            Y = y;
            Shape = shape;
            Color = color;
        }

        public static List<Figure> AllFigures = new List<Figure>
        {
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { false, false, false, false },
                { true, true, true, true },
                { false, false, false, false },
                { false, false, false, false }
            }, "red"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { false, true, false },
                { true, true, true },
                { false, false, false }

            }, "blue"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { true, true },
                { true, true }

            }, "orange"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { true, false, false },
                { true, true, true },
                { false, false, false },

            }, "green"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { false, false, true },
                { true, true, true },
                { false, false, false },

            }, "yellow"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { false, true, true },
                { true, true, false },
                { false, false, false },

            }, "purple"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { true, true, false },
                { false, true, true },
                { false, false, false },

            }, "pink")

        };

        public Figure Clone()
        {
            bool[,] newShape = new bool[Shape.GetLength(0), Shape.GetLength(1)];
            Array.Copy(Shape, newShape, Shape.Length);

            return new Figure(X, Y, newShape, Color);
        }

        public bool CanRotateRight(bool[,] filledCells)
        {
            int newSizeX = SizeY;
            int newSizeY = SizeX;
            bool[,] newShape = new bool[newSizeY, newSizeX];

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    newShape[x, SizeY - 1 - y] = Shape[y, x];
                }
            }


            return CheckPositionValid(newShape, X, Y, filledCells);
        }

        private bool CheckPositionValid(bool[,] shape, int shapeX, int shapeY, bool[,] filledCells)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int boardX = shapeX + x;
                    int boardY = shapeY + y;

                    // Проверяем, что клетка находится в рамках поля
                    if (boardX < 0 || boardX >= filledCells.GetLength(0) || boardY >= filledCells.GetLength(1))
                    {
                        return false;
                    }

                    // Пропускаем строки, которые еще не вошли в игровое поле
                    if (boardY < 0)
                    {
                        continue;
                    }

                    // Проверяем, что фигура не пересекается с другими заполненными клетками
                    if (shape[y, x] && filledCells[boardX, boardY])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public Figure RotateRight()
        {
            int newSizeX = SizeY;
            int newSizeY = SizeX;
            bool[,] newShape = new bool[newSizeY, newSizeX];

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    newShape[x, SizeY - 1 - y] = Shape[y, x];
                }
            }

            return new Figure(X, Y, newShape, Color);
        }
    }
