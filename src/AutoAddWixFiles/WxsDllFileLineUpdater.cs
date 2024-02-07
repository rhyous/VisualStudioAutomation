namespace Rhyous.AutoAddDLLtoWXSFiles
{
    internal class WxsDllFileLineUpdater
    {
        const string Template = "{0}<File Id=\"{1}\" Name=\"{1}\" Source=\"$(var.{2}.TargetDir){1}\" />";

        private readonly ISettings _settings;

        public WxsDllFileLineUpdater(ISettings settings)
        {
            _settings = settings;
        }
        public async Task AddAsync(AddDetails addDetails)
        {
            var lines = File.ReadAllLines(addDetails.File);
            var newFileLines = new List<string>();
            for (int i = 0; i < addDetails.FoundDllLine; i++)
            {
                newFileLines.Add(lines[i]);
            }
            // handle line 
            newFileLines.Add(string.Format(Template, addDetails.LeadingWhitespace, _settings.Dll, addDetails.ProjectName));
            for (int i = addDetails.FoundDllLine + 1; i < lines.Length; i++)
            {
                    newFileLines.Add(lines[i]);
            }
            if (!_settings.DoNothing)
                await File.WriteAllLinesAsync(addDetails.File, newFileLines);
        }
    }
}
