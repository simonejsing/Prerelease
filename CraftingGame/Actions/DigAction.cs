using Contracts;
using CraftingGame.Physics;
using CraftingGame.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;

namespace CraftingGame.Actions
{
    public class DigAction
    {
        private readonly ActionQueue actionQueue;
        private readonly CollectAction collectAction;
        private readonly GameState state;
        private readonly Grid grid;
        private readonly IModifiableTerrain terrain;

        public DigAction(ActionQueue actionQueue, CollectAction collectAction, GameState state, Grid grid, IModifiableTerrain terrain)
        {
            this.actionQueue = actionQueue;
            this.collectAction = collectAction;
            this.state = state;
            this.grid = grid;
            this.terrain = terrain;
        }

        public void Invoke(object sender, PlayerObject player)
        {
            if (player.LookDirection.TooSmall)
            {
                return;
            }

            // Find the spot below and in front of the player's center
            var playerCoord = grid.PointToGridCoordinate(player.Center);
            Coordinate digCoord = LookingAtCoordinate(playerCoord, player.LookDirection);

            // Can the player dig here?
            terrain.Generate(digCoord, player.Plane);
            var type = terrain[digCoord, player.Plane].Type;
            if (type == TerrainType.Free)
            {
                // No, try below
                digCoord = new Coordinate(digCoord.U, digCoord.V - 1);
                terrain.Generate(digCoord, player.Plane);
                type = terrain[digCoord, player.Plane].Type;
            }

            // Dig it!
            switch (type)
            {
                case TerrainType.Dirt:
                case TerrainType.Rock:
                    // Yes! Then get to work...
                    terrain.Destroy(digCoord, player.Plane);

                    // Drop an item of the terrain type.
                    var item = ItemFactory.FromTerrain(type);
                    if (item != null)
                    {
                        var size = new Vector2(10, 10);
                        var coordPos = grid.GridCoordinateToPoint(digCoord);
                        var position = coordPos + 0.5f * (grid.Size - size);
                        var itemObject = new ItemObject(actionQueue, item)
                        {
                            Plane = player.Plane,
                            Position = position,
                            Size = size,
                        };
                        itemObject.Collect += collectAction.Invoke;
                        state.ActiveLevel.AddCollectableObjects(itemObject);
                    }
                    break;
            }
        }

        private static Coordinate LookingAtCoordinate(Coordinate coord, Vector2 lookDirection)
        {
            var lookAngleUnit = lookDirection.Angle / Math.PI;

            // u-offset
            if (lookAngleUnit > -3f / 8f && lookAngleUnit < 3f / 8f)
            {
                coord += new Coordinate(1, 0);
            }
            else if (lookAngleUnit > 5f / 8f || lookAngleUnit < -5f / 8f)
            {
                coord += new Coordinate(-1, 0);
            }

            // v-offset
            if (lookAngleUnit > 1f / 8f && lookAngleUnit < 7f / 8f)
            {
                coord += new Coordinate(0, 1);
            }
            else if (lookAngleUnit < -1f / 8f && lookAngleUnit > -7f / 8f)
            {
                coord += new Coordinate(0, -1);
            }

            return coord;
        }

    }
}
