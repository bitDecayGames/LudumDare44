using System;

namespace Utils {
    public static class TileConstants {
        public static int TILE_WIDTH = 16;
        public static int TILE_HEIGHT = 8;
        public static int WIDTH_HALF = TILE_WIDTH / 2;
        public static int HEIGHT_HALF = TILE_HEIGHT / 2;

        // Used to map untiy coordinates to board coordinates
        public static int TILE_SIZE = 8;

        public static double cos30 = Math.Cos(30);
        public static double cos60 = Math.Cos(60);
    }
}