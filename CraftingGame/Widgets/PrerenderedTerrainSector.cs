using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Terrain;

namespace CraftingGame.Widgets
{
    internal class PrerenderedTerrainSector
    {
        private TerrainSector sector;
        private List<Coordinate> modifiedCoordinates = new List<Coordinate>();

        public ViewportProjection Projection { get; }
        public IGpuTexture Texture { get; }
        public bool TerrainModified => modifiedCoordinates.Any();
        public Voxel Index => sector.Index;
        public TerrainSector TerrainSector => sector;
        public IEnumerable<Coordinate> Modifications => modifiedCoordinates;
        public bool Initialized { private get; set; } = false;
        public bool Redraw => !Initialized || Texture.ContentLost;

        public PrerenderedTerrainSector(TerrainSector sector, ViewportProjection projection, IGpuTexture texture)
        {
            this.sector = sector;
            this.Projection = projection;
            this.Texture = texture;
            sector.TerrainModification += TerrainModifiedEvent;
        }

        private void TerrainModifiedEvent(object sender, TerrainModificationEvent args)
        {
            modifiedCoordinates.Add(args.LocalCoord);
        }

        internal void ClearModification()
        {
            modifiedCoordinates.Clear();
        }
    }
}