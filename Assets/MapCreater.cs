using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class Map {
    public MapChip[][] MapData;
    //あえてジャグ配列

    public int Height { get; private set; }
    public int Width { get; private set; }
    public Dictionary<GameObject,Vector3> TeleportPoint = new Dictionary<GameObject, Vector3>();

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
        /*
        int[,] tmp_map = {
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0, },
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0, },
            {1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,1,0,0, },
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0, },
            {1,0,1,1,0,1,0,1,1,1,0,1,0,1,1,0,1,0,0, },
            {1,0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,1,0,0, },
            {1,1,1,1,0,1,1,0,1,0,1,1,0,1,1,1,1,0,0, },
            {1,1,1,1,0,1,0,0,1,0,0,1,0,1,1,1,1,0,0, },

            {1,1,1,1,0,1,0,0,0,0,0,1,0,1,1,1,1,0,0, },
            {0,0,0,0,0,1,0,0,0,0,0,1,0,0,0,0,0,0,0, },
            {1,1,1,1,0,1,0,0,0,0,0,1,0,1,1,1,1,0,0, },

            {1,1,1,1,0,1,0,0,1,0,0,1,0,1,1,1,1,0,0, },
            {1,1,1,1,0,1,1,0,1,0,1,1,0,1,1,1,1,0,0, },
            {1,0,0,0,0,1,0,0,1,0,0,1,0,0,0,0,1,0,0, },
            {1,0,1,1,0,1,0,1,1,1,0,1,0,1,1,0,1,0,0, },
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0, },
            {1,0,1,1,0,1,1,0,1,0,1,1,0,1,1,0,1,0,0, },
            {1,0,0,0,0,0,0,0,1,0,0,0,0,0,0,0,1,0,0, },
            {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,0, },
        };
        for(int i = 0;i < 19;i++) {
            for(int j = 0;j < 19;j++) {
                MapData[i][j] = (MapChip)tmp_map[j, i];
            }
        }
        */
    }

    public void ShowInConsole() {
        for(int i = 0;i < Height;i++) {
            string tmp = i.ToString() + " : ";
            for(int j = 0;j < Width;j++) {
                tmp += ConvertToChar(MapData[j][i]);
            }
            Debug.Log(tmp);
        }
    }

    private static char ConvertToChar(MapChip mc) {
        char ret = ' ';
        switch(mc) {
            case MapChip.None:
                ret = ' ';break;
            case MapChip.Wall:
                ret = '#';break;
            case MapChip.Respown:
                ret = 'X';break;
        }

        return ret;
    }

    public void OverwriteTile() {
        var wall_object = (GameObject)Resources.Load("WallBlock");
        float size = 1.05f;
        Vector3 Point1 = new Vector3(0, 0, 0), Point2 = new Vector3(0, 0, 0);
        for(int x = 0;x < Width;x++) {
            for (int y = 0; y < Height; y++) {
                switch (MapData[x][Height - y - 1]) {
                    case MapChip.Wall:
                        MonoBehaviour.Instantiate(wall_object, new Vector3(x * size, y * size, 0), Quaternion.identity);
                        break;
                    case MapChip.TeleportPoint1:
                        Point1 = new Vector3(x * size, y * size, 0);
                        var Teleport1 = (GameObject)Resources.Load("Teleport");
                        Teleport1.name = "Teleport_1";
                        MonoBehaviour.Instantiate(Teleport1, Point1, Quaternion.identity);
                        break;
                    case MapChip.TeleportPoint2:
                        Point2 = new Vector3(x * size, y * size, 0);
                        var Teleport2 = (GameObject)Resources.Load("Teleport");
                        Teleport2.name = "Teleport_2";
                        MonoBehaviour.Instantiate(Teleport2, Point2, Quaternion.identity);
                        break;
                }
            }
        }
        TeleportPoint.Add(
            GameObject.Find("Teleport_1(Clone)"),
            Point2 + new Vector3(-size, 0, 0)
            );
        TeleportPoint.Add(
            GameObject.Find("Teleport_2(Clone)"),
            Point1 + new Vector3(size, 0, 0)
            );
    }

    public static Map CreateByString(string map_data) {
        var row = new List<string>(map_data.Split(','));
        int hei = row.Count;
        int wei = row[0].Length;

        Map ret = new Map(hei, wei);
        row
            .Select((e, i) => (e, i))
            .ToList()
            .ForEach(a => {
                a.e
                    .ToCharArray()
                    .ToList()
                    .Select((f, j) => (f, j))
                    .ToList()
                    .ForEach(b => {
                        ret.MapData[b.j][a.i] = (MapChip)int.Parse(b.f.ToString());
                    }
                            );
            });
        return ret;
    }
}

public enum MapChip { 
    None = 0,
    Wall = 1,
    Respown = 2,
    TeleportPoint1 = 3,
    TeleportPoint2 = 4,
    //etc...
}

public static class MapCreater
{
    public static Map CreateTestMap() {
        return new Map(10, 10);
    }
}
