/* Author: Anmol Singh
 * GitHub: https://github.com/arizonasingh/Runbeck
 * Date: 28 October 2020
 * Version: 1.0.0
 * Purpose: Coding Exercise as part of Runbeck Election Services Interview process.
 *          This program should accept a delimited text file as an input with
 *          either comma or tab delimination (CSV or TSV) and output a text file with
 *          the correct number of fields (if any) as selected by the user and a text
 *          file with the incorrect number of fields (if any).
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Runbeck
{
    class Program
    {
        static void Main(string[] args)
        {
            // ========= START OF USER INPUT QUESTIONS =========================

            // USER INPUT QUESTION 1
            Console.Write("Where is the Test delimited text file located? ");

            string filePath = Console.ReadLine();

            // if the file does not exist or is not a CSV or TSV file, ask the user to enter a valid file path again
            // using RegEx instead of checking if filePath ends with .csv or .tsv will help prevent junk file names such as Names.csv.csv
            while (!Regex.IsMatch(Path.GetFileName(filePath), @"^[\w]+.(TXT|txt|CSV|csv|TSV|tsv)$") || !File.Exists(filePath))
            {
                Console.Write("\nThe file does not exist or the file path is incorrect! Please enter the full valid file path for the Test file: ");

                filePath = Console.ReadLine();
            }

            // USER INPUT QUESTION 2
            Console.Write("\nIs the file format" +
                "\n[1] CSV (comma-separated values) or" +
                "\n[2] TSV (tab-separated values)?" +
                "\n\nPlease select the number corresponding to your choice: ");

            string fileFormat = Console.ReadLine();

            // USER INPUT QUESTION 3
            Console.Write("\nHow many fields should each record contain? ");

            string fields = Console.ReadLine();
            int numOfFields;

            // check to see if the user input was a digit, and if not, ask the user again until correct
            // the program will not accept a value larger than Int32 MaxValue of 2147483647
            while (!int.TryParse(fields, out numOfFields) || Convert.ToInt32(fields) <= 0)
            {
                Console.Write("\nPlease try again and select a Numeric value greater than Zero: ");
                fields = Console.ReadLine();
            }

            string regexMatch, regexMatchHigher;

            // while the program asked the user to specify a number corresponding to their file format choice
            // it is good practice to handle common user error or common user inputs, thus the program will
            // accept number selection as well as text abbreviation selection while ignoring case sensitivity
            while (true)
            {
                // number of commas or tabs is equal to number of fields - 1
                // records should match the exact RegEx match and not a higher or any Regex Match
                if (fileFormat.Equals("1") || fileFormat.Equals("CSV", StringComparison.OrdinalIgnoreCase))
                {
                    regexMatch = "([^,]*,){" + (numOfFields - 1) + "}([^,]*)";
                    regexMatchHigher = "([^,]*,){" + (numOfFields) + "}([^,]*)";
                    fileFormat = ".csv";
                    break;
                }
                else if (fileFormat.Equals("2") || fileFormat.Equals("TSV", StringComparison.OrdinalIgnoreCase))
                {
                    regexMatch = "([^\\t]*\\t){" + (numOfFields - 1) + "}([^\\t]*)";
                    regexMatchHigher = "([^\\t]*\\t){" + (numOfFields) + "}([^\\t]*)";
                    fileFormat = ".tsv";
                    break;
                }
                else
                {
                    Console.Write("\nThe selected file format was not a valid option. Please select [1] CSV or [2] TSV: ");
                    fileFormat = Console.ReadLine();
                    continue;
                }
            }

            if (filePath.EndsWith(".txt"))
            {
                fileFormat = ".txt";
            }

            // ========== END OF USER INPUT QUESTIONS ==========================

            List<string> lines = new List<string>();
            lines = File.ReadAllLines(filePath).ToList();

            if (lines.Any())
            {
                List<string> goodRecords = new List<string>();
                List<string> badRecords = new List<string>();

                // Need to skip the header row, thus starting iterator at 1
                // It was specified that header row is always included, thus no need to check and can immediately skip first row/line
                for (int i = 1; i < lines.Count; i++)
                {
                    //remove any trailing commas or tabs if not all columns utilized for that row
                    if (lines[i].EndsWith(',') || lines[i].EndsWith("\t"))
                    {
                        lines[i] = lines[i].Remove(lines[i].Length - 1);
                    }

                    if (Regex.IsMatch(lines[i], regexMatch) && !Regex.IsMatch(lines[i], regexMatchHigher))
                    {
                        goodRecords.Add(lines[i]);
                    }
                    else
                    {
                        badRecords.Add(lines[i]);
                    }
                }

                // if the good and bad record lists are not empty and not null, create an output file with the corresponding records
                // alternative is to check records List Count > 0 however below method is much more readable and concise
                if (goodRecords.Any())
                {
                    File.WriteAllLines(Path.Combine(Path.GetDirectoryName(filePath), "CorrectlyFormattedRecords" + fileFormat), goodRecords);
                }
                if (badRecords.Any())
                {
                    File.WriteAllLines(Path.Combine(Path.GetDirectoryName(filePath), "IncorrectlyFormattedRecords" + fileFormat), badRecords);
                }

                Console.WriteLine("\nDone! The newly created output files are in the same directory as the original file.");
            }
            else
            {
                Console.WriteLine("\nThe selected file had empty contents. Please try running the program again with a different file.");
            }
        }

    }
}