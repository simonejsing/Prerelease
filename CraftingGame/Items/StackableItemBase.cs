using CraftingGame.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace CraftingGame.Items
{
    public abstract class StackableItemBase : ItemBase
    {
        public abstract bool Consumable { get; }
        public abstract bool Placable { get; }

        public virtual TerrainType PlacableTerrainType => TerrainType.NotGenerated;

        public virtual void Consume(PlayerObject player)
        {
        }
    }
}
