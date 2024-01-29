
namespace Rhyous.AutoAddDLLtoWXSFiles
{
    internal interface IWxsFileFinder
    {
        IEnumerable<AddDetails> FindPotentialWxsFiles(IEnumerable<string> dirs);
    }
}