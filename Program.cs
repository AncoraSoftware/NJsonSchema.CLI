using CommandLine;
using static NJsonSchema.CLI.SchemaCommand;

namespace NJsonSchema.CLI
{
    class Program
    {
        public static int Main(string[] args) => Parser.Default
            .ParseArguments<ToJsonCommand>(args)
            .MapResult(
                (ToJsonCommand opts) => opts.OnExecute(),
                errs => 1);

    }
}
