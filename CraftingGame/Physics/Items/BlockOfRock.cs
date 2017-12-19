using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using VectorMath;

namespace CraftingGame.Physics.Items
{
    public sealed class BlockOfRock : Item
    {
        public override string Name => nameof(BlockOfRock);
        public override bool Consumable => false;
        public override bool Placable => true;

        public BlockOfRock()
        {
        }

        static BlockOfRock()
        {
            ItemFactory.Register(nameof(BlockOfRock), () => new BlockOfRock());
        }
    }
}
