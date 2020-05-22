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
    static public Dictionary<(int x, int y), (MapChip, GameObject)> DestroyList = new Dictionary<(int x, int y), (MapChip, GameObject)>();



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
        tmp = new MySwitch<int, Sprite>(direction)
             // 上下
            .Case(0b1000, block[9])
            .Case(0b0010)
            .Case(0b1010)
            .Case(0b1011)
            .Case(0b1110)
            // 左右
            .Case(0b0100, block[10])
            .Case(0b0001)
            .Case(0b0101)
            .Case(0b1101)
            .Case(0b0111)
            // 全方向
            .Case(0b1111, null)
            // 右下
            .Case(0b0110, block[0])
            // 下左
            .Case(0b0011, block[2])
            // 上右
            .Case(0b1100, block[6])
            // 上左
            .Case(0b1001, block[8])
            .Default(null);

        // 全方向
        if (direction == 0b1111) {
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
        for (int x = 0; x < Width; x++) {
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
                        /*
                        if(Vector3.Distance(pos, new Vector3(12.6f, 11.55f, 0)) < 0.1f) {
                            Debug.Log($"bait at {x}, {y}");
                        }
                        */
                        DestroyList.Add((x, y - 1), (MapChip.Bait, obj));
                        break;
                    case MapChip.PowerBait:
                        obj = MonoBehaviour.Instantiate(powerBait_object, pos, Quaternion.identity);
                        obj.name = $"PowerBait_[{x},{y}]";
                        obj.transform.parent = map.transform;
                        DestroyList.Add((x, y - 1), (MapChip.PowerBait, obj));
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


/// <summary>
/// switch式が使えない環境で同じような機能を提供します。最後に必ずDefaultを挿入する必要があります。
/// </summary>
/// <typeparam name="U">比較対象の型</typeparam>
/// <typeparam name="T">式の返り値の型</typeparam>
class MySwitch<U, T> {
    // T:switchで返す値の型
    // U:switchで比較する型
    T res;
    T prevNum;
    dynamic cmp = default(U);
    private bool match = false;

    public MySwitch(U obj) {
        this.cmp = obj;
    }

    /// <summary>
    /// case句
    /// </summary>
    /// <param name="targ">比較のラベル</param>
    /// <param name="num">マッチした場合の値</param>
    /// <returns></returns>
    public MySwitch<U, T> Case(U targ, T num) {
        if(targ.Equals(cmp)) {
            match = true;
            res = num;
        }
        prevNum = num;
        return this;
    }

    /// <summary>
    /// case句のフォールスルー
    /// 当てはまる場合の返り値は連続する条件のはじめに書く必要があります。
    /// </summary>
    /// <param name="targ">比較のラベル</param>
    public MySwitch<U, T> Case(U targ) {
        if(targ.Equals(cmp)) {
            match = true;
            res = prevNum;
        }
        return this;
    }
    /// <summary>
    /// default句
    /// </summary>
    /// <param name="num">デフォルトの値</param>
    /// <returns></returns>
    public T Default(T num) {
        if (match) {
            return res;
        } else {
            return num;
        }
    }
}
