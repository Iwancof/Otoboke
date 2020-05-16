using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;
using System;
using System.IO;


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
    private Sprite WallDirection(Sprite[] block,int direction, int x, int yIdx) {
        Sprite tmp;
        switch(direction) {
            case 0b1000:
            case 0b0010:
            case 0b1010:
                // 上下
                tmp = block[9];
                break;
            case 0b0100:
            case 0b0001:
            case 0b0101:
                // 左右
                tmp = block[10];
                break;
            case 0b1111:
                // 全方向
                if(yIdx-1 >= 0 && x-1 >= 0 && MapData[x-1][yIdx-1] != MapChip.Wall) {
                    // 左上が壁じゃない
                    tmp = block[8];
                } else if(yIdx-1 >= 0 && x+1 < Width && MapData[x+1][yIdx-1] != MapChip.Wall) {
                    // 右上が壁じゃない
                    tmp = block[6];
                } else if(yIdx+1 < Height && x-1 >= 0 && MapData[x-1][yIdx+1] != MapChip.Wall) {
                    // 左下が壁じゃない
                    tmp = block[2];
                } else if(yIdx+1 < Height && x+1 < Width && MapData[x+1][yIdx+1] != MapChip.Wall) {
                    // 右下が壁じゃない
                    tmp = block[0];
                } else {
                    tmp = null;
                }
                break;
            case 0b0110:
                // 右下
                tmp = block[0];
                break;
            case 0b0011:
                // 下左
                tmp = block[2];
                break;
            case 0b1100:
                // 上右
                tmp = block[6];
                break;
            case 0b1001:
                // 上左
                tmp = block[8];
                break;
            case 0b1011:
                // 上下左
                //tmp = block[5];
                tmp = block[9];
                break;
            case 0b1101:
                // 上右左
                //tmp = block[7];
                tmp = block[10];
                break;
            case 0b0111:
                // 右下左
                //tmp = block[1];
                tmp = block[10];
                break;
            case 0b1110:
                // 上右下
                //tmp = block[3];
                tmp = block[9];
                break;
            default:
                tmp = null;
                break;
        }
        return tmp;
    }

    public void OverwriteTile() {
        var wall_object = (GameObject)Resources.Load("WallBlock");
        var bait_object = (GameObject)Resources.Load("Bait");
        var powerBait_object = (GameObject)Resources.Load("PowerBait");
        Sprite[] block = Resources.LoadAll<Sprite>("stage");
        float size = 1.05f;
        Vector3 Point1 = new Vector3(0, 0, 0), Point2 = new Vector3(0, 0, 0);
        GameObject TeleportObj1 = null, TeleportObj2 = null;
        GameObject map = new GameObject("Map");
        bool isRespownPointSet = false;

        var str = "";
        for (int x = 0;x < Width;x++) {
            for (int y = 0; y < Height; y++) {
                var pos = new Vector3(x * size, y * size, 0);
                str += ((int)MapData[x][Height - y - 1]).ToString();
                // マップのタイプ
                var yIdx = Height - y - 1;
                switch (MapData[x][yIdx]) {
                    case MapChip.Wall:
                        // 壁のテクスチャ分岐
                        var direction = 0b0000;//(false, false, false, false); // up, right, down, left
                        if(yIdx-1 >= 0 && MapData[x][yIdx-1] == MapChip.Wall) direction |= 0b1000;
                        if(x+1 < Width && MapData[x+1][yIdx] == MapChip.Wall) direction |= 0b0100;
                        if(yIdx+1 < Height && MapData[x][yIdx+1] == MapChip.Wall) direction |= 0b0010;
                        if(x-1 >= 0 && MapData[x-1][yIdx] == MapChip.Wall) direction |= 0b0001;
                        //Vector3 inspos = new Vector3(x * size, y * size, 0);
                        GameObject obj;
                        obj = MonoBehaviour.Instantiate(wall_object, new Vector3(x * size, y * size, 0), Quaternion.identity);
                        obj.name = $"WallBlock_[{x},{y}]";
                        obj.transform.parent = map.transform;
                        Sprite tmp = WallDirection(block, direction, x, yIdx);
                        obj.GetComponent<SpriteRenderer>().sprite = tmp;
                        break;
                    case MapChip.Respown:
                        if (isRespownPointSet) throw MapCreateException.RespownPointDuplication;
                        Player.respownPoint = new Vector3(x * size, y * size, 0);
                        isRespownPointSet = true;
                        break;
                    case MapChip.TeleportPoint1:
                        Point1 = new Vector3(x * size, y * size, 0);
                        TeleportObj1 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Teleport"), Point1, Quaternion.identity);
                        TeleportObj1.name = "Teleport_1";
                        TeleportObj1.transform.parent = map.transform;
                        break;
                    case MapChip.TeleportPoint2:
                        Point2 = new Vector3(x * size, y * size, 0);
                        TeleportObj2 = MonoBehaviour.Instantiate((GameObject)Resources.Load("Teleport"), Point2, Quaternion.identity);
                        TeleportObj2.name = "Teleport_2";
                        TeleportObj2.transform.parent = map.transform;
                        break;
                    case MapChip.Bait:
                        obj = MonoBehaviour.Instantiate(bait_object, pos, Quaternion.identity);
                        obj.name = $"Bait_[{x},{y}]";
                        obj.transform.parent = map.transform;
                        break;
                    case MapChip.PowerBait:
                        obj = MonoBehaviour.Instantiate(powerBait_object, pos, Quaternion.identity);
                        obj.name = $"PowerBait_[{x},{y}]";
                        obj.transform.parent = map.transform;
                        break;
                }
            }
            str += '\n';
        }
        //File.WriteAllText(Application.dataPath + "/mapdata.txt", str);
        
        GameObject.FindWithTag("Player").GetComponent<Player>().RootSearch();

        GameObject[] objects = GameObject.FindGameObjectsWithTag("Bait");
        PointManager.numofBite = objects.Length;
        objects = GameObject.FindGameObjectsWithTag("PowerBait");
        PointManager.numofBite += objects.Length;

        //if (TeleportObj1 != null && TeleportObj2 != null) {
        TeleportPoint.Add(
            GameObject.Find("Teleport_1"),
            //TeleportObj1,
            Point2 + new Vector3(-size, 0, 0)
            );
        TeleportPoint.Add(
            GameObject.Find("Teleport_2"),
            //TeleportObj2,
            Point1 + new Vector3(size, 0, 0)
            );
        //}
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
    Bait = 5,
    PowerBait = 6,
    //etc...
}

public static class MapCreater
{
    public static Map CreateTestMap() {
        return new Map(10, 10);
    }
}

public class MapCreateException : Exception {
    public MapCreateException(string str) : base(str) {

    }
    public static MapCreateException RespownPointDuplication = 
        new MapCreateException("Respown point must not be more than two.");
}