using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スワイプに関する操作
/// </summary>
public class Swipe {
    static bool touchFlag = true;               // 1フレームだけ有効
    static Vector2 touchedPos, liftedFingerPos; // タッチした座標・離した座標
    static float flickSensitivity = 100.0f;       // フリック感度
    static Vector2 dir = Vector2.zero;          // フリックした方向
    static Touch touchProcess = default;

    /// <summary>
    /// スワイプした方向を返します。初期値はVector2.zeroです。
    /// </summary>
    public static Vector2 SwipeDirection () {
        if(Input.touchCount > 0) {
            if (Input.touches[0].phase == TouchPhase.Began && touchFlag) {
                touchedPos = Input.touches[0].position;
                touchFlag = false;
            }
            if (Input.touches[0].phase == TouchPhase.Ended && !touchFlag) {
                liftedFingerPos = Input.touches[0].position;
                touchFlag = true;
                Vector2 dist = liftedFingerPos - touchedPos;
                if (Mathf.Abs (dist.x) >= flickSensitivity && Mathf.Abs (dist.y) < Mathf.Abs (dist.x)) {
                    dist.y = 0;
                    dir = dist.normalized;
                } else if (Mathf.Abs (dist.y) >= flickSensitivity && Mathf.Abs (dist.y) >= Mathf.Abs (dist.x)) {
                    dist.x = 0;
                    dir = dist.normalized;
                } else {
                    dir = Vector2.zero;
                }
                return dir;
            }
        }
        return Vector2.zero;
    }
}
