using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame.Items.Creatable
{
    public sealed class BlockOfDirt : StackableItemBase
    {
        public override string Name => nameof(BlockOfDirt);
        public override bool Consumable => false;
        public override bool Placable => true;

        public override TerrainType PlacableTerrainType => TerrainType.Dirt;

        internal BlockOfDirt()
        {
        }
    }
}
