using System.Drawing;
using System.Windows.Forms;

namespace Tetris.Controllers
{
    public static class MapController
    {
        public static Shape currentShape; // Текущая фигура

        public static int size; // Размер одной ячейки фигуры в px
        public static int[,] map = new int[16, 8]; // Размерность карты
        public static int linesRemoved; // Переменная количество удаленных линий
        public static int score; // Переменная количество игровых очков
        public static int Interval; // Интервал срабатывания таймера

        /// <summary>
        /// Функция которая отрисовывает следующую фигуру
        /// </summary>
        /// <param name="e"></param>
        public static void ShowNextShape(Graphics e)
        {
            for (int i = 0; i < currentShape.sizeNextMatrix; i++)
            {
                for (int j = 0; j < currentShape.sizeNextMatrix; j++)
                {
                    Brush brush = SelectBrush(currentShape.nextMatrix[i, j]); // Выбираем цвет кисти в зависимости от фигуры

                    e.FillRectangle(brush, new Rectangle(300 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1)); // Отрисовываем следующую фигуру
                }
            }
        }


        /// <summary>
        /// Функция которая выбирает цвет кисти для отрисовки фигур
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static Brush SelectBrush(int value)
        {
            Brush brush = Brushes.Transparent; // Задаем кисть по умолчанию прозрачную

            switch (value) // В зависимости от типа фигуры задаем разные цвета
            {
                case 1: brush = Brushes.Red; break;
                case 2: brush = Brushes.Yellow; break;
                case 3: brush = Brushes.Green; break;
                case 4: brush = Brushes.Blue; break;
                case 5: brush = Brushes.Purple; break;
            }

            return brush; // Возвращаем нашу кисть
        }


        /// <summary>
        /// Функция для очистки карты
        /// </summary>
        public static void ClearMap()
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map[i, j] = 0;
                }
            }
        }

        /// <summary>
        /// Функция отрисовки фигур на нашей карте
        /// </summary>
        /// <param name="e"></param>
        public static void DrawMap(Graphics e)
        {
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Brush brush = SelectBrush(map[i, j]); // Выбираем цвет которым будет закрашена ячейка карты

                    e.FillRectangle(brush, new Rectangle(50 + j * (size) + 1, 50 + i * (size) + 1, size - 1, size - 1)); // Закрашиваем ячейку карты
                }
            }
        }

        /// <summary>
        /// Функция для отрисовки сетки
        /// </summary>
        /// <param name="g"></param>
        public static void DrawGrid(Graphics g)
        {
            for (int i = 0; i <= 16; i++)
            {
                g.DrawLine(Pens.Black, new Point(50, 50 + i * size), new Point(50 + 8 * size, 50 + i * size)); // Отрисовываем горизонтальные линии
            }
            for (int i = 0; i <= 8; i++)
            {
                g.DrawLine(Pens.Black, new Point(50 + i * size, 50), new Point(50 + i * size, 50 + 16 * size)); // Отрисовываем вертикальные линии
            }
        }


        /// <summary>
        /// Функция для удаленяи линий блоков и обновления игровых данных
        /// </summary>
        /// <param name="label1"></param>
        /// <param name="label2"></param>
        public static void SliceMap(Label label1, Label label2)
        {
            int count = 0; // Количество ненулевых блоков
            int curRemovedLines = 0; // Текущее количество удаленных линий

            for (int i = 0; i < 16; i++)  // Пробегаемся по всем линиям карты
            {
                count = 0; // Обнуляем количество ненулевых элементов
                for (int j = 0; j < 8; j++) // Проверяем линию по столбцам
                {
                    if (map[i, j] != 0) // Если элемент карты в линии ненулевой увеличиваем количество ненулевых элементов на 1
                        count++;
                }

                if (count == 8) // После проверки линии проверяем заполнена ли вся линия
                {
                    curRemovedLines++; // Если заполнена то увеличиваем количествов удаляемых линий

                    //Смещаем карту на 1 линию вниз
                    for (int k = i; k >= 1; k--)
                    {
                        for (int o = 0; o < 8; o++)
                        {
                            map[k, o] = map[k - 1, o];
                        }
                    }
                }
            }

            for (int i = 0; i < curRemovedLines; i++) // За каждую удаленную линию добавляем по 10 очков (В серии за 1 линию -                                           
            {                                         // 10 очков за 2 линии - 30 очков и т.д.)
                score += 10 * (i + 1);
            }

            linesRemoved += curRemovedLines; // Обновляем информацию о удаленных линиях в общем

            if (linesRemoved % 5 == 0 && Interval > 60) // Проверяем если число удаленных линий кратно пяти и интервал больше 60 мс 
            {                                           // уменьшаем интервал для увеличения сложности
                Interval -= 10;
            }

            label1.Text = "Очки: " + score; // Обновляем информацию по игровым очкам
            label2.Text = "Линии: " + linesRemoved; // Обновляем информацию по удаленным линиям
        }

        /// <summary>
        /// Функция для проверки можно ли повернуть фигуру
        /// </summary>
        /// <returns></returns>
        public static bool IsIntersects()
        {
            // Пробегаем по матрице и проверяем
            // если при повороте наша фигура не
            // выйдет за границы карты или
            // не наложится на другие фигуры то
            // тогда возвращаем false то есть не накладывается 
            // значит можно повернуть

            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (j >= 0 && j <= 7)
                    {
                        if (map[i, j] != 0 && currentShape.matrix[i - currentShape.y, j - currentShape.x] == 0)
                            return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        ///  Функция для синхронизации фигуры с картой
        /// </summary>
        public static void Merge()
        {
            /* 
             * Здесь мы пробегаемся по всей карте начиная от текущих координат нашей фигуры 
             * до длины матрицы нашей фигуры соответственно
             * и это будут координаты нашей фигуры по отношению к карте
            */

            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    /* 
                     * Здесь мы проверяем что в ячейке нашей фигуры не 
                     * ноль для того чтобы прорисовать это на карте
                     * 0 - пустота
                     * (1-5) какая либо фигура деленная по типам 
                     * в матрицах фигур в зависимости от типа находятся различные числа
                    */

                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        map[i, j] = currentShape.matrix[i - currentShape.y, j - currentShape.x]; // Присваиваем ячейке карты не нулевое
                                                                                                 // значение для будущей отрисовки фигуры
                }
            }
        }

        /// <summary>
        /// Функция для очистки участка карты после движения фигуры
        /// </summary>
        public static void ResetArea()
        {
            /* 
             * Здесь мы пробегаемся по всей карте начиная от текущих координат нашей фигуры 
             * до длины матрицы нашей фигуры соответственно
             * и это будут координаты нашей фигуры по отношению к карте
            */

            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (i >= 0 && j >= 0 && i < 16 && j < 8) // проверяем не вышли ли мы за границы карты
                    {
                        if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                        {
                            map[i, j] = 0; // "Стираем с карты остатки фигуры после ее движения"
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Функция проверяющая столкновения фигуры
        /// </summary>
        /// <returns></returns>
        public static bool Collide()
        {
            for (int i = currentShape.y + currentShape.sizeMatrix - 1; i >= currentShape.y; i--)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (i + 1 == 16 || map[i + 1, j] != 0) // Проверяем встретилась фигура с полом или другой фигурой
                            return true;
                    }
                }
            }
            return false; // Если не сталкивается
        }


        /// <summary>
        /// Функция проверяющая столкновения и выходы за границы справа и слева
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool CollideHorizontal(int dir)
        {
            for (int i = currentShape.y; i < currentShape.y + currentShape.sizeMatrix; i++)
            {
                for (int j = currentShape.x; j < currentShape.x + currentShape.sizeMatrix; j++)
                {
                    if (currentShape.matrix[i - currentShape.y, j - currentShape.x] != 0)
                    {
                        if (j + 1 * dir > 7 || j + 1 * dir < 0)
                            return true;

                        if (map[i, j + 1 * dir] != 0)
                        {
                            if (j - currentShape.x + 1 * dir >= currentShape.sizeMatrix || j - currentShape.x + 1 * dir < 0)
                            {
                                return true;
                            }
                            if (currentShape.matrix[i - currentShape.y, j - currentShape.x + 1 * dir] == 0)
                                return true;
                        }
                    }
                }
            }
            return false;
        }


    }
}
