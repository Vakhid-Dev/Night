using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CsvHelper;

namespace Sample
{
    namespace Json
    {
        internal class Json
        {
            // Using Text.Json
            private async Task FileJson_Ser_Des_Async(Person person)
            {

                string file = "user.json";

                var options = new JsonSerializerOptions
                {
                    AllowTrailingCommas = true,
                    IgnoreNullValues = false,
                    WriteIndented = true,

                };
                using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
                {
                    await JsonSerializer.SerializeAsync(fs, person, options);
                    Console.WriteLine("Data has been saved to file");
                }
                using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate))
                {
                    Person restoredPerson = await JsonSerializer.DeserializeAsync<Person>(fs);
                    Console.WriteLine($"Name: {restoredPerson.Name}  Age: {restoredPerson.Age}");
                }
            }

            public class Person
            {
                [JsonPropertyName("FirstName")]

                public string Name { get; set; }

                public int Age { get; set; }
            }
        }

    }
    namespace CSV
    {
        internal class CSV
        {
            // Using CsvHelper
            private string path = @"C:\SomeDir2\Simple_CSV.csv";

            private void WriteObject(string path)
            {
                var records = new List<Person>
                {
                    new Person { Id = 1, Name = "Ivan" },
                    new Person { Id = 2, Name = "Max" },
                    new Person { Id = 3, Name = "Elena" },
                };

                using (var writer = new StreamWriter(path))

                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(records);
                }
            }

            private List<Person> ReadObjects(string path)
            {
                using (var reader = new StreamReader(path))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<Person>().ToList();

                    return records;
                }
            }

            private class Person
            {
                public int Id { get; set; }
                public string Name { get; set; }
            }
        }

    }
    namespace Excel
    {
        public class Excel
        {

        }

    }
    namespace XML
    {
        public class Xml
        {
            static void LinqToXml()
            {
                var path = "Path => xml files"
                XDocument doc = XDocument.Load($"{path}");
                IEnumerable<XElement> orders = doc.Element("Orders").Descendants("Order");
                int orderId = 100;
                IEnumerable<XElement> order = orders.Where(x => x.Attribute("id").Value == orderId.ToString());
                XElement adress = order.Select(x => x.Element("Adress")).FirstOrDefault();
                string name = adress.Element("Name").Value;
            }
        }
    }
    namespace PDF
    {
        public class PDF { }
    }
}
