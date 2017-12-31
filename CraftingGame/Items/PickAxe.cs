using Contracts;
using CraftingGame.Actions;
using CraftingGame.Physics;
using CraftingGame.State;
using CraftingGame.Widgets;
using System;
using Terrain;
using VectorMath;

namespace CraftingGame.Items
{
    internal class PickAxe : TerrainModifyingItem
    {
        public override string Name => "Pick axe";

        public PickAxe(GameState state) : base(state)
        {
        }

        protected override bool ValidTerrainTarget(TerrainType type)
        {
            return type != TerrainType.Free;
        }

        public override void Attack()
        {
            if (Target.IsValid)
            {
                CooldownFrames = 30;

                var targetCoord = Target.Coordinate.Value;
                var type = state.Terrain[targetCoord, Wielder.Plane].Type;

                // Dig it!
                switch (type)
                {
                    case TerrainType.Dirt:
                    case TerrainType.Rock:
                        // Yes! Then get to work...
                        state.Terrain.Destroy(targetCoord, Wielder.Plane);

                        // Drop an item of the terrain type.
                        var item = ItemFactory.ItemFromTerrain(type);
                        if (item != null)
                        {
                            var size = new Vector2(10, 10);
                            var coordPos = state.Grid.GridCoordinateToPoint(targetCoord);
                            var position = coordPos + 0.5f * (state.Grid.Size - size);
                            var itemObject = new ItemObject(state.ActionQueue, item)
                            {
                                Plane = Wielder.Plane,
                                Position = position,
                                Size = size,
                                Color = TerrainWidget.TerrainColor(type)
                            };
                            itemObject.Collect += CollectAction.Invoke;
                            state.ActiveLevel.AddCollectableObjects(itemObject);
                        }
                        break;
                }
            }
        }
    }
}