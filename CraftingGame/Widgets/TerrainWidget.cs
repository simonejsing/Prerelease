using Contracts;
using System;
using System.Collections.Generic;
using Terrain;
using VectorMath;

namespace CraftingGame.Widgets
{
    internal class PrerenderedTerrainSector
    {
        public ISprite Sprite { get; }

        public PrerenderedTerrainSector(ISprite sprite)
        {
            this.Sprite = sprite ?? throw new ArgumentNullException(nameof(sprite));
        }
    }

    internal class TerrainWidget
    {
        private readonly IRenderer renderer;
        private readonly CachedTerrainGenerator terrain;

        // Pre-rendered sectors
        private readonly QuadTree<PrerenderedTerrainSector> sectorSprites = new QuadTree<PrerenderedTerrainSector>();
        //private readonly Dictionary<Voxel, ISprite> sectorSprites = new Dictionary<Voxel, ISprite>();

        public TerrainWidget(IRenderer renderer, CachedTerrainGenerator terrain)
        {
            this.renderer = renderer;
            this.terrain = terrain;
        }

        public void Prerender(Grid grid, ViewportProjection view, Plane plane)
        {
            // TODO: Limit to pre-rendering one sector per cycle, this causes lag - could we pre-render incrementally?
            ForeachVisibleSector(grid, view, plane, voxel => {
                var sectorSprite = sectorSprites[voxel];
                if(sectorSprite != null)
                {
                    // TODO: Check for pending modifications
                    return;
                }

                // Render to sprite and cache.
                var sprite = PrerenderSectorToTexture(grid, voxel);
                sectorSprite = new PrerenderedTerrainSector(sprite);
                sectorSprites.Add(voxel, sectorSprite);
            });
        }

        public void Render(Grid grid, ViewportProjection view, Plane plane)
        {
            /*var activeView = view.Projection;
            var startCoord = grid.PointToGridCoordinate(activeView.BottomLeft);
            var numberCells = grid.PointToGridCoordinate(activeView.Size);

            for (var v = 0; v <= numberCells.V; v++)
            {
                for (var u = 0; u <= numberCells.U; u++)
                {
                    var coord = startCoord + new Coordinate(u, v);
                    var block = terrain[coord, plane];
                    var position = grid.GridCoordinateToPoint(coord);
                    RenderTerrainBlock(view, block.Type, position, grid.Size);
                }
            }*/

            // TODO: This does not account for terrain modifications! The pre-rendered textures needs to be updated with each modification.

            ForeachVisibleSector(grid, view, plane, voxel => {
                var bottomLeft = new Vector2(voxel.U * TerrainSector.SectorWidth, voxel.V * TerrainSector.SectorHeight) * grid.Size;
                var size = new Vector2(TerrainSector.SectorWidth, TerrainSector.SectorHeight) * grid.Size;

                var sectorSprite = sectorSprites[voxel];
                if(sectorSprite != null)
                //if (sectorSprites.TryGetValue(voxel, out ISprite sectorSprite))
                {
                    // Render the sector sprite
                    RenderTexture(view, bottomLeft, size, sectorSprite.Sprite);
                }
                else
                {
                    // Fuck, we did not find the sector in the cache... fallback to rendering
                    // Render to sprite and cache.
                    // TODO: Fallback strategy?
                }

                // Render sector boundary
                RenderVector(view, bottomLeft, Matrix2x2.ProjectX * size, Color.Red, 3.0f);
                RenderVector(view, bottomLeft, Matrix2x2.ProjectY * size, Color.Red, 3.0f);
                RenderVector(view, bottomLeft + size, (Matrix2x2.ProjectX * size).FlipX, Color.Red, 3.0f);
                RenderVector(view, bottomLeft + size, Matrix2x2.ProjectY * size, Color.Red, 3.0f);
            });
        }

        private void ForeachVisibleSector(Grid grid, ViewportProjection view, Plane plane, Action<Voxel> action)
        {
            // Find visible sectors
            var activeView = view.Projection;
            var startCoord = grid.PointToGridCoordinate(activeView.BottomLeft);
            var endCoord = grid.PointToGridCoordinate(activeView.TopRight);
            var startIndex = terrain.SectorIndex(startCoord);
            var endIndex = terrain.SectorIndex(endCoord);
            for (var v = startIndex.V; v <= endIndex.V; v++)
            {
                for (var u = startIndex.U; u <= endIndex.U; u++)
                {
                    var sectorVoxel = new Voxel(u, v, plane.W);
                    action(sectorVoxel);
                }
            }
        }

        private void RenderTerrainBlock(ViewportProjection view, TerrainType type, Vector2 position, Vector2 size)
        {
            var color = TerrainColor(type);
            switch (type)
            {
                case TerrainType.Dirt:
                case TerrainType.Rock:
                case TerrainType.Bedrock:
                case TerrainType.Sea:
                    RenderRectangle(view, position, size, color);
                    break;
                case TerrainType.Free:
                    break;
            }
        }

        private Color TerrainColor(TerrainType type)
        {
            switch (type)
            {
                case TerrainType.Dirt:
                    return Color.Yellow;
                case TerrainType.Rock:
                    return Color.Gray;
                case TerrainType.Bedrock:
                    return Color.DarkGray;
                case TerrainType.Sea:
                    return Color.Blue;
                default:
                    return Color.Black;
            }
        }

        private ISprite PrerenderSectorToTexture(Grid grid, Voxel index)
        {
            int textureWidth = TerrainSector.SectorWidth * (int)grid.Size.X;
            int textureHeight = TerrainSector.SectorHeight * (int)grid.Size.Y;
            var size = new Vector2(textureWidth, textureHeight);
            return renderer.RenderToTexture(textureWidth, textureHeight, () => PrerenderSector(grid, index, size));
        }

        private void PrerenderSector(Grid grid, Voxel index, Vector2 textureSize)
        {
            var textureView = ViewportProjection.ToTexture(textureSize);
            var startCoord = terrain.SectorPosition(index.Coordinate);
            for (var v = 0; v < TerrainSector.SectorHeight; v++)
            {
                for (var u = 0; u < TerrainSector.SectorHeight; u++)
                {
                    var localCoord = new Coordinate(u, v);
                    var worldCoord = startCoord + localCoord;
                    terrain.Generate(worldCoord, index.Plane);
                    var block = terrain[worldCoord, index.Plane];
                    var position = new Vector2(u, v) * grid.Size;
                    RenderTerrainBlock(textureView, block.Type, position, grid.Size);
                }
            }
        }

        private void RenderVector(ViewportProjection view, Vector2 point, Vector2 vector, Color color, float thickness)
        {
            renderer.RenderVector(view.MapToViewport(point), view.MapSizeToViewport(vector), color, thickness);
        }

        private void RenderRectangle(ViewportProjection view, Vector2 point, Vector2 size, Color color)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            renderer.RenderRectangle(view.MapToViewport(point), view.MapSizeToViewport(size.FlipY), color);
        }

        private void RenderTexture(ViewportProjection view, Vector2 point, Vector2 size, ISprite sprite)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            renderer.RenderOpagueSprite(sprite, view.MapToViewport(point), view.MapSizeToViewport(size.FlipY));
        }
    }
}