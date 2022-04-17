# Тесстовое задание AEON

## Оригиналный текст задания

Задача:

Сделать браузерную мини-игру (WebGL) со следующим функционалом:

1. Главный экран

На старте показываем пользователю экран с двумя кнопками "Играть" и "Результаты". По клику на первой кнопке запускается игровой трек. По клику на второй кнопке показывается экран с результатами прошлой игры (если они есть).

2. Игровой трек

На плоском прямоугольном треке у нас есть шарик (3d), который мы можем передвигать по треку клавишами WASD. Шарик находится в начале трека, пользователь должен довести его до конца трека (надо пересечь финишную черту). У шарика должна быть небольшая инерция при движении. На экран с результатами переходим при успешном завершении трека, либо падении шарика с трека. При нажатии ESC переходим на главный экран.

3. Экран с результатами

Если шарик сходит с трека - переходим на экран с результатами и выводим "Вы проиграли". Если шарик успешно дошел до финиша - переходим на экран с результатами и выводим "Вы выиграли" + время в секундах, которое потребовалось для перемещения шарика. При нажатии ESC переходим на главный экран.

Результаты:

Выполненное тестовое задание загружаем в репозиторий на GitHub (исходный код и демо-страница для демонстрации работы игры в браузере).

## Декомпозиция и согласование задания

Задача:

Сделать браузерную мини-игру (WebGL) со следующим функционалом:

Приложение состоит из экранов:

1. Главный экран (точка входа UI)

  - На старте показываем пользователю экран с двумя кнопками "Играть" и "Результаты".
  - По клику на первой кнопке ("Играть") запускается игровой трек.
  - По клику на второй кнопке ("Результаты") показывается экран с результатами прошлой игры (если они есть)

2. Игровой трек

  - на плоском прямоугольном треке (3d), финишную черту, стартовая позиция
  - на плоском прямоугольном треке у нас есть шарик (3d)
  - мы можем передвигать (шарик) по треку клавишами WASD: управление заменено на стрелки в соответствие с расположением камеры
  - (характер) у шарика должна быть небольшая инерция при движении
  - на старте матча шарик находится в начале трека
  - пользователь должен довести его до конца трека (надо пересечь финишную черту) - окончание матча
  - на экран с результатами переходим при успешном завершении трека - окончание матча, отдельный экран "Выиграли"
  - на экран с результатами переходим при падении шарика с трека - окончание матча, отдельный экран "Проиграли"
  - на главный экран переходим при нажатии ESC - окончание матча

3. Экран с результатами

  - переходим на экран (от игрового экрана с состоянием проигрыша: шарик сходит с трека вне зоны финиша), выводим:
    - "Вы проиграли"
  - переходим на экран (от игрового экрана с состоянием победы: если шарик успешно дошел до финиша, находится в зоне финиша), выводим:
    - "Вы выиграли"
    - время в секундах, которое потребовалось для перемещения шарика.
  - При нажатии ESC переходим на главный экран - далее после доп. экранов

4. Дополнительные экраны (2) победы и поражения в зависимости от результата матча

Результаты:

Выполненное тестовое задание загружаем в репозиторий на GitHub (исходный код и демо-страница для демонстрации работы игры в браузере).

## Пояснения к выполнению

Приложение реализовано как две FSM, игровая и пользовательского интерфейса. Реализация для них различна поскольку правила матча просты, и упрощенная конструкция второй не будет неудобна в рамках задания. Кроме того реализация демонстрирует подходы.  
Текс заменен на английский, т.к. использовался простой компонент текста для уменьшения объема приложения и простоты реализации.  
Предусмотрена возможность подключения анализатора игр и изменения управления матчем и пользовательским интерфейсом.  
Ведущая FSM пользовательского интерфейса, реализация в ScreensFsm, описание узлов в _UIDescription. Ведомая FSM игры, реализация _GameController, описание там же, конфигурируется на старте в ScreensFsm, далее в состоянии матча в _UIDescription.  
Оформление кода на мое усмотрение, т.к. требований на этот счет нет.
Не было никакого замечания о внешнем виде, по этому соблюдены технические параметры насколько это необходимо и приложение выглядит так чтобы было удобно видеть выполнение условий задания - не более.  

## Реализация

[С реализацией можно ознакомиться по ссылке](http://unity-aeon-demo.s3-website.eu-central-1.amazonaws.com/)
