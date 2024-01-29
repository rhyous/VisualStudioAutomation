namespace Rhyous.AutoAddDLLtoWXSFiles
{
    internal class Settings : ISettings
    {
        /// <summary>The Dll to add to the wxs files.</summary>
        public string Dll { get; set; }
        /// <summary>The prototype dlls to match syntax of when add the <see cref="Dll"/> to the wxs files.</summary>
        public HashSet<string> PrototypeDlls { get; set; }
        /// <summary>
        /// Whether to actually make the change, or whether to just lists the files that would have been changed.
        /// </summary>
        public bool DoNothing { get; set; }

        /// <summary>
        /// The path to TF.exe
        /// </summary>
        public string TFPath { get; set; }

        /// <summary>
        /// Whether to checkout from TFS or not.
        /// </summary>
        public bool CheckoutFromTFS { get; set; }

        /// <summary>
        /// Directories to skip.
        /// </summary>
        public HashSet<string> ExcludeDirs { get; set; }
    }
}
