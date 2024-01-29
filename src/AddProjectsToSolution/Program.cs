using EnvDTE;
using EnvDTE100;
using EnvDTE80;
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
            foreach (var csprojPath in Directory.GetFiles(searchDirectory, $"*.{fileExtension}", SearchOption.AllDirectories))
            {
                if (File.ReadAllText(csprojPath).Contains(searchPattern))
                {
                    projectsToAdd.Add(csprojPath);
                }
            }

            var projectAdder = new ProjectAdder(new Retry());
            await projectAdder.AddAsync(solutionFullPath, projectsToAdd);
        }
    }

    internal class ProjectAdder
    {
        private readonly Retry _retry;

        public ProjectAdder(Retry retry)
        {
            _retry = retry;
        }

        public async Task AddAsync(string solutionFullPath, IEnumerable<string> projectsToAdd)
        {
            // Get an instance of the Visual Studio IDE
            DTE2 dte = (DTE2)Marshal.GetActiveObject("VisualStudio.DTE.17.0");
            var set = false;
            while (!set)
            {
                try { dte.SuppressUI = true; set = true; }
                catch (COMException) { System.Threading.Thread.Sleep(100); }
            }
            Solution4 solution = _retry.Run((DTE2 dte2) => (Solution4)dte2.Solution, dte);

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

            // Find the project to delete
            foreach (var projectToAdd in projectsToAdd)
            {
                solution.AddFromFile(projectToAdd);
            }

            // Save and close the solution
            try { solution.SaveAs(solutionFullPath); }
            catch (Exception e)
            {
                Console.WriteLine($"{solutionFullPath}: Failed to save.");
                Console.WriteLine(e.Message);
            }
            solution.Close();
        }
    }
}
