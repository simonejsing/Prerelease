using CraftingGame.Physics;
using CraftingGame.State;
using System;
using System.Collections.Generic;
using System.Text;
using Terrain;
using VectorMath;

namespace CraftingGame.Items
{
    public class TerrainTarget
    {
        private readonly GameState state;
        private readonly Func<TerrainType, bool> validTerrain;
        private PlayerObject wielder;

        public Coordinate? Coordinate { get; private set; }

        public TerrainTarget(GameState state, Func<TerrainType, bool> validTerrain)
        {
            this.state = state;
            this.validTerrain = validTerrain;
        }

        public void Update()
        {
            Coordinate = FindTarget();
        }

        public void Equip(PlayerObject player)
        {
            this.wielder = player;
        }

        private Coordinate? FindTarget()
        {
            // Set target
            if (wielder == null || wielder.LookDirection.TooSmall)
            {
                return null;
            }

            // Find the spot below and in front of the player's center
            var wielderCoord = state.Grid.PointToGridCoordinate(wielder.Center);
            var targetCoord = LookingAtCoordinate(wielderCoord, wielder.LookDirection);

            // Can the player dig here?
            state.Terrain.Generate(targetCoord, wielder.Plane);
            var type = state.Terrain[targetCoord, wielder.Plane].Type;
            if (!validTerrain(type))
            {
                return null;
                // No, try below
                /*targetCoord = new Coordinate(targetCoord.U, targetCoord.V - 1);
                state.Terrain.Generate(targetCoord, wielder.Plane);
                type = state.Terrain[targetCoord, wielder.Plane].Type;*/
            }

            return targetCoord;
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
