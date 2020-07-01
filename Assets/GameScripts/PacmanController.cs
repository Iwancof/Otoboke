using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PacmanController: MonoBehaviour
{
    [SerializeField]
    AudioClip bait0 = default, bait1 = default, ate = default;
    AudioSource source;
    Animator anim;
    public List<GameObject> Baits;
    bool whichSound = false; // true -> bait0 , false -> bait1
    public float powerTime = 10f;
    public Vector3 targetPos;

    Vector3 speed = new Vector3();

    public static Vector3[] teleportPoint = new Vector3[2];

    Color normal = new Color(1, 1, 0);
    Color power = new Color(0, 0, 1);
    SpriteRenderer spr;

    void Start()
    {
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
        anim.speed = 0;
        spr = GetComponent<SpriteRenderer>();
    }

    bool teleportFlag = false;
    void Update()
    {
        if(MapController.systemStatus != MapController.SystemStatus.GameStarted) {
            return;
        }
        if(PointManager.hasPower) {
            spr.color = power;
        } else {
            spr.color = normal;
        }
        if(targetPos != teleportPoint[0] && targetPos != teleportPoint[1]) {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref speed, powerTime);
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
                if(!source.isPlaying) {
                    PointManager.returningNest = false;
                    if (whichSound) {
                        source.clip = bait0;
                    } else {
                        source.clip = bait1;
                    }
                    source.Play();
                }
                Destroy(paced_object.Item2);
                break;
            case MapChip.PowerBait:
                if (PointManager.baites == -1) PointManager.baites = 0;
                PointManager.baites++;
                PointManager.startTime = Time.time;
                PointManager.hasPower = true;
                Destroy(paced_object.Item2);
                break;
        }
    }

    List<GameObject> returningPlayers = new List<GameObject>();
    void OnCollisionEnter2D(Collision2D collision) {
        GameObject player = collision.collider.transform.gameObject;
        if(PointManager.hasPower && collision.collider.tag == "Player" && !ExistInReturningPlayers(player)) {
            source.Stop();
            source.clip = ate;
            source.Play();
            PointManager.returningNest = true;
            returningPlayers.Add(player);
            StartCoroutine("IsReturningNest", player);
        }
    }

    bool ExistInReturningPlayers(GameObject player) {
        foreach(var e in returningPlayers) {
            if(e == player) return true;
        }
        return false;
    }
    
    // FIXME: すでにぶつかったプレイヤーは無視する
    IEnumerator IsReturningNest(GameObject player) {
        while(Vector3.Distance(player.transform.position, Player.respownPoint) > 1f) {
            yield return null;
        }
        Debug.LogWarning(player.transform.position);
        PointManager.returningNest = false;
        returningPlayers.RemoveAll(p => p == player);
    }

    public (MapChip, GameObject) FindBaitObjectByCoordinate(int px, int py) {
        Logger.Log(Logger.GameSystemPacTag, px + " : " + py + " in " + Map.DestroyList.Count());
        return Map.DestroyList[((int)px ,(int)py )];
    }
}