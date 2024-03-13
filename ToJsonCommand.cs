using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NJsonSchema.CLI
{
    [Verb("to-json", HelpText = "Generate a json-schema from a .NET assembly and class(es). See https://github.com/RicoSuter/NJsonSchema/wiki/JsonSchemaGenerator.")]
    public class ToJsonCommand
    {
        [Option('a', "assembly-path", Required = true, HelpText = "The path to the .NET assembly.")]
        public IEnumerable<string> AssemblyPath { get; set; }

        [Option('t', "type-name", HelpText = "The full name of the .NET type without namespace, or a regex.", Default = "Options$")]
        public IEnumerable<string> TypeName { get; set; }

        [Option('o', "output", HelpText = "The output file path.")]
        public string Output { get; set; }

        public int OnExecute()
        {
            var schemaBuilder = new StringBuilder();

            foreach (var anAssemblyPath in AssemblyPath)
            {
                Console.WriteLine(anAssemblyPath);
                var assembly = Assembly.LoadFrom(anAssemblyPath);

                foreach (var typeName in TypeName)
                if (Regex.IsMatch(typeName, "^[A-Za-z_0-9]+$"))
                {
                    var type = assembly.GetTypes().First(type => type.Name.EndsWith(typeName));
                    Console.WriteLine(type.FullName);

                    var schema = JsonSchema.FromType(type);
                    var schemaData = schema.ToJson();
                    schemaBuilder.Append(schemaData);
                }
                else
                {
                    var regex = new Regex(typeName);
                    var matchingTypes = assembly.GetTypes().Where(type => regex.IsMatch(type.Name));
                    if (matchingTypes.Count() == 0)
                    {
                        Console.WriteLine("No matching types found.");
                        return 1;
                    }
                    foreach (var type in matchingTypes)
                    {
                        var schema = JsonSchema.FromType(type);
                        Console.WriteLine(type.FullName);
                        var schemaData = schema.ToJson();
                        schemaBuilder.Append(schemaData);
                    }
                }
            }
            if (Output != null)
            {
                System.IO.File.WriteAllText(Output, schemaBuilder.ToString());
            }
            else
            {
                Console.WriteLine(schemaBuilder.ToString());
                Console.ReadKey();
            }
            return 0;
        }
    }
}
