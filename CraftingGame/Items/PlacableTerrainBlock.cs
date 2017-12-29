using CraftingGame.Physics;
using CraftingGame.State;
using System;
using System.Collections.Generic;
using System.Text;
using Terrain;

namespace CraftingGame.Items
{
    internal class PlacableTerrainBlock : TerrainModifyingItem
    {
        private TerrainType blockType;

        public PlacableTerrainBlock(GameState state, TerrainType type) : base(state)
        {
            this.blockType = type;
        }

        protected override bool ValidTerrainTarget(TerrainType type)
        {
            return type == TerrainType.Free;
        }

        public override void Attack()
        {
            if (Wielder != null && Target.Coordinate.HasValue && Wielder.Inventory.CanTake(blockType))
            {
                // Consume item from inventory
                Wielder.Inventory.Take(blockType);

                var targetCoord = Target.Coordinate.Value;
                var type = state.Terrain[targetCoord, Wielder.Plane].Type;
                if(type == TerrainType.Free)
                {
                    CooldownFrames = 10;

                    state.Terrain.Place(targetCoord, Wielder.Plane, blockType);
                }
            }
        }
    }
}
