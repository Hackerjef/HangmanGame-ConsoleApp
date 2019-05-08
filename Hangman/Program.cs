using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using static System.Console;

// August
// 4/29/19

namespace Hangman
{

    static class Storage
    {
        //From setup
        public static string PickedString;
        public static string type = "word";
        public static int amountofchars;
        public static bool isSentence = false;
        public static List<string> UserEnteredStrings = new List<string>();
        public static List<string> String_Disassembled = new List<string>();
        public static List<string> String_Disassembled_Blank = new List<string>();
        //During Gameplay
        public static List<char> CharAlreadyUsed = new List<char>();
        public static int failedattempts = 0;
        public static int attemptsmade = 0;
    }

    class Program
    {
        static void Main(string[] args)
        {
            //Get away from the main Public method for Easy Callback into Main Menu
            MainMenu();
        }

        static void MainMenu()
        {
            WriteLine("Main Menu:\n\n");

            //Menu
            WriteLine("1) Enter Words or sentences " + WordList());
            WriteLine("2) Clear Words or sentances");
            WriteLine("3) Play Hangman");
            WriteLine("Q) Quit");
            ConsoleKeyInfo UserChoice = ReadKey();
            //WriteLine(UserChoice.Key);

            switch (UserChoice.Key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    Clear();
                    EnterWordsDisplay();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    Storage.UserEnteredStrings.Clear();
                    Clear();
                    MainMenu();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Clear();
                    if (!Storage.UserEnteredStrings.Any())
                    {
                        WriteLine("Please Enter words to play hangman\n");
                        MainMenu();
                    }
                    else
                    {
                        SetupGame();
                        Play();
                    }
                    break;
                case ConsoleKey.Escape:
                case ConsoleKey.Q:
                    break;
                case ConsoleKey.Enter:
                    Clear();
                    MainMenu();
                    break;
                default:
                    Clear();
                    Console.WriteLine("Invalid answer\n");
                    MainMenu();
                    break;
            }
        }

        private static string WordList()
        {
            if (!Storage.UserEnteredStrings.Any())
            {
                return "";
            }
            else
            {
                return "[" + string.Join(", ", Storage.UserEnteredStrings.ToArray()) + "]";
            }
        }

        static void SetupGame()
        {
            //Select Random String from User Generated List
            Random r = new Random();
            int index = r.Next(Storage.UserEnteredStrings.Count);
            Storage.PickedString = Storage.UserEnteredStrings[index];

            //Disassemble stringnq
            Storage.String_Disassembled = Storage.PickedString.ToCharArray().Select(c => c.ToString()).ToArray().ToList();
                
            //make empty string array with _
            foreach(string s in Storage.String_Disassembled)
            {
                //only using this just so make a identical list with _ and skip spaces *how to allow sentences hopefully* and Count the chars + decide if its a sentence or word
                if (string.IsNullOrWhiteSpace(s))
                {
                    Storage.type = "sentence";
                    Storage.String_Disassembled_Blank.Add(" ");
                }
                else
                {
                    Storage.String_Disassembled_Blank.Add("_");
                    Storage.amountofchars += 1;
                }
            }            
        }

        static void EnterWordsDisplay()
        {
            WriteLine("Please Enter a word or sentence:");
            string UserEnteredWord = ReadLine();
            Clear();
            if (string.IsNullOrEmpty(UserEnteredWord))
            {
                WriteLine("Can not have a Empty input\n");
            }
            else
            {
                Storage.UserEnteredStrings.Add(UserEnteredWord.ToString().ToLower());
            }
            MainMenu();
        }

        static void Play()
        {
            //uncomment for debuging
            //Debug();
            
            bool docontinue = true;

            //check loss
            if (Storage.failedattempts > 7)
            {
                docontinue = false;
                WriteLine("You have lost the game\n");
                WriteLine(Storage.type + ": " + Storage.PickedString);
                WriteLine("Amount of tries: " + Storage.attemptsmade);
            }

            //check won
            if (Storage.String_Disassembled.SequenceEqual(Storage.String_Disassembled_Blank))
            {
                docontinue = false;
                WriteLine("You have Won The game :D\n");
                WriteLine(Storage.type + ": " + Storage.PickedString);
                WriteLine("Amount of falled attempts: " + Storage.failedattempts);
                WriteLine("Amount of tries: " + Storage.attemptsmade);
            }

            if (docontinue)
            {
                DisplayGame();
            }
            else
            {
                WriteLine("\nDo you want to play another round? y/n");
                ConsoleKeyInfo UserChoice = ReadKey();
                if (UserChoice.Key == ConsoleKey.Y)
                {
                    Clear();
                    Storage.PickedString = "";
                    Storage.type = "word";
                    Storage.amountofchars = 0;
                    Storage.UserEnteredStrings.Clear();
                    Storage.String_Disassembled.Clear();
                    Storage.String_Disassembled_Blank.Clear();
                    Storage.CharAlreadyUsed.Clear();
                    Storage.failedattempts = 0;
                    Storage.attemptsmade = 0;
                    MainMenu();
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        //Main Playing Method
        static void DisplayGame()
        {
            DrawHangman(Storage.failedattempts);
            WriteLine("\n\n");
            WriteLine("The " + Storage.type + " has " + Storage.amountofchars + " letters: " + string.Join(" ", Storage.String_Disassembled_Blank.ToArray()) + "\n");
            WriteLine("Please Enter A Letter:");
            ConsoleKeyInfo UserChoice = ReadKey();
            //WriteLine(UserChoice.Key);
            Clear();
            switch (UserChoice.Key)
            {
                case ConsoleKey.Enter:
                case ConsoleKey.Spacebar:
                    Clear();
                    Play();
                    break;
                default:
                    Clear();
                    GameLogic(UserChoice.KeyChar);
                    break;
            }
        }

        static void GameLogic(char charstring)
        {
            //check if they did that already
            if (Storage.CharAlreadyUsed.Contains(charstring))
            {
                WriteLine("Already used that Letter!\n");
                Play();
            }
            Storage.CharAlreadyUsed.Add(charstring);

            //count attempt
            Storage.attemptsmade += 1;

            //check if char is in word list, If so update the blank one if not then add a falled attempt
            int timesfound = 0;
            for (int i = 0; i < Storage.String_Disassembled.Count; i++)
            {
                if (Storage.String_Disassembled[i].Equals(charstring.ToString()))
                {
                    timesfound += 1;
                    Storage.String_Disassembled_Blank[i] = Storage.String_Disassembled[i];
                }
            }
            if (timesfound == 0)
            {
                WriteLine("Sorry, The letter " + charstring.ToString() + " has not been found\n\n");
                Storage.failedattempts += 1;
            }
            else
            {
                WriteLine("The Letter " + charstring.ToString() + " has been found " + timesfound + " times!\n\n");
            }

            //continue playing
            Play();
        }

        private static void Debug()
        {
            //If you want to enable stats throughout the game please commentout line 147
            WriteLine("Picked String: " + Storage.PickedString);
            WriteLine("Picked string dissisembled: " + string.Join(", ", Storage.String_Disassembled.ToArray()));
            WriteLine("Picked String dissisembled blank: " + string.Join(", ", Storage.String_Disassembled_Blank.ToArray()));
            WriteLine("Entered strings: " + string.Join(", ", Storage.UserEnteredStrings.ToArray()));
            WriteLine("Type of string: " + Storage.type);
            WriteLine("Ammount of chars: " + Storage.amountofchars);
            WriteLine("Ammount of chars user has entered: " + string.Join(", ", Storage.CharAlreadyUsed.ToArray()));
            WriteLine("Ammount of tries: " + Storage.attemptsmade);
            WriteLine("Amount of Fails: " + Storage.failedattempts);
            WriteLine("Is the game complete: " + Storage.String_Disassembled.SequenceEqual(Storage.String_Disassembled_Blank));
            WriteLine("\n\n");
        }

        private static void DrawHangman(int state)
        {
            switch (state)
            {
                case 0:
                    WriteLine(@"
   ____
  |    |
  |
  |
  |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 1:
                    WriteLine(@"
   ____
  |    |
  |    o
  |
  |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 2:
                    WriteLine(@"
   ____
  |    |
  |    o
  |    |
  |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 3:
                    WriteLine(@"
   ____
  |    |
  |    o
  |   /|
  |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 4:
                    WriteLine(@"
   ____
  |    |
  |    o
  |   /|\
  |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 5:
                    WriteLine(@"
   ____
  |    |
  |    o
  |   /|\
  |    |
  |
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 6:
                    WriteLine(@"
   ____
  |    |
  |    o
  |   /|\
  |    |
  |   /	
 _|_
|   |______
|          |
|__________|
");
                    break;
                case 7:
                    WriteLine(@"
   ____
  |    |
  |    o
  |   /|\
  |    |
  |   /	\
 _|_
|   |______
|          |
|__________|
");
                    break;
                default:
                    break;
            }
        }
    }
}