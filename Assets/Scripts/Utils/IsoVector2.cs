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

        public static IsoVector2 ScreenToWorldPos(Vector3 screenPos) {
            return ScreenToWorldPos((int) screenPos.x, (int) screenPos.y);
        }

        public static IsoVector2 ScreenToWorldPos(int x, int y) {
            var screenX = x; // TODO: MW there probably needs to be some kind of camera offset being added here
            var screenY = y;
            var halfTileWidth = TileConstants.TILE_WIDTH * 0.5f;
            var halfTileHeight = TileConstants.TILE_HEIGHT * 0.5f;
            return new IsoVector2(
                (int)Math.Floor((screenX / halfTileWidth + screenY / halfTileHeight) * 0.5f),
                (int)Math.Floor((screenX / halfTileWidth + screenY / halfTileHeight) * 0.5f)
                );
        }

        public Vector2 ToScreenPos() {
            return new Vector2((x - y) * (TileConstants.TILE_WIDTH * 0.5f), (x + y) * (TileConstants.TILE_HEIGHT * 0.5f));
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
    }
}