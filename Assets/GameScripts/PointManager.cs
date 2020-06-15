﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointManager : MonoBehaviour
{
    int score = 0;
    public static int baites = -1;
    public static bool hasPower = false;
    public static float startTime;
    float duration, diff;
    public static int numOfBite = 0;
    public static bool defeat = false;
    Image img;
    Slider slider;
    Text timelimit;
    int prevBites;

    public static float gameStartTime;
    float timeLimit = 30;
    FirstTimeClass timerFt = new FirstTimeClass();
    void Start()
    {
        duration = 10;
        defeat = false;
        hasPower = false;
        numOfBite = 0;
        baites = -1;
        slider = GameObject.Find("Canvas/Slider").GetComponent<Slider>();
        img = GameObject.Find("Canvas/Slider/FillArea/Fill").GetComponent<Image>();
        img.color = new Color(0, 255, 0);
        timelimit = GameObject.Find("Canvas/TimeLimit").GetComponent<Text>();
    }

    void Update()
    {
        if (MapController.systemStatus != MapController.SystemStatus.GameStarted) {
            timelimit.text = "Time limit:" + (timeLimit).ToString("F1");
            return;
        }
        if(timerFt) {
            gameStartTime = Time.time;
        }
        timelimit.text = "Time limit:" + (timeLimit - Time.time + gameStartTime).ToString("F1");
        if(hasPower) {
            diff = Time.time - startTime;
            if(diff >= duration) {
                hasPower = false;
            }
        }        
        if(numOfBite <= baites || Time.time - gameStartTime >= timeLimit) { // パックマンが餌を食べ尽くした
            // ゲームオーバー処理
            defeat = true;
        }
        if (numOfBite > 0) {
            slider.value = (float)(numOfBite - baites) / numOfBite;
        }
        if(prevBites != baites) {
            // r -> value 1～0.5 の間0から255まで上がる
            // g -> value 0.5～0 の間255から0まで下がる
            img.color = new Color(slider.value >= 0.5 ? (float)(1-(slider.value-0.5) * 2) : 1, slider.value < 0.5 ? (float)(slider.value * 2) : 1, 0);
            timelimit.color = new Color(1, img.color.g, 0);
        }
        prevBites = baites;
    }
}
