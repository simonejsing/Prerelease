using System;
using Terrain;

namespace CraftingGame
{
    public class TerrainModificationEvent : EventArgs
    {
        public Voxel Index;
        public Coordinate LocalCoord;
        public Coordinate GlobalCoord;

        public TerrainModificationEvent(Voxel index, Coordinate localCoord, Coordinate globalCoord)
        {
            Index = index;
            LocalCoord = localCoord;
            GlobalCoord = globalCoord;
        }
    }

    //type TerrainModificationEventHandler = EventHandler<TerrainModificationEvent>;
    //public delegate void TerrainModificationEventHandler(object sender, TerrainModificationEvent args);
}