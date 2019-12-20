using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapShader : MonoBehaviour
{
    // Start is called before the first frame update
    Map map;

    void Start()
    {
        map = MapCreater.CreateTestMap();
    }

    // Update is called once per frame
    void Update()
    {
        for(int x = 0;x < map.Width;x++) {
            for(int y = 0;y < map.Height;y++) {

            }
        }
    }
}
