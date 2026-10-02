using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;

class MathGame
{
    static Random random = new Random();
    static List<User> users = new List<User>();
    static User? currentUser = null;
    static string usersFile = "users.json";
    
    // Система прогресса
    static int score = 0;
    static int level = 1;
    static int experience = 0;
    static int expToNextLevel = 100;
    static int totalCorrectAnswers = 0;
    static int currentStreak = 0;
    static int bestStreak = 0;
    static DateTime gameStartTime;
    static List<string> unlockedAchievements = new List<string>();

    static void Main()
    {
        Console.Title = "🎮 Математическое приключение";
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        LoadUsers();
        
        ShowAnimatedTitle();
        
        while (true)
        {
            if (currentUser == null)
            {
                ShowMainMenu();
            }
            else
            {
                ShowGameMenu();
            }
        }
    }

    static void ShowAnimatedTitle()
    {
        Console.Clear();
        string[] title = {
            "╔══════════════════════════════════════════════════════════════╗",
            "║                                                              ║",
            "║     ███╗   ███╗ █████╗ ████████╗██╗  ██╗                     ║",
            "║     ████╗ ████║██╔══██╗╚══██╔══╝██║  ██║                     ║",
            "║     ██╔████╔██║███████║   ██║   ███████║                     ║",
            "║     ██║╚██╔╝██║██╔══██║   ██║   ██╔══██║                     ║",
            "║     ██║ ╚═╝ ██║██║  ██║   ██║   ██║  ██║                     ║",
            "║     ╚═╝     ╚═╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝                     ║",
            "║                                                              ║",
            "║              ██████╗  █████╗ ███╗   ███╗███████╗            ║",
            "║             ██╔════╝ ██╔══██╗████╗ ████║██╔════╝            ║",
            "║             ██║  ███╗███████║██╔████╔██║█████╗              ║",
            "║             ██║   ██║██╔══██║██║╚██╔╝██║██╔══╝              ║",
            "║             ╚██████╔╝██║  ██║██║ ╚═╝ ██║███████╗            ║",
            "║              ╚═════╝ ╚═╝  ╚═╝╚═╝     ╚═╝╚══════╝            ║",
            "║                                                              ║",
            "╚══════════════════════════════════════════════════════════════╝"
        };
        
        foreach (string line in title)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(line);
            Thread.Sleep(50);
        }
        
        Console.WriteLine("\n              Нажмите любую клавишу чтобы начать...");
        Console.ReadKey();
    }

    static void ShowMainMenu()
    {
        Console.Clear();
        DrawBox("ГЛАВНОЕ МЕНЮ", ConsoleColor.Cyan);
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(@"
    ╔══════════════════════════════════════╗
    ║          ВЫБЕРИТЕ ДЕЙСТВИЕ           ║
    ╠══════════════════════════════════════╣
    ║                                      ║
    ║   🔑 1. Войти в аккаунт              ║
    ║   📝 2. Создать персонажа            ║
    ║   🏆 3. Зал славы                    ║
    ║   📊 4. Статистика сервера           ║
    ║   ❓ 5. Об игре                      ║
    ║   🚪 6. Выход                        ║
    ║                                      ║
    ╚══════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.Write("\n  🎯 Ваш выбор: ");
        string? choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1": Login(); break;
            case "2": Register(); break;
            case "3": ShowLeaderboard(); break;
            case "4": ShowServerStats(); break;
            case "5": ShowAbout(); break;
            case "6": 
                SaveUsers();
                Console.WriteLine("\n  👋 До новых встреч, странник!");
                Environment.Exit(0);
                break;
            default:
                ShowError("Неверный выбор!");
                break;
        }
    }

    static void ShowGameMenu()
    {
        Console.Clear();
        
        // Верхняя панель с информацией
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.Write("║ ");
        Console.ForegroundColor = GetConsoleColor(currentUser!.NameColor);
        Console.Write($"🎮 {GetTitleWithColor(currentUser.Title)} {currentUser.Username}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($" Уровень {currentUser.Level}  |  💎 {currentUser.Crystals} кристаллов".PadLeft(35) + " ║");
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        
        // Опыт
        Console.Write("║ ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"⭐ Опыт: {currentUser.Experience}/{currentUser.ExpToNextLevel}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("".PadLeft(40) + " ║");
        DrawExpBar(currentUser.Experience, currentUser.ExpToNextLevel);
        Console.WriteLine("╠══════════════════════════════════════════════════════════════╣");
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(@"
    ╔══════════════════════════════════════╗
    ║            ИГРОВОЕ МЕНЮ               ║
    ╠══════════════════════════════════════╣
    ║                                      ║
    ║   ⚔️ 1. Рейтинговая битва            ║
    ║   📚 2. Тренировочный лагерь         ║
    ║   🎨 3. Гардероб                     ║
    ║   🏪 4. Магазин наград               ║
    ║   📜 5. Мои достижения               ║
    ║   📊 6. Статистика                   ║
    ║   ⚙️ 7. Настройки профиля            ║
    ║   🚪 8. Выйти из аккаунта            ║
    ║                                      ║
    ╚══════════════════════════════════════╝");
        Console.ResetColor();
        
        Console.Write("\n  🎯 Ваш выбор: ");
        string? choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1": PlayGame(true); break;
            case "2": PlayGame(false); break;
            case "3": ShowWardrobe(); break;
            case "4": ShowShop(); break;
            case "5": ShowAchievements(); break;
            case "6": ShowPlayerStats(); break;
            case "7": EditProfile(); break;
            case "8": Logout(); break;
            default: ShowError("Неверный выбор!"); break;
        }
    }

    static void PlayGame(bool ranked)
    {
        // Сброс игровой сессии
        score = 0;
        level = 1;
        experience = 0;
        expToNextLevel = CalculateExpForLevel(level);
        totalCorrectAnswers = 0;
        currentStreak = 0;
        bestStreak = 0;
        unlockedAchievements.Clear();
        gameStartTime = DateTime.Now;
        
        Console.Clear();
        DrawBox("⚔️ МАТЕМАТИЧЕСКАЯ БИТВА ⚔️", ConsoleColor.Red);
        
        if (ranked)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n  🏆 РЕЙТИНГОВЫЙ РЕЖИМ - Ваши победы будут записаны в летопись!");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n  📚 ТРЕНИРОВОЧНЫЙ ЛАГЕРЬ - Тренируйтесь без последствий!");
        }
        Console.ResetColor();
        
        Console.WriteLine("\n  📖 Правила:");
        Console.WriteLine("  • За правильный ответ вы получаете опыт");
        Console.WriteLine("  • Сложность растёт с каждым уровнем");
        Console.WriteLine("  • Серия правильных ответов даёт бонусы!");
        Console.WriteLine("  • Напишите 'exit' чтобы сбежать с поля боя\n");
        
        Console.WriteLine("  Нажмите любую клавишу чтобы начать...");
        Console.ReadKey();
        
        while (true)
        {
            if (!GenerateQuestion())
            {
                // Конец игры
                TimeSpan gameTime = DateTime.Now - gameStartTime;
                
                Console.Clear();
                DrawBox("🏁 БИТВА ЗАВЕРШЕНА 🏁", ConsoleColor.Yellow);
                
                Console.WriteLine($"\n  ⏱️  Время в игре: {gameTime.Minutes} мин {gameTime.Seconds} сек");
                Console.WriteLine($"  ✅ Правильных ответов: {totalCorrectAnswers}");
                Console.WriteLine($"  🔥 Лучшая серия: {bestStreak}");
                Console.WriteLine($"  📊 Достигнут уровень: {level}");
                Console.WriteLine($"  💰 Заработано опыта: {experience}");
                
                if (ranked)
                {
                    int earnedCrystals = CalculateCrystals(experience, totalCorrectAnswers, bestStreak);
                    currentUser!.Crystals += earnedCrystals;
                    
                    if (experience > currentUser.Experience)
                    {
                        currentUser.Experience = experience;
                        CheckLevelUp();
                    }
                    
                    if (score > currentUser.HighScore)
                    {
                        currentUser.HighScore = score;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n  🎉 НОВЫЙ РЕКОРД: {score} очков! 🎉");
                    }
                    
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"\n  💎 Получено кристаллов: {earnedCrystals}");
                    
                    SaveUsers();
                }
                
                Console.ResetColor();
                Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
                Console.ReadKey();
                break;
            }
        }
    }

    static bool GenerateQuestion()
    {
        Console.Clear();
        
        // Верхняя информационная панель
        DrawGameHeader();
        
        // Генерация примера
        int maxNumber = Math.Max(10, 10 + (level * 8));
        int operationCount = Math.Min(1 + (level / 4), 5); // Ограничиваем количество операций
        
        string question;
        int correctAnswer;
        
        try
        {
            if (operationCount == 1)
            {
                (question, correctAnswer) = GenerateSimpleExample(maxNumber);
            }
            else
            {
                (question, correctAnswer) = GenerateComplexExample(maxNumber, operationCount);
            }
        }
        catch
        {
            // Если произошла ошибка, генерируем простой пример
            (question, correctAnswer) = GenerateSimpleExample(20);
        }

        int[] options = GenerateOptions(correctAnswer, maxNumber);
        ShuffleArray(options);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\n  📐 Решите пример:\n");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"     {question} = ?\n");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("  📋 Варианты ответов:\n");
        
        for (int i = 0; i < options.Length; i++)
        {
            Console.ForegroundColor = (ConsoleColor)(i + 10);
            Console.WriteLine($"     {i + 1}) {options[i]}");
        }

        int userAnswer = GetUserAnswer(options.Length);
        
        if (userAnswer == -1) return false;
        
        bool isCorrect = options[userAnswer - 1] == correctAnswer;
        
        if (isCorrect)
        {
            // Правильный ответ
            currentStreak++;
            if (currentStreak > bestStreak) bestStreak = currentStreak;
            
            totalCorrectAnswers++;
            
            // Расчет награды
            int baseExp = 10 + (level * 5);
            int streakBonus = currentStreak >= 3 ? (int)(baseExp * (currentStreak * 0.1)) : 0;
            int earnedExp = baseExp + streakBonus;
            
            experience += earnedExp;
            score += earnedExp;
            
            Console.Clear();
            DrawGameHeader();
            
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ✅ ВЕРНО! +{earnedExp} опыта");
            
            if (streakBonus > 0)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  🔥 Серия x{currentStreak}! Бонус +{streakBonus}!");
            }
            
            // Проверка повышения уровня
            if (experience >= expToNextLevel)
            {
                LevelUp();
            }
            
            // Достижения
            CheckAchievements();
            
            Console.ResetColor();
            Thread.Sleep(1500);
        }
        else
        {
            // Неправильный ответ
            Console.Clear();
            DrawGameHeader();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n  ❌ ОШИБКА! Правильный ответ: {correctAnswer}");
            Console.WriteLine($"  📉 Серия прервана на {currentStreak}");
            
            currentStreak = 0;
            
            Console.ResetColor();
            Thread.Sleep(2000);
            return false;
        }
        
        return true;
    }

    static (string, int) GenerateSimpleExample(int maxNumber)
    {
        int operation = random.Next(4);
        int a, b, result = 0;
        string op = "";

        // Убедимся что maxNumber достаточно большой
        maxNumber = Math.Max(maxNumber, 10);

        switch (operation)
        {
            case 0: // Сложение
                a = random.Next(1, maxNumber);
                b = random.Next(1, maxNumber);
                result = a + b;
                op = "+";
                break;
            case 1: // Вычитание
                a = random.Next(maxNumber / 2, maxNumber);
                b = random.Next(1, Math.Max(2, a - 1));
                result = a - b;
                op = "-";
                break;
            case 2: // Умножение
                a = random.Next(1, Math.Max(2, maxNumber / 3));
                b = random.Next(1, Math.Max(2, maxNumber / 3));
                result = a * b;
                op = "*";
                break;
            case 3: // Деление
                b = random.Next(1, Math.Max(2, maxNumber / 4));
                result = random.Next(1, Math.Max(2, maxNumber / 4));
                a = b * result;
                op = "/";
                break;
            default:
                a = 5;
                b = 3;
                result = 8;
                op = "+";
                break;
        }

        return ($"{a} {op} {b}", result);
    }

    static (string, int) GenerateComplexExample(int maxNumber, int operationCount)
    {
        string expression = "";
        int result = 0;
        
        // Ограничиваем количество операций
        operationCount = Math.Min(operationCount, 5);
        maxNumber = Math.Max(maxNumber, 20);
        
        for (int i = 0; i < operationCount; i++)
        {
            int divisor = Math.Max(1, i + 1);
            int divisor2 = Math.Max(1, i + 2);
            
            int a = random.Next(1, Math.Max(2, maxNumber / divisor));
            int b = random.Next(1, Math.Max(2, maxNumber / divisor2));
            int op = random.Next(2);
            
            if (i == 0)
            {
                result = op == 0 ? a + b : a - b;
                expression = $"{a} {(op == 0 ? "+" : "-")} {b}";
            }
            else
            {
                int newNum = random.Next(1, Math.Max(2, maxNumber / divisor2));
                int newOp = random.Next(2);
                result = newOp == 0 ? result + newNum : result - newNum;
                expression += $" {(newOp == 0 ? "+" : "-")} {newNum}";
            }
        }

        return (expression, result);
    }

    static int[] GenerateOptions(int correctAnswer, int maxNumber)
    {
        int[] options = new int[4];
        options[0] = correctAnswer;
        
        for (int i = 1; i < 4; i++)
        {
            int wrongAnswer;
            int attempts = 0;
            do
            {
                int variation = random.Next(-Math.Max(5, maxNumber / 3), Math.Max(5, maxNumber / 3));
                wrongAnswer = correctAnswer + variation;
                if (wrongAnswer < 0) wrongAnswer = Math.Abs(correctAnswer - variation);
                if (wrongAnswer == correctAnswer) wrongAnswer += random.Next(1, 5);
                
                attempts++;
                if (attempts > 50) break; // Защита от бесконечного цикла
                
            } while (Array.IndexOf(options, wrongAnswer) != -1 || wrongAnswer == correctAnswer);
            
            options[i] = wrongAnswer;
        }
        
        return options;
    }

    static void ShuffleArray(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    static int GetUserAnswer(int maxOption)
    {
        Console.Write("\n  🎯 Твой ответ (1-4) или 'exit': ");
        string? input = Console.ReadLine();
        
        if (input?.ToLower() == "exit")
        {
            return -1;
        }
        
        if (int.TryParse(input, out int answer) && answer >= 1 && answer <= maxOption)
        {
            return answer;
        }
        
        Console.WriteLine($"  ❌ Пожалуйста, введи число от 1 до {maxOption}");
        return GetUserAnswer(maxOption);
    }

    static void LevelUp()
    {
        level++;
        experience -= expToNextLevel;
        expToNextLevel = CalculateExpForLevel(level);
        
        // Награды за уровень
        int crystalsReward = level * 10;
        currentUser!.Crystals += crystalsReward;
        
        Console.Clear();
        DrawBox("🎊 ПОВЫШЕНИЕ УРОВНЯ! 🎊", ConsoleColor.Yellow);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  ⭐ Вы достигли {level} уровня!");
        Console.WriteLine($"  💎 Получено {crystalsReward} кристаллов");
        Console.WriteLine($"  📈 До следующего уровня: {experience}/{expToNextLevel} опыта");
        
        // Разблокировка новых возможностей и цветов
        CheckUnlockables();
        
        Console.ResetColor();
        Thread.Sleep(3000);
    }

    static void CheckUnlockables()
    {
        // Разблокировка цветов по уровням
        var colorUnlocks = new Dictionary<int, string>
        {
            { 3, "Green" },
            { 5, "Blue" },
            { 7, "Yellow" },
            { 10, "Cyan" },
            { 15, "Magenta" },
            { 20, "Red" }
        };
        
        if (colorUnlocks.ContainsKey(level))
        {
            string colorName = colorUnlocks[level];
            if (!currentUser!.UnlockedColors.Contains(colorName))
            {
                currentUser.UnlockedColors.Add(colorName);
                Console.ForegroundColor = GetConsoleColor(colorName);
                Console.WriteLine($"\n  🎨 РАЗБЛОКИРОВАН НОВЫЙ ЦВЕТ: {colorName}!");
                Console.ResetColor();
            }
        }
        
        // Разблокировка титулов за достижения
        CheckTitleUnlocks();
    }

    static void CheckTitleUnlocks()
    {
        var titleUnlocks = new Dictionary<string, (string title, string color, int requirement)>
        {
            ["math_novice"] = ("📚 Ученик", "White", 10),
            ["math_expert"] = ("🎓 Эксперт", "Green", 25),
            ["math_master"] = ("👑 Мастер", "Yellow", 50),
            ["math_legend"] = ("🌟 Легенда", "Magenta", 100),
            ["streak_master"] = ("🔥 Неудержимый", "Red", 10)
        };
        
        foreach (var title in titleUnlocks)
        {
            if (!currentUser!.UnlockedTitles.ContainsKey(title.Key))
            {
                bool unlock = false;
                
                switch (title.Key)
                {
                    case "math_novice": unlock = totalCorrectAnswers >= 10; break;
                    case "math_expert": unlock = totalCorrectAnswers >= 25; break;
                    case "math_master": unlock = totalCorrectAnswers >= 50; break;
                    case "math_legend": unlock = totalCorrectAnswers >= 100; break;
                    case "streak_master": unlock = bestStreak >= 10; break;
                }
                
                if (unlock)
                {
                    currentUser.UnlockedTitles[title.Key] = (title.Value.title, title.Value.color);
                    Console.ForegroundColor = GetConsoleColor(title.Value.color);
                    Console.WriteLine($"\n  🏆 РАЗБЛОКИРОВАН НОВЫЙ ТИТУЛ: {title.Value.title}!");
                    Console.ResetColor();
                }
            }
        }
    }

    static void CheckAchievements()
    {
        var achievements = new Dictionary<string, (string name, string icon, int crystals)>
        {
            ["first_blood"] = ("Первая кровь", "⚔️", 25),
            ["math_novice"] = ("Начинающий математик", "📚", 50),
            ["math_expert"] = ("Эксперт математики", "🎓", 100),
            ["math_master"] = ("Мастер математики", "👑", 250),
            ["streak_5"] = ("На волне", "🔥", 75),
            ["streak_10"] = ("Неудержимый", "💪", 150),
            ["level_5"] = ("Подающий надежды", "⭐", 50),
            ["level_10"] = ("Ветеран", "🌟", 100),
            ["level_20"] = ("Легенда", "👑", 500)
        };
        
        foreach (var ach in achievements)
        {
            if (!currentUser!.Achievements.Contains(ach.Key))
            {
                bool unlock = false;
                
                switch (ach.Key)
                {
                    case "first_blood": unlock = totalCorrectAnswers >= 1; break;
                    case "math_novice": unlock = totalCorrectAnswers >= 10; break;
                    case "math_expert": unlock = totalCorrectAnswers >= 50; break;
                    case "math_master": unlock = totalCorrectAnswers >= 100; break;
                    case "streak_5": unlock = bestStreak >= 5; break;
                    case "streak_10": unlock = bestStreak >= 10; break;
                    case "level_5": unlock = level >= 5; break;
                    case "level_10": unlock = level >= 10; break;
                    case "level_20": unlock = level >= 20; break;
                }
                
                if (unlock)
                {
                    currentUser.Achievements.Add(ach.Key);
                    currentUser.Crystals += ach.Value.crystals;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n  {ach.Value.icon} ДОСТИЖЕНИЕ РАЗБЛОКИРОВАНО: {ach.Value.name}!");
                    Console.WriteLine($"  💎 +{ach.Value.crystals} кристаллов!");
                    Console.ResetColor();
                    Thread.Sleep(2000);
                }
            }
        }
    }

    static int CalculateExpForLevel(int lvl)
    {
        return (int)(100 * Math.Pow(1.5, lvl - 1));
    }

    static int CalculateCrystals(int exp, int answers, int streak)
    {
        return (exp / 10) + (answers * 2) + (streak * 5);
    }

    static void CheckLevelUp()
    {
        while (currentUser!.Experience >= currentUser.ExpToNextLevel)
        {
            currentUser.Level++;
            currentUser.Experience -= currentUser.ExpToNextLevel;
            currentUser.ExpToNextLevel = CalculateExpForLevel(currentUser.Level);
            currentUser.Crystals += currentUser.Level * 10;
        }
    }

    static void ShowShop()
    {
        while (true)
        {
            Console.Clear();
            DrawBox("🏪 МАГАЗИН НАГРАД 🏪", ConsoleColor.Yellow);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n  💎 Ваши кристаллы: {currentUser!.Crystals}\n");
            
            var items = new[]
            {
                new { Name = "Титул 'Гений'", Price = 100, Type = "title", Value = "🧠 Гений", Color = "Cyan" },
                new { Name = "Титул 'Император'", Price = 500, Type = "title", Value = "👑 Император", Color = "Yellow" },
                new { Name = "Титул 'Феникс'", Price = 300, Type = "title", Value = "🔥 Феникс", Color = "Red" },
                new { Name = "Цвет 'Радужный'", Price = 500, Type = "color", Value = "Magenta", Color = "Magenta" }
            };
            
            Console.WriteLine("  📦 Доступные товары:\n");
            
            for (int i = 0; i < items.Length; i++)
            {
                Console.ForegroundColor = GetConsoleColor(items[i].Color);
                Console.WriteLine($"  {i + 1}. {items[i].Name}");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"     Цена: {items[i].Price} 💎");
                Console.WriteLine();
            }
            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("  0. Выйти из магазина");
            
            Console.Write("\n  🎯 Ваш выбор: ");
            string? choice = Console.ReadLine();
            
            if (choice == "0") break;
            
            if (int.TryParse(choice, out int index) && index >= 1 && index <= items.Length)
            {
                var item = items[index - 1];
                
                if (currentUser.Crystals >= item.Price)
                {
                    currentUser.Crystals -= item.Price;
                    
                    if (item.Type == "title")
                    {
                        string titleKey = "shop_" + item.Value;
                        currentUser.UnlockedTitles[titleKey] = (item.Value, item.Color);
                    }
                    else if (item.Type == "color")
                    {
                        if (!currentUser.UnlockedColors.Contains(item.Value))
                        {
                            currentUser.UnlockedColors.Add(item.Value);
                        }
                    }
                    
                    SaveUsers();
                    ShowSuccess($"✅ Куплено: {item.Name}!");
                }
                else
                {
                    ShowError("❌ Недостаточно кристаллов!");
                }
            }
        }
    }

    static void ShowWardrobe()
    {
        while (true)
        {
            Console.Clear();
            DrawBox("🎨 ГАРДЕРОБ 🎨", ConsoleColor.Magenta);
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n  👤 Настройка внешнего вида\n");
            
            Console.WriteLine("  1. Сменить цвет имени");
            Console.WriteLine("  2. Сменить титул");
            Console.WriteLine("  0. Назад");
            
            Console.Write("\n  🎯 Ваш выбор: ");
            string? choice = Console.ReadLine();
            
            if (choice == "0") break;
            
            switch (choice)
            {
                case "1":
                    ChangeNameColor();
                    break;
                case "2":
                    ChangeTitleFromWardrobe();
                    break;
            }
        }
    }

    static void ChangeNameColor()
    {
        Console.Clear();
        DrawBox("🎨 ВЫБОР ЦВЕТА ИМЕНИ 🎨", ConsoleColor.Cyan);
        
        Console.WriteLine("\n  Доступные цвета:\n");
        
        int index = 1;
        var availableColors = new List<string>();
        
        foreach (var color in currentUser!.UnlockedColors)
        {
            Console.ForegroundColor = GetConsoleColor(color);
            Console.WriteLine($"  {index}. {color}");
            availableColors.Add(color);
            index++;
        }
        
        if (availableColors.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  Нет доступных цветов. Повышайте уровень чтобы разблокировать!");
        }
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  0. Назад");
        
        Console.Write("\n  🎯 Ваш выбор: ");
        string? choice = Console.ReadLine();
        
        if (choice == "0") return;
        
        if (int.TryParse(choice, out int selectedIndex) && selectedIndex >= 1 && selectedIndex <= availableColors.Count)
        {
            currentUser.NameColor = availableColors[selectedIndex - 1];
            SaveUsers();
            ShowSuccess($"✅ Цвет изменён на {availableColors[selectedIndex - 1]}!");
        }
    }

    static void ChangeTitleFromWardrobe()
    {
        Console.Clear();
        DrawBox("🏆 ВЫБОР ТИТУЛА 🏆", ConsoleColor.Yellow);
        
        Console.WriteLine("\n  Доступные титулы:\n");
        
        int index = 1;
        var availableTitles = new List<string>();
        
        foreach (var title in currentUser!.UnlockedTitles)
        {
            Console.ForegroundColor = GetConsoleColor(title.Value.color);
            Console.WriteLine($"  {index}. {title.Value.title}");
            availableTitles.Add(title.Key);
            index++;
        }
        
        // Всегда доступен базовый титул
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"  {index}. 🆕 Новичок");
        
        if (availableTitles.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("\n  Разблокируйте новые титулы через достижения и магазин!");
        }
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  0. Назад");
        
        Console.Write("\n  🎯 Ваш выбор: ");
        string? choice = Console.ReadLine();
        
        if (choice == "0") return;
        
        if (int.TryParse(choice, out int selectedIndex))
        {
            if (selectedIndex >= 1 && selectedIndex <= availableTitles.Count)
            {
                string titleKey = availableTitles[selectedIndex - 1];
                currentUser.CurrentTitleKey = titleKey;
                currentUser.Title = currentUser.UnlockedTitles[titleKey].title;
                SaveUsers();
                ShowSuccess($"✅ Титул изменён на {currentUser.Title}!");
            }
            else if (selectedIndex == availableTitles.Count + 1)
            {
                currentUser.CurrentTitleKey = "default";
                currentUser.Title = "🆕 Новичок";
                SaveUsers();
                ShowSuccess("✅ Титул изменён на Новичок!");
            }
        }
    }

    static string GetTitleWithColor(string title)
    {
        if (currentUser != null && currentUser.CurrentTitleKey != "default" && 
            currentUser.UnlockedTitles.ContainsKey(currentUser.CurrentTitleKey))
        {
            return currentUser.UnlockedTitles[currentUser.CurrentTitleKey].title;
        }
        return title;
    }

    static ConsoleColor GetConsoleColor(string colorName)
    {
        return colorName switch
        {
            "White" => ConsoleColor.White,
            "Green" => ConsoleColor.Green,
            "Blue" => ConsoleColor.Blue,
            "Yellow" => ConsoleColor.Yellow,
            "Red" => ConsoleColor.Red,
            "Magenta" => ConsoleColor.Magenta,
            "Cyan" => ConsoleColor.Cyan,
            _ => ConsoleColor.White
        };
    }

    static void ShowAchievements()
    {
        Console.Clear();
        DrawBox("📜 МОИ ДОСТИЖЕНИЯ 📜", ConsoleColor.Yellow);
        
        var allAchievements = new Dictionary<string, (string name, string icon)>
        {
            ["first_blood"] = ("Первая кровь", "⚔️"),
            ["math_novice"] = ("Начинающий математик", "📚"),
            ["math_expert"] = ("Эксперт математики", "🎓"),
            ["math_master"] = ("Мастер математики", "👑"),
            ["streak_5"] = ("На волне", "🔥"),
            ["streak_10"] = ("Неудержимый", "💪"),
            ["level_5"] = ("Подающий надежды", "⭐"),
            ["level_10"] = ("Ветеран", "🌟"),
            ["level_20"] = ("Легенда", "👑")
        };
        
        Console.WriteLine("\n  📊 Ваши достижения:\n");
        
        foreach (var ach in allAchievements)
        {
            if (currentUser!.Achievements.Contains(ach.Key))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"  {ach.Value.icon} {ach.Value.name} - ВЫПОЛНЕНО ✅");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"  {ach.Value.icon} {ach.Value.name} - ЗАБЛОКИРОВАНО 🔒");
            }
        }
        
        Console.ResetColor();
        Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    static void ShowPlayerStats()
    {
        Console.Clear();
        DrawBox("📊 СТАТИСТИКА ИГРОКА 📊", ConsoleColor.Cyan);
        
        Console.ForegroundColor = GetConsoleColor(currentUser!.NameColor);
        Console.WriteLine($"\n  👤 {GetTitleWithColor(currentUser.Title)} {currentUser.Username}\n");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"  ⭐ Уровень: {currentUser.Level}");
        Console.WriteLine($"  📈 Опыт: {currentUser.Experience}/{currentUser.ExpToNextLevel}");
        Console.WriteLine($"  🏆 Рекорд: {currentUser.HighScore}");
        Console.WriteLine($"  💎 Кристаллы: {currentUser.Crystals}");
        Console.WriteLine($"  🎯 Достижений: {currentUser.Achievements.Count}/9");
        Console.WriteLine($"  🎨 Разблокировано цветов: {currentUser.UnlockedColors.Count}/6");
        Console.WriteLine($"  🏆 Разблокировано титулов: {currentUser.UnlockedTitles.Count}");
        
        Console.ResetColor();
        Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    static void ShowServerStats()
    {
        Console.Clear();
        DrawBox("📊 СТАТИСТИКА СЕРВЕРА 📊", ConsoleColor.Cyan);
        
        Console.WriteLine($"\n  👥 Всего игроков: {users.Count}");
        Console.WriteLine($"  🏆 Самый высокий рекорд: {(users.Count > 0 ? users.Max(u => u.HighScore) : 0)}");
        Console.WriteLine($"  ⭐ Самый высокий уровень: {(users.Count > 0 ? users.Max(u => u.Level) : 0)}");
        Console.WriteLine($"  📊 Средний уровень: {(users.Count > 0 ? users.Average(u => u.Level) : 0):F1}");
        
        Console.ResetColor();
        Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    static void ShowAbout()
    {
        Console.Clear();
        DrawBox("❓ ОБ ИГРЕ ❓", ConsoleColor.Cyan);
        
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("\n  🎮 Математическое приключение v3.0\n");
        Console.WriteLine("  Эпическое путешествие в мир математики!");
        Console.WriteLine("\n  📖 Особенности:");
        Console.WriteLine("  • Прогрессивная сложность");
        Console.WriteLine("  • Система уровней и опыта");
        Console.WriteLine("  • Достижения и награды");
        Console.WriteLine("  • Разблокировка цветов и титулов");
        Console.WriteLine("  • Магазин с уникальными предметами");
        Console.WriteLine("  • Персонализация персонажа");
        Console.WriteLine("\n  👨‍💻 Создано с любовью к математике!");
        
        Console.ResetColor();
        Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    static void ShowLeaderboard()
    {
        Console.Clear();
        DrawBox("🏆 ЗАЛ СЛАВЫ 🏆", ConsoleColor.Yellow);
        
        var topPlayers = users.OrderByDescending(u => u.HighScore).Take(10).ToList();
        
        Console.WriteLine("\n  Топ-10 игроков:\n");
        Console.WriteLine("  ┌─────┬────────────────────┬──────────┬──────────┬────────────────────┐");
        Console.WriteLine("  │ #   │ Игрок              │ Уровень  │ Рекорд   │ Титул              │");
        Console.WriteLine("  ├─────┼────────────────────┼──────────┼──────────┼────────────────────┤");
        
        for (int i = 0; i < topPlayers.Count; i++)
        {
            var player = topPlayers[i];
            string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
            string title = player.Title.Length > 18 ? player.Title.Substring(0, 15) + "..." : player.Title;
            Console.WriteLine($"  │ {medal,-3} │ {player.Username.PadRight(18)} │ {player.Level.ToString().PadRight(8)} │ {player.HighScore.ToString().PadRight(8)} │ {title.PadRight(18)} │");
        }
        
        if (topPlayers.Count == 0)
        {
            Console.WriteLine("  │     │ Нет игроков        │ 1        │ 0        │ Новичок            │");
        }
        
        Console.WriteLine("  └─────┴────────────────────┴──────────┴──────────┴────────────────────┘");
        
        Console.ResetColor();
        Console.WriteLine("\n  Нажмите любую клавишу для возврата...");
        Console.ReadKey();
    }

    static void DrawBox(string title, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║{CenterText(title, 62)}║");
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    static void DrawGameHeader()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
        Console.Write("║ ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"⭐ Уровень {level}  |  📊 Опыт: {experience}/{expToNextLevel}");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"  |  🔥 Серия: {currentStreak}".PadLeft(20) + " ║");
        
        // Полоса опыта
        Console.Write("║ ");
        int barWidth = 40;
        int filled = expToNextLevel > 0 ? (int)((double)experience / expToNextLevel * barWidth) : 0;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', filled));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', barWidth - filled));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(" ║");
        
        Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
        Console.ResetColor();
    }

    static void DrawExpBar(int current, int max)
    {
        Console.Write("║ ");
        int barWidth = 58;
        int filled = max > 0 ? (int)((double)current / max * barWidth) : 0;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(new string('█', filled));
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(new string('░', barWidth - filled));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(" ║");
    }

    static string CenterText(string text, int width)
    {
        if (text.Length >= width) return text;
        int leftPadding = (width - text.Length) / 2;
        int rightPadding = width - text.Length - leftPadding;
        return new string(' ', leftPadding) + text + new string(' ', rightPadding);
    }

    static void ShowSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n  {message}");
        Console.ResetColor();
        Thread.Sleep(1500);
    }

    static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n  {message}");
        Console.ResetColor();
        Thread.Sleep(1500);
    }

    static void Register()
    {
        Console.Clear();
        DrawBox("📝 СОЗДАНИЕ ПЕРСОНАЖА 📝", ConsoleColor.Yellow);
        
        Console.Write("\n  👤 Введите имя персонажа: ");
        string? username = Console.ReadLine();
        
        if (string.IsNullOrEmpty(username))
        {
            ShowError("❌ Имя не может быть пустым!");
            return;
        }
        
        if (users.Any(u => u.Username == username))
        {
            ShowError("❌ Это имя уже занято!");
            return;
        }
        
        Console.Write("  🔐 Придумайте пароль: ");
        string password = ReadPassword();
        
        var user = new User
        {
            Username = username,
            Password = password,
            HighScore = 0,
            Level = 1,
            Experience = 0,
            ExpToNextLevel = 100,
            Crystals = 50,
            NameColor = "White",
            Title = "🆕 Новичок",
            CurrentTitleKey = "default",
            Achievements = new List<string>(),
            UnlockedColors = new List<string> { "White" },
            UnlockedTitles = new Dictionary<string, (string title, string color)>()
        };
        
        users.Add(user);
        SaveUsers();
        
        ShowSuccess($"✅ Персонаж {username} создан! +50 💎 стартовых кристаллов");
    }

    static void Login()
    {
        Console.Clear();
        DrawBox("🔑 ВХОД В ИГРУ 🔑", ConsoleColor.Cyan);
        
        Console.Write("\n  👤 Имя персонажа: ");
        string? username = Console.ReadLine();
        
        Console.Write("  🔐 Пароль: ");
        string password = ReadPassword();
        
        var user = users.FirstOrDefault(u => u.Username == username && u.Password == password);
        
        if (user != null)
        {
            currentUser = user;
            ShowSuccess($"✅ С возвращением, {username}!");
        }
        else
        {
            ShowError("❌ Неверное имя или пароль!");
        }
    }

    static void Logout()
    {
        currentUser = null;
        ShowSuccess("👋 До новых встреч!");
    }

    static void EditProfile()
    {
        while (true)
        {
            Console.Clear();
            DrawBox("⚙️ НАСТРОЙКИ ПРОФИЛЯ ⚙️", ConsoleColor.Magenta);
            
            Console.WriteLine("\n  1. 🔐 Сменить пароль");
            Console.WriteLine("  2. ◀️ Назад");
            
            Console.Write("\n  🎯 Ваш выбор: ");
            string? choice = Console.ReadLine();
            
            switch (choice)
            {
                case "1":
                    ChangePassword();
                    break;
                case "2":
                    return;
                default:
                    ShowError("Неверный выбор!");
                    break;
            }
        }
    }

    static void ChangePassword()
    {
        Console.Clear();
        Console.Write("  🔐 Старый пароль: ");
        string oldPassword = ReadPassword();
        
        if (oldPassword != currentUser!.Password)
        {
            ShowError("❌ Неверный пароль!");
            return;
        }
        
        Console.Write("  🔐 Новый пароль: ");
        string newPassword = ReadPassword();
        
        currentUser.Password = newPassword;
        SaveUsers();
        
        ShowSuccess("✅ Пароль изменён!");
    }

    static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;
        
        do
        {
            key = Console.ReadKey(true);
            
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Substring(0, password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);
        
        Console.WriteLine();
        return password;
    }

    static void LoadUsers()
    {
        try
        {
            if (File.Exists(usersFile))
            {
                string json = File.ReadAllText(usersFile);
                var loadedUsers = JsonSerializer.Deserialize<List<User>>(json);
                if (loadedUsers != null)
                {
                    users = loadedUsers;
                    
                    // Обеспечиваем совместимость со старыми версиями
                    foreach (var user in users)
                    {
                        if (user.UnlockedColors == null)
                            user.UnlockedColors = new List<string> { "White" };
                        if (user.UnlockedTitles == null)
                            user.UnlockedTitles = new Dictionary<string, (string title, string color)>();
                        if (string.IsNullOrEmpty(user.NameColor))
                            user.NameColor = "White";
                        if (string.IsNullOrEmpty(user.CurrentTitleKey))
                            user.CurrentTitleKey = "default";
                    }
                }
            }
        }
        catch
        {
            users = new List<User>();
        }
    }

    static void SaveUsers()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(users, options);
            File.WriteAllText(usersFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка сохранения: {ex.Message}");
        }
    }
}

class User
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int HighScore { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExpToNextLevel { get; set; }
    public int Crystals { get; set; }
    public string NameColor { get; set; } = "White";
    public string Title { get; set; } = "🆕 Новичок";
    public string CurrentTitleKey { get; set; } = "default";
    public List<string> Achievements { get; set; } = new List<string>();
    public List<string> UnlockedColors { get; set; } = new List<string>();
    public Dictionary<string, (string title, string color)> UnlockedTitles { get; set; } = new Dictionary<string, (string title, string color)>();
}