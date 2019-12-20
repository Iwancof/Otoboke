using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map {
    MapChip[][] MapData;
    //あえてジャグ配列

    public int Height { get; private set; }
    public int Width { get; private set; }

    public Map(int height,int width) {
        Height = height;
        Width = width;

        MapData = new MapChip[Width][];
        for(int x = 0;x < Width;x++) {
            MapData[x] = new MapChip[Height];
            for(int y = 0;y < Height;y++) {
                MapData[x][y] = MapChip.None;
            }
        }
        MapData[2][3] = MapChip.Wall;
    }
}

enum MapChip { 
    None = 0,
    Wall = 1,
    Respown = 2,
    //etc...
}

public static class MapCreater
{
    public static Map CreateTestMap() {
        return new Map(10, 10);
    }
}
