using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace CraftingGame.Items
{
    public sealed class BlockOfRock : ItemBase
    {
        public override string Name => nameof(BlockOfRock);
        public override bool Consumable => false;
        public override bool Placable => true;

        internal BlockOfRock()
        {
        }
    }
}
