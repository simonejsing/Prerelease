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
        private Vector2 gridSize;
        private readonly ITerrainGenerator terrainGenerator;
        private int plane;

        public ProceduralObjectGrid(ITerrainGenerator terrainGenerator, Vector2 gridSize, int plane)
        {
            this.terrainGenerator = terrainGenerator;
            this.gridSize = gridSize;
            this.plane = plane;
        }

        public ICollidableObject[] Neighbors(Object obj)
        {
            var neighbors = new ICollidableObject[9];

            int gridU = (int)Math.Floor((obj.Center.X) / gridSize.X);
            int gridV = (int)Math.Floor((200 /* arbitrary height... */ - obj.Center.Y) / gridSize.Y);
            int localIndex = 0;

            for (int v = gridV - 1; v <= gridV + 1; v++)
            {
                for (int u = gridU - 1; u <= gridU + 1; u++)
                {
                    var occupied = false;
                    var type = terrainGenerator[u, v, plane].Type;
                    if (type == TerrainType.NotGenerated)
                    {
                        // Force generate
                        terrainGenerator.Generate(u, v, plane);
                        type = terrainGenerator[u, v, plane].Type;
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
                        Position = new Vector2(u * gridSize.X, v * gridSize.Y),
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
        public ProceduralObjectManager(ITerrainGenerator terrainGenerator, Vector2 gridSize, int plane)
        {
            this.Blocks = new ProceduralObjectGrid(terrainGenerator, gridSize, plane);
        }

        public ICollidableObjectGrid Blocks { get; }
        public IEnumerable<ICollidableObject> CollisionOrder => Enumerable.Empty<ICollidableObject>();
    }
}