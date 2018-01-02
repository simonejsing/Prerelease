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

        public override string Name => $"Block ({blockType.ToString()})";

        public override int Quantity => Wielder?.Inventory?.Count(blockType) ?? 0;

        public PlacableTerrainBlock(GameState state, TerrainType type) : base(state)
        {
            this.blockType = type;
        }

        protected override bool ValidTerrainTarget(Coordinate coord, TerrainType type)
        {
            if (type != TerrainType.Free)
                return false;

            return AdjacentBlock(coord);
        }

        private bool AdjacentBlock(Coordinate centerCoord)
        {
            foreach (var coord in centerCoord.AdjacentCoordinates())
            {
                if (state.Terrain[coord, Wielder.Plane].Type != TerrainType.Free)
                    return true;
            }

            return false;
        }

        public override void Attack()
        {
            if (Wielder != null && Wielder.Inventory.CanTake(blockType) && Target.IsValid)
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
