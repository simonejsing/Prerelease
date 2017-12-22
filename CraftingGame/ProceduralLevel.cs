using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;
using VectorMath;
using Contracts;
using CraftingGame.State;

namespace CraftingGame
{
    internal class ProceduralLevel
    {
        private readonly CachedTerrainGenerator terrain;

        public string Name => "procedural";
        public IModifiableTerrain Terrain => terrain;
        public Vector2 SpawnPoint { get; private set; }
        public LevelState State { get; private set; }

        private readonly Grid grid;

        public ProceduralLevel(CachedTerrainGenerator terrain, Grid grid)
        {
            this.terrain = terrain;
            this.grid = grid;
        }

        public void Load(ViewportProjection view, Plane plane)
        {
            // Initialize the starting sector(s) based on the active view
            var activeView = view.Projection;
            var points = new[] { activeView.TopLeft, activeView.BottomLeft, activeView.TopRight, activeView.BottomRight };
            foreach (var point in points)
            {
                var coord = grid.PointToGridCoordinate(point);
                Terrain.Generate(coord, plane);
            }

            // Pre-load visible terrain
            terrain.Update(-1);

            // Spawn the player on the first free coordinate (0,v)
            var spawnCoord = new Coordinate(0, -Terrain.MaxDepth);
            var type = TerrainType.NotGenerated;
            do
            {
                spawnCoord.V++;
                Terrain.Generate(spawnCoord, plane);
                type = Terrain[spawnCoord, plane].Type;
            } while (type != TerrainType.Free);

            SpawnPoint = grid.GridCoordinateToPoint(spawnCoord);
            this.State = new LevelState(Name, SpawnPoint);
        }

        internal void Update()
        {
            terrain.Update(200);
        }
    }
}
