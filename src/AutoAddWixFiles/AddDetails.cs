namespace Rhyous.AutoAddDLLtoWXSFiles
{
    public class AddDetails
    {
        /// <summary>The wxs file to add the dll to.</summary>
        public string File { get; set; }
        /// <summary>The project string to use.</summary>
        public string ProjectName { get; set; }
        /// <summary>The leading whitespace.</summary>
        public string LeadingWhitespace { get; set; }
        /// <summary>The line number to add this line after.</summary>
        public int AfterLineNumber { get; set; }
        /// <summary>FoundLine start.</summary>
        public int FoundDllLineStart { get; set; }
        /// <summary>FoundLine end.</summary>
        public int FoundDllLineEnd { get; set; }

    }
}
