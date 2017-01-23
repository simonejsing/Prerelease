using System.Collections.Generic;
using System.Linq;

namespace Prerelease.Main
{
    internal class DialogPage
    {
        public IEnumerable<string> Lines { get; }

        public DialogPage(IEnumerable<string> lines)
        {
            this.Lines = lines.ToArray();
        }
    }
}