using System.Collections.Generic;

namespace Rhyous.AutoAddDLLtoWXSFiles
{
    internal interface ITFSCheckout
    {
        void Checkout(IEnumerable<string> filesToCheckout);
    }
}