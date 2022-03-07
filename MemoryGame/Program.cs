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

        static void Main()
        {
            difficulty = 0;
            int attempts = 0;
            int chances = 1;
            int firstSel, secondSel;

            do
            {
                Console.WriteLine("Welcome in  Memory Game!");
                Console.WriteLine("1 - easy");
                Console.WriteLine("2 - hard");
                Console.WriteLine("3 - info");

                // wybor tryby przez uzytkownika
                Console.Write("\nPlease choose difficulty: ");
                while (!int.TryParse(Console.ReadLine(), out difficulty) || difficulty <= 0 || difficulty > 3)
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
                    Console.Write("Enter 1st index: ");
                    while (!int.TryParse(Console.ReadLine(), out firstSel) || firstSel <= 0 || firstSel > covered.Length)
                        Console.Write("Not integer or wrong index, try again: ");
                    firstSel--;
                    if (covered[firstSel] == "x")
                    {
                        valid = true;
                    }
                    else
                    {
                        Console.Write("\nInvalid input. Enter diffrent index");
                        valid = false;
                    }
                } while (!valid);

                // aktualizacja planszy i wyswietlenie jej na konsoli
                ShowCovered(firstSel);
                ShowBoard(attempts, difficulty);

                // odsloniecie drugiej karty przez uzytkownika
                do
                {
                    Console.Write("Enter 2nd index: ");
                    while (!int.TryParse(Console.ReadLine(), out secondSel) || secondSel <= 0 || secondSel > covered.Length)
                        Console.Write("Not integer or wrong index, try again: ");
                    secondSel--;
                    if (covered[secondSel] == "x")
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
                ShowCovered(secondSel);
                ShowBoard(attempts, difficulty);

                // sprawdzenie zgodnosci elementow
                CheckMatch(covered, firstSel, secondSel);
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

        // losowanie haseł i przypisanie ich do tablicy 'uncovered'
        public static string[] Randomizer(int elements)
        {
            Random random = new Random();

            // dodanie wszystkich hasel do listy
            List<string> allWords = File.ReadAllLines(@"C:\Users\kewsw\OneDrive\Pulpit\Coding\C#\MemoryGame\Words.txt").ToList();

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
            Console.WriteLine($"Difficulty:  {level[difficulty - 1]}");
            Console.WriteLine("Attempts:    {0}\n", attempts);
            for (int i = 0; i < covered.Length; i++)
            {
                Console.WriteLine($"{i + 1}.\t{covered[i]}");
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
                string path = @"C:\Users\kewsw\OneDrive\Pulpit\Coding\C#\MemoryGame\Scores.txt";
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

