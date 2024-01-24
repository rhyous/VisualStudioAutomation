using System.Collections.Generic;

namespace Rhyous.RemoveProjectReferences
{
    internal interface ITFSCheckout
    {
        void Checkout(IEnumerable<string> filesToCheckout);
    }
}