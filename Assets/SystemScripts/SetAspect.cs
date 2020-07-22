using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAspect : MonoBehaviour {
    float displayHeight, displayWidth;
    float displayAspect;
    float gameHeight = 600, gameWidth = 800;
    float gameAspect;
    Camera cam = null;
    void Awake () {
        cam = GetComponent<Camera>();
        displayHeight = Screen.height;
        displayWidth = Screen.width;
        displayAspect = displayHeight / displayWidth;
        gameAspect = gameHeight / gameWidth;
        float aspect = displayAspect / gameAspect;

        cam.rect = new Rect((1 - aspect) / 2, 0, aspect, 1);
    }
}