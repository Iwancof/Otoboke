using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    int score = 0;
    public static int baites = -1;
    public static bool hasPower = false;
    public static float startTime;
    float duration, diff;
    public static int numofBite = 0;
    public static bool defeat = false;
    void Start()
    {
        duration = 10;
        defeat = false;
        numofBite = 0;
        baites = -1;

    }

    void Update()
    {
        if(hasPower) {
            diff = Time.timeSinceLevelLoad - startTime;
            if(diff >= duration) {
                hasPower = false;
            }
        }        
        if(numofBite <= baites) { // パックマンが餌を食べ尽くした
            //Debug.LogWarning("Defeat");
            // ゲームオーバー処理
            defeat = true;
        }
        
    }
}
