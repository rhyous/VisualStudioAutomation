﻿using Rhyous.SimpleArgs;

namespace Rhyous.AutoAddDLLtoWXSFiles.Arguments
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
                new Argument
                {
                    Name = "DoNothing",
                    ShortName = "dn",
                    Description = "Instead of doing anything, it will just list what it would have changed.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFSCheckout",
                    ShortName = "tfc",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "false"
                },
                new Argument
                {
                    Name = "TFdotExePath",
                    ShortName = "tf",
                    Description = "Check out the files with TFS.",
                    Example = "{name}=true",
                    IsRequired = false,
                    DefaultValue = "C:\\Program Files\\Microsoft Visual Studio\\2022\\Enterprise\\Common7\\IDE\\CommonExtensions\\Microsoft\\TeamFoundation\\Team Explorer\\TF.exe",
                    CustomValidation = File.Exists
                },
                new Argument
                {
                    Name = "ExcludeDirectories",
                    ShortName = "ed",
                    Description = "Directories to skip, semicolon separated.",
                    Example = "{name}=c:\\dev\folderToSkip",
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