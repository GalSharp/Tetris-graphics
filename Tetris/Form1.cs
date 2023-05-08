using System;
using System.Windows.Forms;
using Tetris.Controllers;

namespace Tetris
{
    public partial class Form1 : Form
    {       
        public Form1()
        {
            InitializeComponent();

            this.KeyUp += new KeyEventHandler(KeyFunc); // Подписываем нашу форму на событие нажатия клавиш на клавиатуре

            Init(); // Функция в которой мы инициализируем переменные
        }
        
        /// <summary>
        /// Функция для инициализации переменных
        /// </summary>
        public void Init()
        {
            MapController.size = 25; // Задаем размер одной ячейки фигуры (25px)
            MapController.score = 0; // Задаем переменную игровых очков (в начале игры 0)
            MapController.linesRemoved = 0; // Задаем переменную количества удаленных строк (в начале 0)
            MapController.currentShape = new Shape(3, 0); // Создаем новую и первую фигуру на карте
                                                          // (передаем координаты спавна фигуры 1 - X , 2 - Y)

            MapController.Interval = 300; // Задаем интервал в мс для обновления действий в игре
            label1.Text = "Очки: " + MapController.score; // Задаем значение надписи очков
            label2.Text = "Линии: " + MapController.linesRemoved; // Задаем значение надписи удаленных линий

            timer1.Interval = MapController.Interval; // Присваиваем значение интервала нашему таймеру
            timer1.Tick += new EventHandler(Update); // Подписываем наш таймер на событие Update
                                                     // которое будет вызываться каждый тик таймера

            timer1.Start(); // Запускаем наш таймер

            Invalidate(); // Обновляем (перерисовываем) поверхность нашей формы
        }

        /// <summary>
        /// Функция для обработки пользовательского ввода с клавиатуры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyFunc(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode) // Проверяем какая клавиша была нажата
            {
                case Keys.R: // Функция переворота фигуры
                    if (!MapController.IsIntersects()) // проверяем можно ли повернуть фигуру
                    {
                        MapController.ResetArea(); // Обновляем поверхность карты
                        MapController.currentShape.RotateShape(); // Поворачиваем фигуру
                        MapController.Merge(); // Синхронизируем фигуру с картой
                        Invalidate(); // Перерисовываем нашу форму
                    }
                    break;

                case Keys.Space: // Функция сброса фигуры
                    timer1.Interval = 10; // Ускоряем таймер чтобы фигура упала быстрее
                    break;

                case Keys.Right: // Функция для движения фигуры вправо при нажатии стрелки вправо или D
                case Keys.D:
                    if (!MapController.CollideHorizontal(1))
                    {
                        MapController.ResetArea(); // Обновляем поверхность карты
                        MapController.currentShape.MoveRight(); // Двигаем фигуру на одну секцию вправо
                        MapController.Merge(); // Синхронизируем фигуру с картой
                        Invalidate(); // Перерисовываем нашу форму
                    }
                    break;

                case Keys.Left: // Функция для движения фигуры влево при нажатии стрелки влево или A
                case Keys.A:
                    if (!MapController.CollideHorizontal(-1))
                    {
                        MapController.ResetArea(); // Обновляем поверхность карты
                        MapController.currentShape.MoveLeft(); // Двигаем фигуру на одну секцию влево
                        MapController.Merge(); // Синхронизируем фигуру с картой
                        Invalidate(); // Перерисовываем нашу форму
                    }
                    break;
            }
        }

        /// <summary>
        /// Функция которая отвечает за обновление информации в игре и вызывается каждый тик таймера
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update(object sender, EventArgs e)
        {
            MapController.ResetArea(); // Обновляем поверхность карты

            if (!MapController.Collide()) // Проверяем фигуру на наличие столкновений и если не сталкивается то двигаем фигуру вниз
            {
                MapController.currentShape.MoveDown(); // Если фигуре ничего не мешает то двигаем ее вниз
            }
            else
            {
                MapController.Merge(); // Соединяем фигуру с картой
                MapController.SliceMap(label1,label2); // Проверяем карту на возможность удаления линий
                timer1.Interval = MapController.Interval; // Если линии удалились и значение итнтервала поменялось то присваиваем
                                                          // новое значение интервалу таймера
                MapController.currentShape.NextShape(3,0); // Генерируем новую фигуру

                if (MapController.Collide()) // Если при спавне следующей фигуры она сразу же сталкивается с чем то то это конец игры
                {
                    MapController.ClearMap(); // Очищаем карту
                    timer1.Tick -= new EventHandler(Update); // Отписываем таймер от события
                    timer1.Stop(); // Останавливаем таймер
                    MessageBox.Show("Ваш результат: " + MapController.score); // Выводим сообщение с заработанными очками
                    Init(); // Проводим инициализацию переменных по новой (рестарт игры)
                }
            }
            MapController.Merge(); // Соединяем нашу фигуру с картой
            Invalidate(); // Переисовываем нашу форму
        }

        /// <summary>
        /// Событие для отрисовки всех графических элементов в игре
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPaint(object sender, PaintEventArgs e)
        {
            MapController.DrawGrid(e.Graphics); // Отрисовка сетки
            MapController.DrawMap(e.Graphics); // Отрисовка карты 
            MapController.ShowNextShape(e.Graphics); // Отрисовка следующей фигуры
        }

        /// <summary>
        /// Событие которое вызывается при нажатии на кнопку паузы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPauseButtonClick(object sender, EventArgs e)
        {
            var pressedButton = sender as ToolStripMenuItem; // Берем объект который вызвал события как кнопку меню-строки

            if (timer1.Enabled) // Если игра идет то мы меняем текст кнопки и останавливаем таймер
            {
                pressedButton.Text = "Продолжить";
                timer1.Stop();
            }
            else // Иначе мы меняем текст кнопки и продолжаем игру
            {
                pressedButton.Text = "Пауза";
                timer1.Start();
            }
        }

        /// <summary>
        /// Событие которое вызывается при нажатии на кнопку перезапуска игры
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAgainButtonClick(object sender, EventArgs e)
        {
            timer1.Tick -= new EventHandler(Update); // Отписываем таймер от события
            timer1.Stop(); // Останавливаем таймер
            MapController.ClearMap(); // Очищаем карту
            Init(); // Перезапускаем игру
        }

        /// <summary>
        /// Событие которое вызывается при нажатии на кнопку справки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnInfoPressed(object sender, EventArgs e)
        {
            string infoString = ""; // Объявляем строку информации 
            infoString = "Для управление фигурами используйте стрелочку влево/вправо (A/D).\n";
            infoString += "Чтобы ускорить падение фигуры - нажмите 'Пробел'.\n";
            infoString += "Для поворота фигуры используйте 'R'.\n";
            MessageBox.Show(infoString,"Справка"); // Пооказываем сообщение со справкой
        }


    }
}
