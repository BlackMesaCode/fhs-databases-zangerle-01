using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LanguageDetection;
using System.Text.RegularExpressions;
using Annytab;
using MySql.Data.MySqlClient;
using Nest;

namespace fhs_databases_zangerle_01
{
    public class Program
    {

        // Pathes

        public static readonly string ExePath = Assembly.GetExecutingAssembly().Location;
        public static readonly string ExeDir = Path.GetDirectoryName(ExePath);
        public static readonly string RawPath = Path.Combine(ExeDir, @"D:\Downloads\pgdvd072006\TXT ONLY");
        public static readonly string HeaderRemovedPath = Path.Combine(ExeDir, "HeaderRemoved");
        public static readonly string NonEnglishFilteredPath = Path.Combine(ExeDir, "NonEnglishFiltered");
        public static readonly string RenamedPath = Path.Combine(ExeDir, "Renamed");
        public static readonly string TestPath = Path.Combine(ExeDir, "Test");
        public static readonly string TermDistinctPath = Path.Combine(ExeDir, @"terms_distinct.txt");
        public static readonly string TermDistinctStemmedPath = Path.Combine(ExeDir, @"terms_distinct_stemmed.txt");
        public static readonly string CountRankCSVPath = Path.Combine(ExeDir, @"count_rank.csv");


        static void Main(string[] args)
        {

            /*

            /////////// Preparing documents ///////////

            Documents.RemoveHeader(RawPath, HeaderRemovedPath);
            Documents.SetupLanguageDetector();
            Documents.FilterNonEnglishBooks(HeaderRemovedPath, NonEnglishFilteredPath);
            Documents.RenameToNumbers(NonEnglishFilteredPath, RenamedPath);

            /////////// Counting terms ///////////

            var terms = Documents.GetTermsForAllDocuments(RenamedPath, CountRankCSVPath);
            Documents.StemTerms(terms);

            /////////// MySQL ///////////

            MySQL.PopulateMysqlDatabase(RenamedPath);

            */

            /////////// ElasticSearch ///////////

            ElasticSearch.SetupIndex();
            ElasticSearch.PopulateIndex(RenamedPath);


            /////////// Prevent Console App From Closing ///////////

            Console.ReadLine();
        }







    }


}
