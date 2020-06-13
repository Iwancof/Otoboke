﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PacmanController: MonoBehaviour
{
    [SerializeField]
    AudioClip bait0, bait1;
    AudioSource source;
    Animator anim;
    public List<GameObject> Baits;
    bool whichSound = false; // true -> bait0 , false -> bait1
    public float time = 10f;
    public Vector3 targetPos;

    Vector3 speed = new Vector3();

    public static Vector3[] teleportPoint = new Vector3[2];

    Color normal = new Color(1, 1, 0);
    Color power = new Color(0, 0, 1);
    Color col;

    void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        anim.speed = 0;
        col = GetComponent<SpriteRenderer>().color;
    }

    bool teleportFlag = false;
    void Update()
    {
        if(MapController.systemStatus != MapController.SystemStatus.GameStarted) {
            return;
        }
        if(PointManager.hasPower) {
            col = power;
        } else {
            col = normal;
        }
        if(targetPos != teleportPoint[0] && targetPos != teleportPoint[1]) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, time);
            teleportFlag = true;
        } else if(teleportFlag) {
            transform.position = new MySwitch<Vector3, Vector3>(targetPos)
                                 .Case(teleportPoint[0], teleportPoint[1] - new Vector3(1f, 0, 0) * Mathf.Sign(teleportPoint[1].x))
                                 .Case(teleportPoint[1], teleportPoint[0] - new Vector3(1f, 0, 0) * Mathf.Sign(teleportPoint[0].x))
                                 .Default(transform.position);
            teleportFlag = false;
        }

        SetAnimation();
    }

    void SetAnimation() {
        if (Mathf.Abs(speed.x) > Mathf.Abs(speed.y)) {
            if (speed.x > 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", true);
                anim.SetBool ("left", false);
                anim.SetBool ("up", false);
                anim.SetBool ("down", false);
            } else if (speed.x < 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", true);
                anim.SetBool ("up", false);
                anim.SetBool ("down", false);
            } 
        } else if (Mathf.Abs(speed.y) > Mathf.Abs(speed.x)) {
            if (speed.y > 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", false);
                anim.SetBool ("up", true);
                anim.SetBool ("down", false);
            } else if (speed.y < 0) {
                anim.speed = 1.0f;
                anim.SetBool ("right", false);
                anim.SetBool ("left", false);
                anim.SetBool ("up", false);
                anim.SetBool ("down", true);
            }
        } else {
            anim.speed = 0;
        }
    }

    public void PacBaitAt(int x, int y) {
        var paced_object = FindBaitObjectByCoordinate(x, y);
        switch(paced_object.Item1) {
            case MapChip.Bait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                whichSound = !whichSound;
                if (whichSound) {
                    source.clip = bait0;
                } else {
                    source.clip = bait1;
                }
                source.Play();
                Destroy(paced_object.Item2);
                break;
            case MapChip.PowerBait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                PointManager.startTime = Time.timeSinceLevelLoad;
                PointManager.hasPower = true;
                Destroy(paced_object.Item2);
                break;
        }
    }

    public (MapChip, GameObject) FindBaitObjectByCoordinate(int px, int py) {
        Logger.Log(Logger.GameSystemPacTag, px + " : " + py + " in " + Map.DestroyList.Count());
        return Map.DestroyList[((int)px ,(int)py )];
    }
}
