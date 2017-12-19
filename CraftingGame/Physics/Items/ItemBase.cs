using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingGame.Physics.Items
{
    public abstract class ItemBase
    {
        public abstract string Name { get; }
        public abstract bool Consumable { get; }
        public abstract bool Placable { get; }

        public virtual void Consume(PlayerObject player)
        {
        }
    }
}
