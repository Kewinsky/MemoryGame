// See https://aka.ms/new-console-template for more information

using System.Diagnostics;

namespace HelloWorld
{
    class Program
    {
        // plansze gry
        static int elements = 4;
        static string[] covered = new string[elements * 2];
        static string[] uncovered = new string[elements * 2];

        // trudnoc gry
        static string[] level = { "easy", "hard" };
        static int difficulty = 0;

        static bool ifWin = false;
        static Stopwatch timer = new Stopwatch();

        static IDictionary<string, int> inputs = new Dictionary<string, int>()
        {
            ["A1"] = 0,
            ["A2"] = 4,
            ["A3"] = 8,
            ["A4"] = 12,
            ["B1"] = 1,
            ["B2"] = 5,
            ["B3"] = 9,
            ["B4"] = 13,
            ["C1"] = 2,
            ["C2"] = 6,
            ["C3"] = 10,
            ["C4"] = 14,
            ["D1"] = 3,
            ["D2"] = 7,
            ["D3"] = 11,
            ["D4"] = 15,
        };

        static void Main()
        {
            difficulty = 0;
            int attempts = 0;
            int chances = 1;
            string firstSel, secondSel;

            do
            {
                Console.WriteLine("Welcome in  Memory Game!");
                Console.WriteLine("1 - easy");
                Console.WriteLine("2 - hard");
                Console.WriteLine("3 - info");
                Console.WriteLine("4 - ranking");

                // wybor tryby przez uzytkownika
                Console.Write("\nPlease choose difficulty: ");
                while (!int.TryParse(Console.ReadLine(), out difficulty) || difficulty <= 0 || difficulty > 4)
                    Console.Write("Not integer or wrong index, try again: ");
                Console.Clear();
                // --------------------------------------------------------------------------

                // ustawienie odpowiednich parametrow dla danego trybu gry
                switch (difficulty)
                {
                    case 1:
                        elements = 2;
                        covered = new string[elements * 2];
                        uncovered = new string[elements * 2];
                        attempts = 10;
                        Randomizer(elements);
                        break;
                    case 2:
                        elements = 8;
                        covered = new string[elements * 2];
                        uncovered = new string[elements * 2];
                        attempts = 15;
                        Randomizer(elements);
                        break;
                    case 3:
                        Info();
                        break;
                    case 4:
                        Ranking();
                        break;
                }
            } while (difficulty != 1 && difficulty != 2);


            // resetowanie planszy (w przypadku rozpoczecia nowej gry ustawia domyslne wartosci tj. 'x')
            ResetGame(covered);

            // timer start / resetowanie timera
            timer = Stopwatch.StartNew();
            timer.Start();

            do
            {
                ShowBoard(attempts, difficulty);

                bool valid;
                // odsloniecie pierwszej karty przez uzytkownika
                do
                {
                    int value = 0;
                    Console.Write("Enter 1st coordinates: ");
                    do
                    {
                        firstSel = Console.ReadLine().ToUpper();
                        if (!inputs.ContainsKey(firstSel) || inputs[firstSel] >= covered.Length)
                        {
                            Console.Write("Invalid input, try again: ");
                        }

                    } while (!inputs.ContainsKey(firstSel) || inputs[firstSel] >= covered.Length);

                    if (covered[inputs[firstSel]] == "x")
                    {
                        valid = true;
                    }
                    else
                    {
                        Console.Write("\nInvalid input. Enter diffrent index.");
                        valid = false;
                    }
                } while (!valid);

                // aktualizacja planszy i wyswietlenie jej na konsoli
                ShowCovered(inputs[firstSel]);
                ShowBoard(attempts, difficulty);

                // odsloniecie drugiej karty przez uzytkownika
                do
                {
                    int value = 0;
                    Console.Write("Enter 2nd coordinates: ");
                    do
                    {
                        secondSel = Console.ReadLine().ToUpper();
                        if (!inputs.ContainsKey(secondSel) || inputs[secondSel] >= covered.Length)
                        {
                            Console.Write("Invalid input, try again: ");
                        }

                    } while (!inputs.ContainsKey(secondSel) || inputs[secondSel] >= covered.Length);

                    if (covered[inputs[secondSel]] == "x")
                    {
                        valid = true;
                    }
                    else
                    {
                        Console.Write("\nInvalid input. Enter diffrent index.");
                        valid = false;
                    }
                } while (!valid);

                // aktualizacja planszy i wyswietlenie jej na konsoli
                ShowCovered(inputs[secondSel]);
                ShowBoard(attempts, difficulty);

                // sprawdzenie zgodnosci elementow
                CheckMatch(covered, inputs[firstSel], inputs[secondSel]);
                Console.Write("\nPress any key to continue.");
                Console.ReadKey();
                Console.Clear();

                // weryfikacja wygranej/przegranej
                IfWin(covered, attempts, chances);
                chances++;
                attempts--;

            } while (!ifWin);
        }

        // tablica informacyjna
        public static void Info()
        {
            Console.Clear();
            Console.WriteLine("         |   Elements    |   Attempts");
            Console.WriteLine("Easy Mode|      4        |      10");
            Console.WriteLine("Hard Mode|      8        |      15");
            Console.Write("\nPress any key to continue.");
            Console.ReadKey();
            Console.Clear();
        }

        public static void Ranking()
        {
            Console.Clear();
            string fileName = "Scores.txt";
            string path = Path.GetFullPath(fileName);
            string[] text = System.IO.File.ReadAllLines(path);
            foreach (string line in text)
                Console.WriteLine(line);
            Console.Write("\nPress any key to continue.");
            Console.ReadKey();
            Console.Clear();
        }

        // losowanie haseł i przypisanie ich do tablicy 'uncovered'
        public static string[] Randomizer(int elements)
        {
            Random random = new Random();

            // dodanie wszystkich hasel do listy
            string fileName = "Words.txt";
            string path = Path.GetFullPath(fileName);
            List<string> allWords = File.ReadAllLines(path).ToList();

            // lista dla wylosowanych hasel
            List<string> selected = new List<string>();


            // dodanie wylosowanych elementow do listy 'selected'
            for (int i = 0; i < elements; i++)
            {
                string randomString = allWords[random.Next(0, allWords.Count)];
                selected.Add(randomString);
            }

            // duplikacja elementow w liscie selected
            selected = selected.SelectMany(t => Enumerable.Repeat(t, 2)).ToList();

            // wypelnienie tablicy 'uncovered' elementami z listy 'selected'
            for (int i = 0; i < uncovered.Length; i++)
            {
                int index = random.Next(0, selected.Count);
                string rndWord = selected[index];
                uncovered[i] = rndWord;
                selected.RemoveAt(index);
            }
            return uncovered;
        }

        // wyswietlenie planszy na konsoli
        static void ShowBoard(int attempts, int difficulty)
        {
            int line = 1;
            Console.WriteLine($"Difficulty:  {level[difficulty - 1]}");
            Console.WriteLine("Attempts:    {0}\n", attempts);
            Console.WriteLine("   A|B|C|D|");
            for (int i = 0; i < covered.Length; i++)
            {
                if (i == 0 || i % 4 == 0)
                {
                    Console.Write($"{line}. ");
                    line++;
                }
                Console.Write(covered[i] + "|");
                if (i == 3 || i == 7 || i == 11)
                    Console.WriteLine();
            }
            Console.WriteLine("");
        }

        // odkrycie odganietych kart
        static void ShowCovered(int input)
        {
            covered[input] = uncovered[input];
            Console.Clear();
        }

        // sprawdzenie czy odloniete karty sa takie same
        static void CheckMatch(string[] array, int firstSel, int secondSel)
        {
            // zgodnosc
            if (array[firstSel] == array[secondSel] && firstSel != secondSel)
            {
                Console.WriteLine("Match!");
                return;
            }

            // brak zgodnosci - ustawienie domyslnych wartosci
            array[firstSel] = "x";
            array[secondSel] = "x";
        }

        // sprawdzenie czy gracz wygral
        static bool IfWin(string[] array, int attempts, int chances)
        {
            // przypadek gdy graczowi skoncza sie ruchy
            if (attempts == 1)
            {
                Console.WriteLine("*----------------------*");
                Console.WriteLine("|                      |");
                Console.WriteLine("|  Out of attempts :(  |");
                Console.WriteLine("|                      |");
                Console.WriteLine("*----------------------*\n");
                ifWin = true;
                Again();
            }
            else if (array.Contains("x"))
            {
                ifWin = false;
            }
            // wygrana
            else
            {
                ifWin = true;
                Console.WriteLine("*----------------*");
                Console.WriteLine("|                |");
                Console.WriteLine("|  Congrats! :)  |");
                Console.WriteLine("|                |");
                Console.WriteLine("*----------------*\n");
                Console.WriteLine($"You solved the memory game after {chances} chances.");

                // zatrzymanie czasu gry i wyswietlenie go na konsoli
                timer.Stop();
                TimeSpan timeTaken = timer.Elapsed;
                string time = "Time taken: " + timeTaken.ToString(@"m\:ss\.fff");
                Console.WriteLine(time + "\n");

                // zapis wyniku do pliku Scores.txt
                Console.Write("What's your name? ");
                string name = Console.ReadLine();
                string fileName = "Scores.txt";
                string path = Path.GetFullPath(fileName);
                File.AppendAllText(path, $"|{name}\t|{timeTaken.ToString(@"m\:ss\.fff")}\t|{chances}\t|" + Environment.NewLine);

                // ponowne uruchomienie gry
                Again();
            }
            return ifWin;
        }

        // zapytanie o ponowna gre
        static void Again()
        {
            string input;
            Console.Write("\nWould you like to play again? (y/n) ");
            do
            {
                input = Console.ReadLine();
                if (input != "y" || input != "n")
                    Console.Write("Choose between 'y' or 'n': ");
            } while (input != "y" && input != "n");

            Console.Clear();

            // ponowna gra
            if (input == "y")
            {
                Main();
            }

            // wyjscie z gry
            else if (input == "n")
            {
                Console.WriteLine("*-----------------------*");
                Console.WriteLine("|                       |");
                Console.WriteLine("|  Have a nice day! :)  |");
                Console.WriteLine("|                       |");
                Console.WriteLine("*-----------------------*");
            }
        }

        // resetuje plansze
        static string[] ResetGame(string[] array)
        {
            // ustawia domyslne znaki planszy
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = "x";
            }
            return array;
        }
    }
}
