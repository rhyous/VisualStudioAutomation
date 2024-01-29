namespace Rhyous.AutoAddDLLtoWXSFiles
{
    internal interface ISettings
    {
        /// <summary>The Dll to add to the wxs files.</summary>
        string Dll { get; set; }
        /// <summary>The prototype dlls to match syntax of when add the <see cref="Dll"/> to the wxs files.</summary>
        HashSet<string> PrototypeDlls { get; set; }
        /// <summary>
        /// Whether to actually make the change, or whether to just lists the files that would have been changed.
        /// </summary>
        bool DoNothing { get; set; }

        /// <summary>
        /// The path to TF.exe
        /// </summary>
        string TFPath { get; set; }

        /// <summary>
        /// Whether to checkout from TFS or not.
        /// </summary>
        bool CheckoutFromTFS { get; set; }

        /// <summary>
        /// Directories to skip.
        /// </summary>
        HashSet<string> ExcludeDirs { get; set; }
    }
}