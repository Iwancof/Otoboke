using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    int score = 0;
    public static int baites = 0;
    public static bool hasPower = false;
    public static float startTime;
    float duration, diff;
    void Start()
    {
        duration = 10;
    }

    void Update()
    {
        if(hasPower) {
            diff = Time.timeSinceLevelLoad - startTime;
            if(diff >= duration) {
                hasPower = false;
            }
        }        
    }
}
