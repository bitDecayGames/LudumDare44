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
            return WorldToBoardPos(worldPos.x, worldPos.y);
        }

        public static IsoVector2 WorldToBoardPos(float x, float y) {
            // double isoX = (x / TileConstants.cos30) - (y / TileConstants.cos60);
            // double isoY = (-x / TileConstants.cos30) + (y / TileConstants.cos60);
            // return new IsoVector2((int) isoX, (int) isoY);
            double isoX = (x / TileConstants.WIDTH_HALF + y / TileConstants.HEIGHT_HALF) / 2;
            double isoY = (y / TileConstants.HEIGHT_HALF - (x / TileConstants.WIDTH_HALF)) / 2;
            return new IsoVector2((int) isoX, (int) isoY);
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

        public bool Equals(int currentNodeX, int currentNodeY)
        {
            return this.x == currentNodeX && this.y == currentNodeY;
        }

        public bool Equals(IsoVector2 isoVector2)
        {
            return this.Equals(isoVector2.x, isoVector2.y);
        }

        public int distance(int checkX, int checkY)
        {
            return distance(new IsoVector2(checkX, checkY));
        }

        public static IsoVector2 GridCoordsToBoard(float x, float y)
        {
            return new IsoVector2((int) (x / 8) - 2, (int) (y / 8) - 2);
        }
    }
}