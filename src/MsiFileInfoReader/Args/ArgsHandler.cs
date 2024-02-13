using Rhyous.SimpleArgs;
using System;
using System.Collections.Generic;

namespace Rhyous.MsiFileInfoReader.Arguments
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
                    Name = "TokenizedFolder",
                    ShortName = "TF",
                    Description = "The folder path with a tokenized directory for MSI compare.",
                    Example = "{name}=c:\\{0}",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "LeftFolder",
                    ShortName = "LF",
                    Description = "The left folder for MSI compare.",
                    Example = "{name}=left",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "RightFolder",
                    ShortName = "RF",
                    Description = "The right folder for MSI compare.",
                    Example = "{name}=right",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "File",
                    ShortName = "F",
                    Description = "The file to find in the MSI and compare",
                    Example = "{name}=example.dll",
                    IsRequired = true
                },
                new Argument
                {
                    Name = "Version",
                    ShortName = "V",
                    Description = "The new expected Version of the file.",
                    Example = "{name}=example.dll",
                    IsRequired = false
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