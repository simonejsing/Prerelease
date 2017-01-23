using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;

namespace HubSceene
{
    public class HubSceeneFactory : ISceeneFactory
    {
        public ISceene Create(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue)
        {
            return new HubSceene(renderer, ui, actionQueue);
        }
    }
}
