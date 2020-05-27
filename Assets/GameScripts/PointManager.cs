using System.Collections;
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
    public static int numofBite = 0;
    public static bool defeat = false;
    Image img;
    Slider slider;
    int prevBites;
    void Start()
    {
        duration = 10;
        defeat = false;
        numofBite = 0;
        baites = -1;
        slider = GameObject.Find("Canvas/Slider").GetComponent<Slider>();
        img = GameObject.Find("Canvas/Slider/FillArea/Fill").GetComponent<Image>();
        img.color = new Color(0, 255, 0);

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
            // ゲームオーバー処理
            defeat = true;
        }
        if (numofBite > 0) {
            slider.value = (float)(numofBite - baites) / numofBite;
        }
        if(prevBites != baites) {
            // r -> value 1～0.5 の間0から255まで上がる
            // g -> value 0.5～0 の間255から0まで下がる
            img.color = new Color(slider.value >= 0.5 ? (float)(1-(slider.value-0.5) * 2) : 1, slider.value < 0.5 ? (float)(slider.value * 2) : 1, 0);
        }
        prevBites = baites;

        
    }
}
