using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CraftingGame.Physics;
using Terrain;
using VectorMath;
using Contracts;

namespace CraftingGame
{
    internal class ProceduralLevel
    {
        private readonly CachedTerrainGenerator terrain;

        public string Name => Plane.W.ToString();
        public ITerrainGenerator Terrain => terrain;
        public Plane Plane { get; }
        public Vector2 SpawnPoint { get; private set; }
        public LevelState State { get; private set; }

        private readonly Grid grid;

        public ProceduralLevel(CachedTerrainGenerator terrain, Grid grid, Plane plane)
        {
            this.terrain = terrain;
            this.Plane = plane;
            this.grid = grid;
        }

        public void Load(ViewportProjection view)
        {
            // Initialize the starting sector(s) based on the active view
            var activeView = view.Projection;
            var points = new[] { activeView.TopLeft, activeView.BottomLeft, activeView.TopRight, activeView.BottomRight };
            foreach (var point in points)
            {
                var coord = grid.PointToGridCoordinate(point);
                Terrain.Generate(coord, Plane);
            }

            // Pre-load visible terrain
            terrain.Update(-1);

            // Spawn the player on the first free coordinate (0,v)
            var spawnCoord = new Coordinate(0, -Terrain.MaxDepth);
            var type = TerrainType.NotGenerated;
            do
            {
                spawnCoord.V++;
                Terrain.Generate(spawnCoord, Plane);
                type = Terrain[spawnCoord, Plane].Type;
            } while (type != TerrainType.Free);

            SpawnPoint = grid.GridCoordinateToPoint(spawnCoord);
            this.State = new LevelState(Name, SpawnPoint);
        }
    }
}
