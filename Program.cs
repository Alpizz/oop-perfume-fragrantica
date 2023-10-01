/*
 * BLG252E Object Oriented Programming
 * CRN: 11061
 * Final Assignment
 * Alper Zorlutuna
 * 040180515
 */


using System;
using System.Linq;

namespace perfumeFinal
{
    internal static class Program
    {
        private static void Main()
        {

            // Get path information
            const string path = @"./data/parfume-data.txt";
            // Instantiate
            var perfume = new Perfume(path);


            Console.WriteLine("Welcome to the world of Perfumes (data parsed from fragrantica.com");
            Console.WriteLine("What would you like to do?");

            // Do-While loop for UI
            do
            {
                Perfume.GuiWelcome();
                var key = Console.ReadLine()?.ToLower();
                // Switch case for UI
                // 1: name
                // 2: filter
                // 3: count
                // 4: suggest
                switch (key)
                {
                    case "1":
                        Console.WriteLine("Please enter perfume name:");
                        var name = Console.ReadLine();
                        Console.WriteLine("Here are the results:");
                        var searched = name != null ? perfume.SearchByName(name).ToList() : new List<Dictionary<string, string>>();
                        Perfume.PrintListOfDict(searched);
                        Console.WriteLine($"{searched.Count} entries found!");
                        break;

                    case "2":
                        Console.WriteLine("Enter your accord(s) (separated by commas if more than one).");
                        Console.WriteLine("(Ex. User input: \"amber,citrus\")");
                        var entry = Console.ReadLine()?.Split($",").ToList();
                        var filtered = entry != null ? perfume.FilterByAccords(entry).ToList() : new List<Dictionary<string, string>>();
                        Perfume.PrintListOfDict(filtered);
                        var count = filtered.Count;
                        Console.WriteLine($"{count} entries found!");
                        break;

                    case "3":
                        perfume.CountPerfumes();
                        perfume.CountPerfumes("men");
                        perfume.CountPerfumes("women");
                        perfume.CountPerfumes("accord");
                        break;
                    case "4":
                        Console.WriteLine("Enter accords and values (Ex. User input: \"amber:30,citrus:70\")");
                        var input = Console.ReadLine();
                        if (input != null)
                        {
                            Perfume.PrintListOfDict(perfume.SuggestPerfume(input));
                        }
                        break;
                    case "exit":
                        return;
                }
                Console.WriteLine("Anything else (y/n)?");
                var yn = Console.ReadLine();
                if ("n" == yn?.ToLower() || "no" == yn?.ToLower()) break;
            }
            while (true);

            Console.WriteLine("Made by Alper Zorlutuna for Object Oriented Programming course");
            Console.WriteLine("Have a fresh day :)");
        }
    }
}