using Annytab;
using LanguageDetection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace fhs_databases_zangerle_01
{
    public static class Documents
    {

        public static LanguageDetector LanguageDetector;
        public static Regex EndOfDisclaimer = new Regex(@"\*END\*.*\*END\*|\*\*\* START .*\*\*\*", RegexOptions.Compiled);

        public static string[] GoogleStopWords = new string[] { "a", "about", "above", "after", "again", "against", "all", "am", "an", "and", "any", "are", "aren't", "as", "at", "be", "because", "been", "before", "being", "below", "between", "both", "but", "by", "can't", "cannot", "could", "couldn't", "did", "didn't", "do", "does", "doesn't", "doing", "don't", "down", "during", "each", "few", "for", "from", "further", "had", "hadn't", "has", "hasn't", "have", "haven't", "having", "he", "he'd", "he'll", "he's", "her", "here", "here's", "hers", "herself", "him", "himself", "his", "how", "how's", "i", "i'd", "i'll", "i'm", "i've", "if", "in", "into", "is", "isn't", "it", "it's", "its", "itself", "let's", "me", "more", "most", "mustn't", "my", "myself", "no", "nor", "not", "of", "off", "on", "once", "only", "or", "other", "ought", "our", "ours", "ourselves", "out", "over", "own", "same", "shan't", "she", "she'd", "she'll", "she's", "should", "shouldn't", "so", "some", "such", "than", "that", "that's", "the", "their", "theirs", "them", "themselves", "then", "there", "there's", "these", "they", "they'd", "they'll", "they're", "they've", "this", "those", "through", "to", "too", "under", "until", "up", "very", "was", "wasn't", "we", "we'd", "we'll", "we're", "we've", "were", "weren't", "what", "what's", "when", "when's", "where", "where's", "which", "while", "who", "who's", "whom", "why", "why's", "with", "won't", "would", "wouldn't", "you", "you'd", "you'll", "you're", "you've", "your", "yours", "yourself", "yourselves" };


        public static HashSet<string> StemTerms(HashSet<string> terms)
        {
            Stemmer stemmer = new EnglishStemmer();
            var stemmedTerms = new HashSet<string>();
            foreach (var term in terms)
            {
                var stemmedTerm = stemmer.GetSteamWord(term);
                stemmedTerms.Add(stemmedTerm);
                Console.WriteLine(term + "  -->  " + stemmedTerm);
            }
            Console.WriteLine("Finished stemming terms");
            Console.WriteLine("Total count before stemming: " + terms.Count);
            Console.WriteLine("Total count after stemming: " + stemmedTerms.Count);
            return stemmedTerms;
        }

        public static List<string> FilterTerms(string[] terms)
        {
            var filteredTerms = new List<string>();
            foreach (var term in terms)
            {
                if (!GoogleStopWords.Contains(term) && Regex.IsMatch(term, "^[a-zA-Z0-9]{2,}$"))
                {
                    filteredTerms.Add(term);
                    //Console.WriteLine("Term, that passed the filter: " + term);
                }
            }
            Console.WriteLine("Finished filtering terms.");
            Console.WriteLine("Total count before filtering: " + terms.Count());
            Console.WriteLine("Total count after filtering: " + filteredTerms.Count);
            return filteredTerms;
        }

        public static HashSet<string> GetTermsForAllDocuments(string sourceDir, string countRankCSVPath)
        {
            var termCounts = new Dictionary<string, int>();
            var uniqueTerms = new HashSet<string>();
            var totalTermCount = 0;

            var files = Directory.GetFiles(sourceDir);

            foreach (var file in files)
            {
                var terms = GetTerms(file);
                var termsAfterFiltering = FilterTerms(terms);

                totalTermCount += termsAfterFiltering.Count;

                foreach (var term in termsAfterFiltering)
                {
                    // count words
                    if (termCounts.ContainsKey(term))
                    {
                        termCounts[term] = termCounts[term] + 1;
                    }
                    else
                    {
                        termCounts.Add(term, 1);
                    }
                    uniqueTerms.Add(term);

                    //stream.WriteLine(term);  // not necessary for our task ...
                }
                //Console.WriteLine(terms.Count() + " terms found in document: " + file);
                Console.WriteLine($"Processed file {Path.GetFileNameWithoutExtension(file)}/{files.Count()})");
                Console.WriteLine("Unique term count: " + uniqueTerms.Count);

            }

            WriteWordCountToCSV(termCounts, totalTermCount, countRankCSVPath);

            Console.WriteLine("Finished getting terms for all documents");
            Console.WriteLine("Term count total: " + totalTermCount);
            Console.WriteLine("Unique term count total: " + uniqueTerms.Count);

            return uniqueTerms;
        }



        public static void WriteWordCountToCSV(Dictionary<string, int> termCounts, int totalTermCount, string csvPath)
        {
            var termCountList = termCounts.ToList(); // convert dictionairy to list of key-value-pairs
            termCountList.Sort((pair1, pair2) => pair2.Value.CompareTo(pair1.Value)); // sort dictionairy by value (descending)

            using (StreamWriter stream = File.AppendText(csvPath))
            {
                int rank = 1;
                foreach (var term in termCounts.OrderByDescending(pair => pair.Value))
                {
                    var relativeTermFrequency = term.Value / (double)totalTermCount;
                    stream.WriteLine(rank + "\t" + relativeTermFrequency); // rank 1 corresponds to highest relative term frequency... and so on
                    rank++;
                }

                //for (int rank = 1; rank <= termCountList.Count; rank++)
                //{
                //    var relativeTermFrequency = termCountList[rank - 1].Value / (double) totalTermCount;
                //    stream.WriteLine(rank + "\t" + relativeTermFrequency); // rank 1 corresponds to highest relative term frequency... and so on
                //}
            }
        }

        public static string[] GetTerms(string file)
        {
            var content = File.ReadAllText(file);
            char[] delimiters = new char[] { '\r', '\n', ' ', ',', '.', '?', ':', ';' };
            return content.TrimStart().TrimEnd().ToLower().Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
        }

        public static void SetupLanguageDetector()
        {
            LanguageDetector = new LanguageDetector();
            LanguageDetector.AddAllLanguages();
        }

        public static void RenameToNumbers(string sourceDir, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var files = Directory.GetFiles(sourceDir);
            for (int fileName = 1; fileName <= files.Count(); fileName++)
            {
                File.Move(files[fileName - 1], Path.Combine(outputDir, fileName + ".txt"));
            }
            Console.WriteLine("Finished Renaming Files");
        }

        public static void RemoveHeader(string sourceDir, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var files = Directory.GetFiles(sourceDir);
            foreach (var file in files)
            {
                var linesToSkip = GetEndOfDisclaimer(file);
                var lines = File.ReadLines(file).Skip(linesToSkip);
                var outputPath = Path.Combine(outputDir, Path.GetFileName(file));
                File.WriteAllLines(outputPath, lines);
            }
            Console.WriteLine("Finished Removing Disclaimer Headers");
        }

        public static int GetEndOfDisclaimer(string file)
        {
            var documentHead = File.ReadLines(file).Take(1000).ToList();
            for (int lineNumber = 0; lineNumber < documentHead.Count; lineNumber++)
            {
                if (EndOfDisclaimer.IsMatch(documentHead[lineNumber]))
                    return lineNumber + 1;
            }
            return 0;
        }

        public static bool TextIsEnglish(string filePath)
        {
            var content = File.ReadAllText(filePath);
            return LanguageDetector.Detect(content) == "en";
        }

        public static void FilterNonEnglishBooks(string path, string outputDir)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                Console.Write("Reading: " + file);
                if (TextIsEnglish(file))
                {
                    var destinationPath = Path.Combine(outputDir, Path.GetFileName(file));
                    File.Copy(file, destinationPath);
                    Console.Write("  --> English --> Copy to: " + outputDir + "\n\n");
                }
            }

            Console.WriteLine("Finished Filtering Books");
        }



    }
}
