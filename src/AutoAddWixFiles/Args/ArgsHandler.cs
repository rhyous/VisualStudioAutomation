using Rhyous.SimpleArgs;

namespace AutoAddWixFiles.Arguments
{
    // Add this line of code to Main() in Program.cs
    //
    //   new ArgsManager<ArgsHandler>().Start(args);
    //

    /// <summary>
    /// A class that implements IArgumentsHandler where command line
    /// arguments are defined.
    /// </summary>
    public sealed class ArgsHandler : ArgsHandlerBase
    {
        public override void InitializeArguments(IArgsManager argsManager)
        {
            Arguments.AddRange(new List<Argument>
            {
                new Argument
                {
                    Name = "FileList",
                    ShortName = "FL",
                    Description = "The list of wixproj files.",
                    Example = "{name}=.\\list\\list.txt",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "PrototypeDLL",
                    ShortName = "PD",
                    Description = "DLL that are already in WXS files.",
                    Example = "{name}=example.dll,example2.dll",
                    IsRequired = true
                },
                 new Argument
                {
                    Name = "DLL",
                    ShortName = "D",
                    Description = "DLL to add.",
                    Example = "{name}=example.dll",
                    IsRequired = true
                },
                

                new ConfigFileArgument(argsManager) // This is a special Argument type to allow for args in a file
            });
        }

        public override void HandleArgs(IReadArgs inArgsHandler)
        {
            base.HandleArgs(inArgsHandler);
            Console.WriteLine("I handled the args!!!");
        }
    }
}