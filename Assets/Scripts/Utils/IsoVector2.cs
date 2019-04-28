using System;
using UnityEngine;

namespace Utils {
    public class IsoVector2 {
        public int x;
        public int y;
        public int z;
    
        public IsoVector2(int x = 0, int y = 0, int z = 0) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static IsoVector2 WorldToBoardPos(Vector3 worldPos) {
            return WorldToBoardPos((int) worldPos.x, (int) worldPos.y);
        }

        public static IsoVector2 WorldToBoardPos(int x, int y) {
            var screenX = x;
            var screenY = y;
            var halfTileWidth = TileConstants.TILE_WIDTH * 0.5f;
            var halfTileHeight = TileConstants.TILE_HEIGHT * 0.5f;
            return new IsoVector2(
                (int)Math.Floor((screenX / halfTileWidth + screenY / halfTileHeight) * 0.5f),
                (int)Math.Floor((screenX / halfTileWidth + screenY / halfTileHeight) * 0.5f)
                );
        }

        public Vector2 ToWorldPos() {
            return new Vector2((x - y) * (TileConstants.TILE_WIDTH * .5f) / TileConstants.TILE_WIDTH, (x + y) * (TileConstants.TILE_HEIGHT * .5f) / -TileConstants.TILE_WIDTH);
        }

        public IsoVector2 Add(int x, int y, int z = 0) {
            this.x += x;
            this.y += y;
            this.z += z;
            return this;
        }

        public IsoVector2 Add(Vector3 toAdd) {
            return Add((int)toAdd.x, (int)toAdd.y, (int)toAdd.z);
        }

        public IsoVector2 Add(IsoVector2 toAdd) {
            return Add(toAdd.x, toAdd.y, toAdd.z);
        }

        public int distance(IsoVector2 dest)
        {
            return Math.Abs(this.x - dest.x) + Math.Abs(this.y - dest.y);
        }

        public bool Matches(int currentNodeX, int currentNodeY)
        {
            return this.x == currentNodeX && this.y == currentNodeY;
        }

        public int distance(int checkX, int checkY)
        {
            return distance(new IsoVector2(checkX, checkY));
        }
    }
}