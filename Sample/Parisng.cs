using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sample
{
    namespace Json
    {
        public class Json
        {
            // Using Text.Json
            public async Task FileJson_Ser_Des_Async(Person person)
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
        public class CSV
        {

        }

    }
    namespace Excell
    {
        public class Excell
        {

        }

    }
    namespace XML
    {
        public class Xml { }
    }
}
