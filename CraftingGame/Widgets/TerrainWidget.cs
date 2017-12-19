using Contracts;
using Terrain;
using VectorMath;

namespace CraftingGame.Widgets
{
    internal class TerrainWidget
    {
        private readonly IRenderer renderer;
        private readonly ITerrainGenerator terrain;

        public TerrainWidget(IRenderer renderer, ITerrainGenerator terrain)
        {
            this.renderer = renderer;
            this.terrain = terrain;
        }

        public void Render(Grid grid, ViewportProjection view, Plane plane)
        {
            var activeView = view.Projection;
            var startCoord = grid.PointToGridCoordinate(activeView.BottomLeft);
            var numberCells = grid.PointToGridCoordinate(activeView.Size);

            for (var v = 0; v <= numberCells.V; v++)
            {
                for (var u = 0; u <= numberCells.U; u++)
                {
                    var coord = startCoord + new Coordinate(u, v);
                    var block = terrain[coord, plane];
                    var position = grid.GridCoordinateToPoint(coord);
                    switch (block.Type)
                    {
                        case TerrainType.Dirt:
                            RenderRectangle(view, position, grid.Size, Color.Yellow);
                            break;
                        case TerrainType.Rock:
                            RenderRectangle(view, position, grid.Size, Color.Gray);
                            break;
                        case TerrainType.Bedrock:
                            RenderRectangle(view, position, grid.Size, Color.DarkGray);
                            break;
                        case TerrainType.Sea:
                            RenderRectangle(view, position, grid.Size, Color.Blue);
                            break;
                        case TerrainType.Free:
                            break;
                    }
                }
            }
        }

        private void RenderRectangle(ViewportProjection view, Vector2 point, Vector2 size, Color color)
        {
            // The renderer expects to get the top left screen pixel and a positive size (after scale)
            // since we have flipped the y axis, we must correct by giving a negative height size
            // and add the height to the origin.
            point = new Vector2(point.X, point.Y + size.Y);
            size = new Vector2(size.X, -size.Y);
            renderer.RenderRectangle(view.MapToViewport(point), view.MapSizeToViewport(size), color);
        }
    }
}