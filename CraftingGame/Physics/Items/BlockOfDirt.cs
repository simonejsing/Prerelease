using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CraftingGame.Physics.Items
{
    public sealed class BlockOfDirt : ItemBase
    {
        public override string Name => nameof(BlockOfDirt);
        public override bool Consumable => false;
        public override bool Placable => true;

        internal BlockOfDirt()
        {
        }
    }
}
