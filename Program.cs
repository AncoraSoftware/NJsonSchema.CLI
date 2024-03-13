using CommandLine;

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
