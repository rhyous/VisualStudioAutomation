using EnvDTE;
using EnvDTE100;
using EnvDTE80;
using Rhyous.RemoveProjectFromSolution.Arguments;
using Rhyous.SimpleArgs;
using Rhyous.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Rhyous.RemoveProjectFromSolution
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            new ArgsManager<ArgsHandler>().Start(args);
            var searchDirectory = Args.Value("SD");
            var solutionFullPath = Args.Value("S");
            var projectsToRemove = Args.Value("P").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToHashSet();

            var slnFiles = new List<string>();
            foreach (var csprojPath in Directory.GetFiles(searchDirectory, $"*.sln", SearchOption.AllDirectories))
            {
                var text = File.ReadAllText(csprojPath);
                if (projectsToRemove.Any(p=> text.Contains(p)))
                {
                    slnFiles.Add(csprojPath);
                }
            }
            var projectRemover = new ProjectRemover(new Retry());
            if (!string.IsNullOrEmpty(solutionFullPath))
                await projectRemover.RemoveAsync(solutionFullPath, projectsToRemove);
            else if (slnFiles.Any())
                await projectRemover.RemoveAllAsync(slnFiles, projectsToRemove);
        }
    }

    internal class ProjectRemover
    {
        private readonly Retry _retry;

        public ProjectRemover(Retry retry)
        {
            _retry = retry;
        }
        public async Task RemoveAllAsync(IEnumerable<string> solutionList, IEnumerable<string> projectsToRemove)
        {
            foreach (var slnPath in solutionList)
            {
                await RemoveAsync(slnPath, projectsToRemove);
            }
        }

        public async Task RemoveAsync(string solutionFullPath, IEnumerable<string> projectsToRemove)
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
            foreach (var projectToRemove in projectsToRemove)
            {

                Project project = FindProject(projectToRemove, solution);

                // Delete the project
                if (project != null)
                {
                    solution.Remove(project);
                }
                else
                {
                    Console.WriteLine($"{solutionFullPath}: Project {projectToRemove} not found!");
                }
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

        private Project FindProject(string projectName, Solution4 solution)
        {
            foreach (Project p in solution.Projects)
            {
                //Console.WriteLine(p.Name);

                var set = false;
                while (!set)
                {
                    try
                    {
                        if (p.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                        {
                            return p;
                        }

                        // If the project is located within a solution folder, search within the folder
                        if (p.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                        {
                            var found = FindProject(projectName, p);
                            if (found != null)
                                return found;
                        }
                        set = true;
                    }
                    catch (COMException) { System.Threading.Thread.Sleep(100); }
                }
            }
            return null;
        }

        private static Project FindProject(string projectName, Project p)
        {
            foreach (ProjectItem pi in p.ProjectItems)
            {
                //Console.WriteLine(p.Name);
                var set = false;
                while (!set)
                {
                    try
                    {
                        if (pi != null && pi.SubProject != null && pi.SubProject.Name.Equals(projectName, StringComparison.OrdinalIgnoreCase))
                        {
                            return pi.SubProject;
                        };
                        set = true;
                    }
                    catch (COMException) { System.Threading.Thread.Sleep(100); }
                }


                // If the project is located within a solution folder, search within the folder
                if (pi != null && pi.SubProject != null && pi.SubProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    var found = FindProject(projectName, pi.SubProject);
                    if (found != null)
                        return found;
                }
            }

            return null;
        }
    }
}
