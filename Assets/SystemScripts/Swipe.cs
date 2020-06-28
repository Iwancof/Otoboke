﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// スワイプに関する操作
/// </summary>
public class Swipe {
    static bool touchFlag = true;               // 1フレームだけ有効
    static Vector3 touchedPos, liftedFingerPos; // タッチした座標・離した座標
    static float flickSensitivity = 1.0f;       // フリック感度
    static Vector2 dir = Vector2.zero;          // フリックした方向

    /// <summary>
    /// スワイプした方向を返します。初期値はVector2.zeroです。
    /// </summary>
    public static Vector2 SwipeDirection () {
        dir = Vector2.zero;
        if (Input.touches[0].phase == TouchPhase.Began && touchFlag) {
            touchedPos = Input.touches[0].position;
            touchFlag = false;
        } else if (Input.touches[0].phase == TouchPhase.Ended && !touchFlag) {
            liftedFingerPos = Input.touches[0].position;
            touchFlag = true;
            Vector3 dist = liftedFingerPos - touchedPos;
            if (Mathf.Abs (dist.x) >= flickSensitivity && Mathf.Abs (dist.y) < Mathf.Abs (dist.x)) {
                dir = new Vector2 (dist.x / Mathf.Abs (dist.x), 0);
            }
            if (Mathf.Abs (dist.y) >= flickSensitivity && Mathf.Abs (dist.y) >= Mathf.Abs (dist.x)) {
                dir = new Vector2 (0, dist.y / Mathf.Abs (dist.y));
            }
        }
        return dir;
    }
}
