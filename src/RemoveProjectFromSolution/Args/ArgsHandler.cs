using Rhyous.SimpleArgs;
using System;
using System.Collections.Generic;

namespace RemoveProjectFromSolution.Arguments
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
                    Name = "SolutionListFile",
                    ShortName = "L",
                    Description = "The path to a file containing a list of solutions.",
                    Example = "{name}=c:\\path\\to\\Solution.sln",
                    Action = (value) => { Console.WriteLine(value); },
                    IsRequired = false
                },
                new Argument
                {
                    Name = "P",
                    ShortName = "ProjectName",
                    Description = "ProjectName",
                    Example = "{name}=NonDefaultValue",
                    IsRequired = true
                },
                // Add more args here
                // new Argument
                // {
                //     Name = "NextArg",
                //     ShortName = "N",
                //     Description = "This is the next arg you are going to add.",
                //     Example = "{name}=true",
                //     DefaultValue = "false"
                //     AllowedValues = CommonAllowedValues.TrueFalse
                // },
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