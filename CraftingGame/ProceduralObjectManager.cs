using System;
using System.Collections.Generic;
using System.Linq;
using CraftingGame.Physics;
using Terrain;
using VectorMath;
using Object = CraftingGame.Physics.Object;

namespace CraftingGame
{
    internal class ProceduralObjectGrid : ICollidableObjectGrid
    {
        private readonly Grid grid;
        private readonly ITerrainGenerator terrainGenerator;
        private readonly Plane plane;

        public ProceduralObjectGrid(ITerrainGenerator terrainGenerator, Grid grid, Plane plane)
        {
            this.terrainGenerator = terrainGenerator;
            this.grid = grid;
            this.plane = plane;
        }

        public ICollidableObject[] Neighbors(Object obj)
        {
            var neighbors = new ICollidableObject[9];

            var centerCoord = grid.PointToGridCoordinate(obj.Center);
            int localIndex = 0;

            for (var v = centerCoord.V - 1; v <= centerCoord.V + 1; v++)
            {
                for (var u = centerCoord.U - 1; u <= centerCoord.U + 1; u++)
                {
                    var coord = new Coordinate(u, v);
                    var occupied = false;
                    var type = terrainGenerator[coord, plane].Type;
                    if (type == TerrainType.NotGenerated)
                    {
                        // Force generate
                        terrainGenerator.Generate(coord, plane);
                        type = terrainGenerator[coord, plane].Type;
                    }
                    switch (type)
                    {
                        case TerrainType.NotGenerated:
                            // ???
                            break;
                        case TerrainType.Free:
                        case TerrainType.Sea:
                            occupied = false;
                            break;
                        default:
                            occupied = true;
                            break;
                    }

                    neighbors[localIndex] = new Block()
                    {
                        Position = grid.GridCoordinateToPoint(coord),
                        Occupied = occupied
                    };

                    localIndex++;
                }
            }

            return neighbors;
        }
    }

    internal class ProceduralObjectManager : IObjectManager
    {
        public ProceduralObjectManager(ITerrainGenerator terrainGenerator, Grid grid, Plane plane)
        {
            this.Blocks = new ProceduralObjectGrid(terrainGenerator, grid, plane);
        }

        public ICollidableObjectGrid Blocks { get; }
        public IEnumerable<ICollidableObject> CollisionOrder => Enumerable.Empty<ICollidableObject>();
    }
}