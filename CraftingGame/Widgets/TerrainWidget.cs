using Contracts;
using System;
using System.Linq;
using Terrain;
using VectorMath;

namespace CraftingGame.Widgets
{
    internal class TerrainWidget
    {
        private readonly IRenderer renderer;
        private readonly CachedTerrainGenerator terrain;
        private readonly IFont debugFont;

        // Pre-rendered sectors
        private readonly QuadTree<PrerenderedTerrainSector> sectorSprites = new QuadTree<PrerenderedTerrainSector>();

        public TerrainWidget(IRenderer renderer, CachedTerrainGenerator terrain, IFont debugFont)
        {
            this.renderer = renderer;
            this.terrain = terrain;
            this.debugFont = debugFont;
        }

        public void Prerender(Grid grid, ViewportProjection view, Plane plane)
        {
            // TODO: Limit to pre-rendering one sector per cycle, this causes lag - could we pre-render incrementally?
            ForeachVisibleSector(grid, view, plane, voxel =>
            {

                var sectorSprite = sectorSprites[voxel];
                if(sectorSprite == null)
                {
                    var sector = terrain.GetSector(voxel);
                    sectorSprite = CreateTexture(grid, voxel, sector);
                    sectorSprites.Add(voxel, sectorSprite);
                }

                if (sectorSprite.Redraw)
                {
                    PrerenderToTexture(sectorSprite, () => PrerenderSector(grid, sectorSprite));
                    sectorSprite.Initialized = true;
                }
                else if (sectorSprite.TerrainModified)
                {
                    PrerenderToTexture(sectorSprite, () => RefreshPrerenderedSector(grid, sectorSprite));
                    sectorSprite.ClearModification();
                }
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
                    RenderTexture(view, bottomLeft, size, sectorSprite.Texture);
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

                // Render sector index
                RenderText(view, bottomLeft, Color.Red, $"({sectorSprite.Index.U},{sectorSprite.Index.V})");
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

        private void RenderTerrainBlock(ViewportProjection view, TerrainBlock block, Vector2 position, Vector2 size, bool overwrite)
        {
            var type = block.Type;
            var color = TerrainColor(type);
            switch (type)
            {
                case TerrainType.Dirt:
                case TerrainType.Bedrock:
                case TerrainType.Sea:
                    RenderRectangle(view, position, size, color);
                    break;
                case TerrainType.Rock:
                    RenderRectangle(view, position, size, color);
                    if(block.Ore != OreType.None)
                    {
                        // Render ore type overlay
                        RenderRectangle(view, position + size / 2, size / 2, Color.Red);
                    }
                    break;
                default:
                    if (overwrite)
                    {
                        RenderRectangle(view, position, size, color);
                    }
                    break;
            }
        }

        public static Color TerrainColor(TerrainType type)
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
                    return new Color(0, 0, 0, 1);
            }
        }

        private PrerenderedTerrainSector CreateTexture(Grid grid, Voxel index, TerrainSector sector)
        {
            int textureWidth = TerrainSector.SectorWidth * (int)grid.Size.X;
            int textureHeight = TerrainSector.SectorHeight * (int)grid.Size.Y;
            var textureView = ViewportProjection.ToTexture(new Vector2(textureWidth, textureHeight));
            var sprite = renderer.InitializeGpuTexture(textureWidth, textureHeight);
            return new PrerenderedTerrainSector(sector, textureView, sprite);
        }

        private void PrerenderToTexture(PrerenderedTerrainSector sector, Action renderAction)
        {
            renderer.RenderToGpuTexture(sector.Texture, renderAction);
        }

        private void PrerenderSector(Grid grid, PrerenderedTerrainSector sector)
        {
            for (var v = 0; v < TerrainSector.SectorHeight; v++)
            {
                for (var u = 0; u < TerrainSector.SectorHeight; u++)
                {
                    PrerenderBlock(grid, sector, u, v, false);
                }
            }
        }

        private void RefreshPrerenderedSector(Grid grid, PrerenderedTerrainSector sectorSprite)
        {
            foreach (var coord in sectorSprite.Modifications)
            {
                PrerenderBlock(grid, sectorSprite, coord, true);
            }
        }

        private void PrerenderBlock(Grid grid, PrerenderedTerrainSector sector, Coordinate coord, bool overwrite)
        {
            PrerenderBlock(grid, sector, coord.U, coord.V, overwrite);
        }

        private void PrerenderBlock(Grid grid, PrerenderedTerrainSector sector, int u, int v, bool overwrite)
        {
            sector.TerrainSector.Generate(u, v);
            var block = sector.TerrainSector[u, v];
            var position = new Vector2(u, v) * grid.Size;
            RenderTerrainBlock(sector.Projection, block, position, grid.Size, overwrite);
        }

        private void RenderVector(ViewportProjection view, Vector2 point, Vector2 vector, Color color, float thickness)
        {
            renderer.RenderVector(view.MapToViewport(point), view.MapSizeToViewport(vector), color, thickness);
        }

        private void RenderText(ViewportProjection view, Vector2 point, Color color, string text)
        {
            renderer.RenderText(debugFont, view.MapToViewport(point), text, color);
        }

        private void RenderRectangle(ViewportProjection view, Vector2 point, Vector2 size, Color color)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            renderer.RenderRectangle(view.MapToViewport(point), view.MapSizeToViewport(size.FlipY), color);
        }

        private void RenderTexture(ViewportProjection view, Vector2 point, Vector2 size, IGpuTexture texture)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            renderer.RenderOpagueSprite(texture, view.MapToViewport(point), view.MapSizeToViewport(size.FlipY));
        }
    }
}