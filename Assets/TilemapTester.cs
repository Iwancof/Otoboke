using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapTester {
    public static void operateTilemap(string name) {
        var tilemap = GameObject.Find(name).GetComponent<Tilemap>();

        int size = 40;

        var range = new BoundsInt(-size / 2, -size / 2, 0, size, size, 1);
        var ret = tilemap.GetTilesBlock(range);

        Debug.Log(ret.Length);
        
        for(int i = 0;i < size;i++) {
            string st = "";
            for(int j = 0;j < size;j++) {
                st += (ret[i * size + j] != null) ? "○" : "●";
                if ((ret[i * size + j] != null)) Debug.Log(ret[i * size + j]    );
            }
            Debug.Log(i.ToString() + " : " + st);
        }
    }

}
