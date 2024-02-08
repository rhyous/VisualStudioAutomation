using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using EnvDTE90;
using Rhyous.AddProjectsToSolution.Arguments;
using Rhyous.SimpleArgs;
using Rhyous.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rhyous.AddProjectsToSolution
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var solutionFullPath = Args.Value("S");
            var searchPattern = Args.Value("P");
            var searchDirectory = Args.Value("SD");
            var fileExtension = Args.Value("FE");
            string[] projectsToRemove;

            var projectsToAdd = new List<string>();
            var csprojFiles = Directory.GetFiles(searchDirectory, $"*.{fileExtension}", SearchOption.AllDirectories);
            foreach (var csprojPath in csprojFiles)
            {
                if (File.ReadAllText(csprojPath).Contains(searchPattern))
                {
                    projectsToAdd.Add(csprojPath);
                }
            }


            // Get an instance of the Visual Studio IDE
            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE.17.0");
            var set = false;
            while (!set)
            {
                try { dte.SuppressUI = true; set = true; }
                catch (COMException) { System.Threading.Thread.Sleep(100); }
            }
            var retry = new Retry();
            Solution4 solution = retry.Run((DTE2 dte2) => (Solution4)dte2.Solution, dte);

            // Create an instance of SolutionEvents
            SolutionEvents solutionEvents = dte.Events.SolutionEvents;

            // Handle the ProjectAdded event
            solutionEvents.ProjectAdded += (projectAdded) =>
            {
                // Set SuppressUI to true to suppress the "Get projects from source control" popup
                dte.SuppressUI = true;
            };
            try { solution.Open(solutionFullPath); }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to open {solutionFullPath}");
                Console.WriteLine(e.Message);
            }

            set = false;
            Window activeWindow = null;
            while (!set)
            {
                try { activeWindow = dte.ActiveWindow; set = true; }
                catch (COMException) { System.Threading.Thread.Sleep(100); }
            }

            var projectAdder = new ProjectAdder(dte, solution, retry);
            projectAdder.Add(solutionFullPath, projectsToAdd);

            // Save and close the solution
            try { solution.SaveAs(solutionFullPath); }
            catch (Exception e)
            {
                Console.WriteLine($"{solutionFullPath}: Failed to save.");
                Console.WriteLine(e.Message);
            }
            //solution.Close();
        }
    }

    internal class ProjectAdder
    {
        private readonly DTE2 _dte;
        private readonly Solution4 _solution;
        private readonly Retry _retry;

        public ProjectAdder(DTE2 dte,
                            Solution4 solution,
                            Retry retry)
        {
            _dte = dte;
            _solution = solution;
            _retry = retry;
        }

        public void Add(string solutionFullPath, IEnumerable<string> projectsToAdd)
        {

            // Find the project to delete
            foreach (var projectToAdd in projectsToAdd)
            {
                 _dte.ExecuteCommand($"File.AddExistingProject \"{projectToAdd}\"");
            }
        }
    }
}
