using Rhyous.SimpleArgs;
using System;
using System.Collections.Generic;

namespace AddProjectsToSolution.Arguments
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
                    Name = "Solution",
                    ShortName = "S",
                    Description = "The path to the solution.",
                    Example = "{name}=c:\\path\\to\\Solution.sln",
                    Action = (value) => { Console.WriteLine(value); },
                    IsRequired = false
                },
                new Argument
                {
                    Name = "Pattern",
                    ShortName = "P",
                    Description = "The string pattern to search for.",
                    Example = "{name}=PatternToSearch",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "SearchDirectory",
                    ShortName = "SD",
                    Description = "The search directory.",
                    Example = "{name}=c:\\Some\\Dir",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "FileExtension",
                    ShortName = "FE",
                    Description = "The file extension.",
                    Example = "{name}=csproj",
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