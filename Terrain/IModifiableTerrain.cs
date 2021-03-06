﻿using Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Terrain
{
    public interface IModifiableTerrain : ITerrainGenerator
    {
        void Destroy(Coordinate c, Plane p);
        void Place(Coordinate c, Plane p, TerrainType type);
        void SetActiveSector(int u, int v, int w);
    }
}
