using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace perfumeFinal
{
    // Perfume class for parsing data
    public class Perfume
    {
        // Properties of Perfume
        // _headers are all accord names
        // _listPerfume is the list of dictionaries parsed from txt while initialization
        // _criteria are not accords
        private readonly List<string> _headers;
        private readonly List<Dictionary<string, string>> _listPerfume;
        private readonly string[] _criteria =
            {"name", "loved", "liked", "disliked", "winter", "spring", "summer", "autumn", "winter", "day", "night"};
        
        // Parses txt while instantiation
        public Perfume(string txtPath)
        {
            // Saves header names into a list and perfume entries into the memory
            // with a "list of dictionaries" data structure
            var perfList = new List<Dictionary<string, string>>();
            var headers = new List<string>();
            try
            {
                // Read all text into string array
                var perfumeRows = File.ReadAllText(txtPath).Split('~');
                // Parse string array into dictionaries
                foreach (var row in perfumeRows)
                {
                    var perfumeEntry = new Dictionary<string, string>();
                    var split = row.Split(";");
                    perfumeEntry["name"] = split[0].Trim();
                    foreach (var info in split.Skip(1))
                    {
                        var kv = info.Split(":");
                        perfumeEntry[kv[0]] = kv[1];
                        if (!headers.Contains(kv[0])) 
                            headers.Add(kv[0]);
                    }
                    perfList.Add(perfumeEntry);
                }
                _headers = headers;
                _listPerfume = perfList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("File cannot be opened. Check path.");
                throw;
            }
        }

        // Check if a string collection is a subset of another
        // only used for convenience
        private static bool IsSubset(IEnumerable<string> subset, IEnumerable<string> outer)
        {
            return !subset.Except(outer).Any();
        }

        // Filter perfume entries by given accord names
        public IEnumerable<Dictionary<string, string>> FilterByAccords(IEnumerable<string> accords)
        {
            // Check if input accords are valid or not
            var subset = accords as string[] ?? accords.ToArray();
            if (!IsSubset(subset, _headers))
            {
                Console.WriteLine("Invalid accords");
                return new List<Dictionary<string, string>>();
            }
            
            // Filter by given accords using LINQ expression
            var filtered = 
                _listPerfume.Where(perfume => IsSubset(subset, perfume.Keys)).ToList();

            return filtered;
        }

        // Print all of the rows
        // Unused right now
        public void PrintData()
        {
            foreach (var perf in _listPerfume)
            {
                foreach (var (k, v) in perf)
                {
                    Console.Write($"{k} : {v}, ");
                }
                Console.WriteLine();
            }
        }

        // Print menu to the console
        public static void GuiWelcome()
        {
            Console.WriteLine("1. Search perfume by name");
            Console.WriteLine("2. Filter perfumes by accords");
            Console.WriteLine("3. Count perfumes");
            Console.WriteLine("4. Suggest perfumes");
            Console.WriteLine("Type \"exit\" to close the program.");
        }
        
        
        // Print headers
        public void PrintHeaders()
        {
            foreach (var h in _headers)
            {
                Console.WriteLine(h);
            }
        }
        
        // Write parsed txt file into another organized txt file line by line
        // Just used once to obtain lines.txt in the folder
        public void WriteToTxt(string path)
        {
            using var sw = new StreamWriter(path);
            foreach (var perf in _listPerfume)
            {
                var last = perf.Last();
                foreach (var kvp in perf)
                {
                    sw.Write(Equals(kvp, last) ? $"{kvp.Key} : {kvp.Value}" : $"{kvp.Key} : {kvp.Value}, ");
                }
                if (!Equals(perf, _listPerfume.Last()))
                    sw.WriteLine(";");
            }
        }

        // Search by name in data using LINQ expression
        public IEnumerable<Dictionary<string, string>> SearchByName(string name)
        {
            return _listPerfume.Where(x => x["name"].ToLower().Contains(name.ToLower()));
        }

        // Print list of dictionaries in an organized way
        public static void PrintListOfDict(IEnumerable<Dictionary<string, string>> collection)
        {
            foreach (var p in collection)
            {
                foreach (var (k, v) in p)
                {
                    Console.Write($"{k} : {v}; ");
                }
                Console.WriteLine();
            }
        }
        
        // Print dictionary key value pairs in an organized way
        private static void PrintDict(Dictionary<string, int> dict)
        {
            foreach (var (k, v) in dict)
            {
                Console.WriteLine($"{k} : {v}");
            }
        }
        
        
        // Suggests perfume with given accords and values
        // Uses vector length calculation
        public IEnumerable<Dictionary<string, string>> SuggestPerfume(string input)
        {
            // Split the input
            var inputAccords = input.Split(',');
            // Initialize empty dictionary
            var accordSearch = new Dictionary<string, int>();
            
            // Store accord and value information into dictionary
            foreach (var accord in inputAccords)
            {
                var kv = accord.Split(':');
                accordSearch[kv[0]] = int.Parse(kv[1]);
            }
            
            // Filter by given accords in order to reduce runtime
            var filtered = FilterByAccords(accordSearch.Keys.ToArray());
            var perfumeList = filtered.ToList();
            
            // For every perfume, calculate their distances (vector lengths from input values to current perfume)
            foreach (var perfume in perfumeList)
            {
                double distance = 0;
                foreach (var (accord, value) in accordSearch)
                {
                    distance += Math.Pow(value - int.Parse(perfume[accord]) ,2);
                }
                
                distance = Math.Sqrt(distance);
                // Store distance information in the same dictionary
                perfume["distance"] = distance.ToString(CultureInfo.InvariantCulture);
            }
            
            // Using LINQ expression, sort perfume list by their distances and store 10 nearest perfumes
            var sug = (from pe in perfumeList
                orderby double.Parse(pe["distance"])
                select pe).Take(10);
            
            return sug;
        }

        // Counts total, for men, for women and accord
        public Dictionary<string, int> CountPerfumes(params string[] type)
        {
            if (type.Length != 0 && type.Length != 1)
            {
                Console.WriteLine("Invalid parameters");
                return new Dictionary<string, int>();
            }
            
            // Total count
            if (type.Length == 0)
            {
                var total = new Dictionary<string, int> {{"total", _listPerfume.Count}};
                return total;
            }

            switch (type[0].ToLower())
            {
                case "men":
                    var men = new Dictionary<string, int> {{"total men", SearchByName("for men").Count()}};
                    PrintDict(men);
                    return men;
                case "women":
                    var women = new Dictionary<string, int> {{"total women", SearchByName("for women").Count()}};
                    PrintDict(women);
                    return women;
                case "accord":
                    var accords = new Dictionary<string, int>();
                    foreach (var acc in _listPerfume.SelectMany(perfume => perfume.Keys))
                    {
                        if (accords.ContainsKey(acc))
                        {
                            accords[acc]++;
                        }
                        else if (!_criteria.Contains(acc))
                        {
                            accords[acc] = 1;
                        }
                    }
                    PrintDict(accords);
                    return accords;
                default:
                    Console.WriteLine("Invalid parameters");
                    return new Dictionary<string, int>();
            }
        }
    }
}
