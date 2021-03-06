﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ISceeneFactory
    {
        ISceene Create(IRenderer renderer, IUserInterface ui, ActionQueue actionQueue);
    }
}
