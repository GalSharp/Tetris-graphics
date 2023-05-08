using System;

namespace Tetris
{
    public class Shape // Наш класс фигуры
    {
        public int x; // Позиция нашей фигуры по X
        public int y; // Позиция нашей фигуры по Y

        public int[,] matrix; // Матрица в которой будет храниться наша фигура (одна из)
        public int[,] nextMatrix; // Матрица в которой генерируется следующая фигура

        public int sizeMatrix; // Размерность матрицы текущей фигуры (2х2 3х3 4х4)
        public int sizeNextMatrix; // Размерность матрицы следующей фигуры (2х2 3х3 4х4)

        public int[,] firstType = new int[4, 4]
        {
            {0,0,1,0},
            {0,0,1,0},
            {0,0,1,0},
            {0,0,1,0},
        }; // Первый тип фигуры
        public int[,] secondType = new int[3, 3]
        {
            {0,2,0},
            {0,2,2},
            {0,0,2},
        }; // Второй тип фигуры
        public int[,] thirdType = new int[3, 3]
        {
            {0,0,0},
            {0,3,0},
            {3,3,3},
        }; // Третий тип фигуры
        public int[,] fourthType = new int[3, 3]
        {
            {4,0,0},
            {4,0,0},
            {4,4,0},
        }; // Четвертый тип фигуры
        public int[,] fifthType = new int[2, 2]
        {
            {5,5},
            {5,5},
        }; // Пятый тип фигуры

        public Shape(int _x, int _y) // Конструктор который мы используем при создании новой фигуры
        {
            x = _x; // Задаем координаты спавна нашей фигуры (x,y)
            y = _y;

            matrix = GenerateMatrix(); // Генерируем фигуру (как матрицу)

            sizeMatrix = (int)Math.Sqrt(matrix.Length); // Находим размерность матрицы (2х2 - 2 3х3 - 3 4х4 - 4)

            nextMatrix = GenerateMatrix(); // Генерируем следующую фигуру (как матрицу)
            sizeNextMatrix = (int)Math.Sqrt(nextMatrix.Length); // Находим размерность следующей матрицы (2х2 - 2 3х3 - 3 4х4 - 4)
        }

        /// <summary>
        /// Функция генерации следующей фигуры когда предыдущая прекратила движение
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        public void NextShape(int _x, int _y)
        {
            x = _x; // Задаем координату спавна нашей фигуры (x,y)
            y = _y;

            matrix = nextMatrix; // Присваиваем матрице текущей фигуры матрицу следующей фигуры

            sizeMatrix = (int)Math.Sqrt(matrix.Length); // Находим размерность матрицы (2х2 - 2 3х3 - 3 4х4 - 4)

            nextMatrix = GenerateMatrix(); // Генерируем следующую фигуру (как матрицу)

            sizeNextMatrix = (int)Math.Sqrt(nextMatrix.Length); // Находим размерность следующей матрицы (2х2 - 2 3х3 - 3 4х4 - 4)
        }


        /// <summary>
        /// Создание "каркаса" фигуры
        /// </summary>
        /// <returns></returns>
        public int[,] GenerateMatrix()
        {
            int[,] _matrix = firstType; // Создание переменной со значением по умолчанию
            Random r = new Random(); // Создаем рандомную переменную которая генерирует тип фигуры
            switch (r.Next(1, 6)) // Выбираем тип следующей фигуры
            {
                case 1: _matrix = firstType; break;
                case 2: _matrix = secondType; break;
                case 3: _matrix = thirdType; break;
                case 4: _matrix = fourthType; break;
                case 5: _matrix = fifthType; break;
            }
            return _matrix; // Возращаем полученную матрицу
        }

        /// <summary>
        /// Функция поворота фигуры
        /// </summary>
        public void RotateShape()
        {
            int[,] tempMatrix = new int[sizeMatrix, sizeMatrix]; // Создаем переменную матрицу

            for (int i = 0; i < sizeMatrix; i++)
            {
                for (int j = 0; j < sizeMatrix; j++) // Поворачиваем значения нашей матрицы на 90 градусов
                {
                    tempMatrix[i, j] = matrix[j, (sizeMatrix - 1) - i];
                }
            }

            matrix = tempMatrix; // Присваиваем текущей матрице значение переменной матрицы

            int offset = (8 - (x + sizeMatrix)); // Находим смещение для того чтобы удерживать нашу фигуру в том же положении

            if (offset < 0)
            {
                for (int i = 0; i < Math.Abs(offset); i++) // Если смещение фигуры ушло за карту то мы двигаем фигуру влево
                    MoveLeft();
            }

            if (x < 0)
            {
                for (int i = 0; i < Math.Abs(x) + 1; i++) // Если смещение фигуры ушло за карту то мы двигаем фигуру вправо
                    MoveRight();
            }

        }

        /// <summary>
        /// Функция движения фигуры вниз
        /// </summary>
        public void MoveDown()
        {
            y++;
        }

        /// <summary>
        /// Функция движения фигуры вправо
        /// </summary>
        public void MoveRight()
        {
            x++;
        }

        /// <summary>
        /// Функция движения фигуры влево
        /// </summary>
        public void MoveLeft()
        {
            x--;
        }
    }
}
