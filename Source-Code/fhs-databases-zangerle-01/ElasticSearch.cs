using Nest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fhs_databases_zangerle_01
{

    public static class ElasticSearch
    {

        public static Uri Node = new Uri("http://localhost:9200");

        private static ConnectionSettings _setttings;
        public static ConnectionSettings Settings
        {
            get
            {
                if (_setttings == null)
                {
                    _setttings = new ConnectionSettings(Node);
                    _setttings.DefaultIndex("books");
                }
                return _setttings;
            }
        }

        public static ElasticClient Client = new ElasticClient(Settings);


        public static void SetupIndex()
        {
            // Setting up our books index
            var indexDescriptor = new CreateIndexDescriptor("books")
                    .Mappings(ms => ms
                        .Map<Book>(m => m.AutoMap()));
            Client.CreateIndex("books", i => indexDescriptor);
            Console.WriteLine("Index created");
        }

        public static void PopulateIndex(string sourceDir)
        {
            var files = Directory.GetFiles(sourceDir);
            var i = 0;
            foreach (var file in files)
            {
                Client.Index(new Book(File.ReadAllText(file)));
                Console.WriteLine($"Indexed {i}/{files.Count()}");
                i++;
            }
            Console.WriteLine("Index populated");
        }
    }
}
