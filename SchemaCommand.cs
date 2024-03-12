using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NJsonSchema.CLI
{
    [Verb("schema", HelpText = "Manipulate schemas.")]
    class SchemaCommand
    {
        [Verb("to-json", HelpText = "Generate a json-schema from a .NET assembly and class(es)")]
        public class ToJsonCommand
        {
            [Option("assembly-path", Required = true, HelpText = "The path to the .NET assembly.")]
            public IEnumerable<string> AssemblyPath { get; set; }

            [Option("type-name", HelpText = "The full name of the .NET type.", Default = "Options$")]
            public string TypeName { get; set; }

            [Option("output", HelpText = "The output file path.")]
            public string Output { get; set; }

            public int OnExecute()
            {
                var schemaBuilder = new StringBuilder();

                foreach (var anAssemblyPath in AssemblyPath)
                {
                    Console.WriteLine(anAssemblyPath);
                    var assembly = Assembly.LoadFile(anAssemblyPath);

                    if (Regex.IsMatch(TypeName, "^[A-Za-z_0-9]+$"))
                    {
                        var type = assembly.GetType(TypeName);
                        Console.WriteLine(type.FullName);

                        var schema = JsonSchema.FromType(type);
                        var schemaData = schema.ToJson();
                        schemaBuilder.Append(schemaData);
                    }
                    else
                    {
                        var regex = new Regex(TypeName);
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
}
