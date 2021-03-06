﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terrain;

namespace MainGame.UnitTests
{
    internal class TerrainGenerator
    {
        private string[] lines;

        public Coordinate Offset { get; }
        public Coordinate Size => new Coordinate(lines.Max(l => l.Length), lines.Length);

        public TerrainGenerator(string terrainMap)
        {
            lines = terrainMap.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            Offset = FindOrigin(lines);
        }

        public TerrainType Generator(Coordinate coord)
        {
            var vCoord = coord.V - Offset.V;
            // Anything above the defined terrain is considered free space
            if (vCoord > 0)
                return TerrainType.Free;

            // Anything below the defined terrain is considered bedrock (otherwise player spawns below the level)
            if (vCoord <= -lines.Length)
                return TerrainType.Bedrock;

            var line = lines[-vCoord];
            var uCoord = coord.U + Offset.U;
            if (uCoord < 0 || uCoord >= line.Length)
            {
                return TerrainType.Free;
            }

            return ParseTerrainChar(line[uCoord]);
        }

        private static Coordinate FindOrigin(string[] lines)
        {
            var xOffset = 0;
            var yOffset = 0;
            for (var v = 0; v < lines.Length; v++)
            {
                xOffset = lines[v].IndexOf('0');
                if (xOffset != -1)
                {
                    yOffset = v;
                    break;
                }
            }

            xOffset = xOffset == -1 ? 0 : xOffset;
            return new Coordinate(xOffset, yOffset);
        }

        private static TerrainType ParseTerrainChar(char c)
        {
            switch (char.ToUpper(c))
            {
                case '.':
                case '0':
                    return TerrainType.Free;
                case 'D':
                    return TerrainType.Dirt;
                case 'R':
                    return TerrainType.Rock;
                default:
                    throw new InvalidOperationException($"Attempt to generate terrain block from unknown symbol '{c}'.");
            }
        }
    }
}
