using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace autobakr {
    /*
     * Written by Ally Macdonald 1/12/2020.
     */
    class Program {
        static void Main(string[] args) {
            // This program uses Console user-based IO only.
            // Simple reason being is for a 3 hour task, going for executable parameters
            // is not practical without writing a full parser. Since I'm only going for 
            // a single file script, this shall do.
            // Compatibility external programs to input data and read output is possible.

            Console.WriteLine("Enter a list of types of bread to detect, or input a file path.");
            Console.WriteLine("File contents must be separated by new line.");
            Console.WriteLine("Press enter on empty line to finish entering types.");

            // We want the longest match possible, such that in the event one search term contains
            // part of another, the longest one takes precedence.
            // Read list of bread types. Whether that be from input or file.

            var breadTypes = GetUserFileList().OrderByDescending(s => s.Length).ToList();
            Console.WriteLine();

            Console.WriteLine("Enter a list of sentences to read, or input a file path.");
            Console.WriteLine("File contents must be separated by new line.");
            Console.WriteLine("Press enter on empty line to finish entering types.");

            // Read list of sentences, whether that be from input or file. (Separated by line)

            var sentences = GetUserFileList();
            Console.WriteLine();

            // A dictionary of sentences which gives the indexes and colours for the regions
            // where something should be highlighted.

            var matchSentences = GetColoredSentencesByPhrase(sentences, breadTypes);

            Console.WriteLine("Output matched sentences:");

            foreach (var output in matchSentences) {
                // Give us our highlighted output for every matched sentence.
                WriteColoured(output.Key, output.Value);
            }
        }

        private static Dictionary<string, Dictionary<int, ConsoleColor>> GetColoredSentencesByPhrase(List<string> sentences, List<string> searchPhrases) {
            var outputSentences = new Dictionary<string, Dictionary<int, ConsoleColor>>();

            // Escape the list of bread types. We don't want fancy characters interfering with
            // RegEx expressions.

            var escapedPhrases = searchPhrases.Select(t => Regex.Escape(t)).ToList();

            for (int i = 0; i < sentences.Count; i++) {
                var sentence = sentences[i];
                var indexes = new Dictionary<int, ConsoleColor>();
                var addSentence = false;

                for (int j = 0; j < searchPhrases.Count; j++) {

                    // Find all bread types and try to find a match or matches.
                    foreach (Match match in Regex.Matches(sentence, escapedPhrases[j], RegexOptions.IgnoreCase)) {

                        // Add the corresponding initial position to the dictionary.
                        if (!indexes.ContainsKey(match.Index)) {
                            indexes.Add(match.Index, ConsoleColor.Red);
                        }

                        // We want to reset the colour, so add the end position too.
                        if (!indexes.ContainsKey(match.Index + match.Length)) {
                            indexes.Add(match.Index + match.Length, Console.ForegroundColor);
                        }

                        // We found a match; we should definitely output this sentence.
                        addSentence = true;
                    }
                }

                // Only add the output sentence if we found *any* matches.
                if (addSentence) outputSentences.Add(sentence, indexes);
            }

            return outputSentences;
        }

        private static void WriteColoured(string str, Dictionary<int, ConsoleColor> textPos) {
            // Rudimentary method of highlight certain blocks of text - character by character.
            // Can be improved by means of substrings, because Console output is sloooooooow.
            var inColor = Console.ForegroundColor;

            // Read from all positions, determine colour change, print.
            for (int i = 0; i < str.Length; i++) {
                if (textPos.ContainsKey(i)) {
                    Console.ForegroundColor = textPos[i];
                }
                Console.Write(str[i]);
            }

            // Reset colour.
            Console.ForegroundColor = inColor;

            // Done.
            Console.WriteLine();
        }

        private static List<string> GetUserFileList() {
            // Maintain a list of all user-inputted entries.
            var userList = new List<string>();

            string input;
            bool empty;
            do {
                Console.Write("> ");

                // Read the line input.
                input = Console.ReadLine();
                empty = String.IsNullOrEmpty(input);

                if (!empty)
                    if (!File.Exists(input)) {
                        // Read from the direct input.
                        userList.Add(input);
                    } else {
                        // Read from the available file, splitting by lines.
                        var fileBreadList = File.ReadAllLines(input);
                        userList.AddRange(fileBreadList);
                    }
            } while (!empty);

            return userList;
        }
    }
}
