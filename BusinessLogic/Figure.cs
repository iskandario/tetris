namespace TETRIS.BusinessLogic;

public class Figure(int x, int y, bool[,] shape, string color)
{
        public int SizeX
        {
            get { return Shape.GetLength(1); } 
        }

        public int SizeY
        {
            get { return Shape.GetLength(0); } 
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




        public int X { get; set; } = x;
        public int Y { get; set; } = y;
        public bool[,] Shape { get; set; } = shape;
        public string Color { get; set; } = color;


        public static List<Figure> AllFigures = new List<Figure>
        {
            new Figure(GameField.Width / 2 - 1, -3, new [,]
            {
                { false, false, true, false },
                { false, false, true, false},
                { false, false, true, false },
                { false, false, true, false }
            }, "red"),


            
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                
                { false, true, false},
                { true, true, true},
                { false, false, false}

            }, "blue"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { true, false, false},
                { true, true, true},
                { false, false, false}
            }, "orange"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
              
                { false, false, true},
                { true, true, true},
                { false, false, false}

            }, "green"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                
                { true, true, false},
                { false, true, true},
                { false, false, false}

            }, "yellow"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
               
                { false, true, true},
                { true, true, false},
                { false, false, false}

            }, "purple"),
            new Figure(GameField.Width / 2 - 2, -2, new [,]
            {
                { false, true, true, false },
                { false, true, true, false },
                { false, false, false, false }

            }, "pink")

        };
        
        // клонирует текущую фигуру, создавая новый экземпляр с идентичной формой и цветом
        public Figure Clone()
        {
            bool[,] newShape = new bool[Shape.GetLength(0), Shape.GetLength(1)];
            Array.Copy(Shape, newShape, Shape.Length);

            return new Figure(X, Y, newShape, Color);
        }
        
        
        // проверяет, можно ли повернуть фигуру влево на текущей позиции
        public bool CanRotateLeft(bool[,] filledCells)
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


        
        // метод, проверяющий, что фигура может быть размещена в указанной позиции
        private bool CheckPositionValid(bool[,] shape, int shapeX, int shapeY, bool[,] filledCells)
        {
            for (int y = 0; y < shape.GetLength(0); y++)
            {
                for (int x = 0; x < shape.GetLength(1); x++)
                {
                    int boardX = shapeX + x;
                    int boardY = shapeY + y;

                    // проверяем, что клетка находится в рамках поля
                    if (boardX < 0 || boardX >= filledCells.GetLength(0) || boardY >= filledCells.GetLength(1))
                    {
                        return false;
                    }

                    // пропускаем строки, которые еще не вошли в игровое поле
                    if (boardY < 0)
                    {
                        continue;
                    }

                    // проверяем, что фигура не пересекается с другими заполненными клетками
                    if (shape[y, x] && filledCells[boardX, boardY])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // метод который поворачивает текущую фигуру влево и возвращает новую фигуру 
        public Figure RotateLeft()
        {
            int newSizeY = SizeX;
            int newSizeX = SizeY;
            bool[,] newShape = new bool[newSizeY, newSizeX];

            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < SizeX; x++)
                {
                    newShape[x, SizeY - 1 - y] = Shape[y, x];
                }
            }

            Figure rotatedFigure = new Figure(X, Y, newShape, Color);

            return rotatedFigure;
        }
        

    }
