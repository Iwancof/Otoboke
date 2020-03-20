﻿using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SafePointer;
using System.Linq;

public class Player : MonoBehaviour {
    Vector3 firstPos;
    Vector3 distance;
    Vector3 posBuff;
    Vector2 buffer = new Vector2(0, 0); // 入力バッファ
    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向
    float rad = 0f;
    bool firsttime = true;
    bool isDead = false;
    float startTime;
    float time = 0.05f;
    public float speed = 1.0f;
    public float delta = 0.001f;
    public static Vector3 respownPoint;
    int[][] root;

    void Start() {
        rad = gameObject.GetComponent<CircleCollider2D>().radius;
        rb = gameObject.GetComponent<Rigidbody2D>();
        status.Add(Vector2.up.ToString(), false);
        status.Add(Vector2.right.ToString(), false);
        status.Add(Vector2.down.ToString(), false);
        status.Add(Vector2.left.ToString(), false);
        status.Add(Vector2.zero.ToString(), false);

        //new Map(19, 19).OverwriteTile();
    }

    void Update() {
        if(!isDead) {
            InputBuffer();
            CheckStatus();
            Move();
        }
    }


    void CheckStatus() {
        status[Vector2.up.ToString()]    = !Physics2D.Raycast(transform.position + transform.up * rad, transform.up, delta);
        status[Vector2.right.ToString()] = !Physics2D.Raycast(transform.position - transform.up * rad, transform.right, rad + delta);
        status[Vector2.down.ToString()]  = !Physics2D.Raycast(transform.position - transform.up * rad, -transform.up, delta);
        status[Vector2.left.ToString()]  = !Physics2D.Raycast(transform.position - transform.up * rad, -transform.right, rad + delta);

        Debug.DrawRay(transform.position + transform.up * rad, transform.up * rad);
        Debug.DrawRay(transform.position - transform.up * rad, transform.right * rad, Color.blue);
        Debug.DrawRay(transform.position - transform.up * rad, -transform.up * rad, Color.red);
        Debug.DrawRay(transform.position - transform.up * rad, -transform.right * rad, Color.yellow);
    }

    void InputBuffer() {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) >= 0.1f && y == 0) {
            buffer = new Vector2(x / Mathf.Abs(x), 0);
        }
        if (Mathf.Abs(y) >= 0.1f && x == 0) {
            buffer = new Vector2(0, y / Mathf.Abs(y));
        }
    }

    // 渡したベクトルがVector2.upとなす角を返す
    float VecAngle(Vector2 vec) {
        float an = Mathf.Atan2(vec.x, vec.y);
        return an * (360 / Mathf.PI / 2);
    }

    void Move() {
        // バッファをプレイヤーから見た方向に変換してからstatusに突っ込む(比較する)
        if (status[((Vector2)(Quaternion.Euler(0, 0, VecAngle(transform.up)) * buffer)).ToString()]) {
            transform.rotation = Quaternion.Euler(0, 0, -VecAngle(buffer));
            if (buffer != Vector2.zero) {
                rb.velocity = transform.up * speed;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Collision!!!!");
        if(collision.tag == "Teleport") {
            this.transform.position = MapController.map.TeleportPoint[collision.gameObject];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.tag == "Pacman" && !isDead) {
            isDead = true;
            Defeat();
        }
    }


    bool isWall(MapChip m) =>
        m == MapChip.Wall || m == MapChip.TeleportPoint1 || m == MapChip.TeleportPoint2;

    (int x, int y) GetCoordinate(Vector3 position) {
        var size = 1.05f;
        return ((int)((position.x - size / 2 + 1) / size),
                (int)((position.y - size / 2 + 1) / size));
    }

    public void Defeat() { //やられた時の処理
        buffer = new Vector2(0, 0);
        StartCoroutine(BackToNest());
    }

    IEnumerator MoveCoroutine(Vector3 pos, float time) {
        firsttime = true;

        var posnow = transform.position;

        startTime = Time.timeSinceLevelLoad;
        firstPos = posnow;
        this.time = time;
        distance = pos - posnow;
        firsttime = false;

        while(Vector3.Distance(transform.position, pos) > 0.05f) {
            if (time <= 0) {
                transform.position = pos;
            }
            var diff = Time.timeSinceLevelLoad - startTime;
            var rate = diff / time;
            if (rate >= 1) {
                firsttime = true;
                pos = transform.position;
                break;
            }

            transform.position = (firstPos + distance * rate);
            yield return null;
        }
        yield break;
    }

    IEnumerator BackToNest() {
        (int x, int y) pos = GetCoordinate(transform.position);
        var direction = new (int x, int y)[4];
        direction[0] = (0, 1);
        direction[1] = (1, 0);
        direction[2] = (0, -1);
        direction[3] = (-1, 0);
        var count = 0;

        while(root[pos.x][pos.y] != 1 && count++ < 100) {
            pos = GetCoordinate(transform.position);
            foreach(var dir in direction) {
                (int x, int y) target = (pos.x + dir.x, pos.y + dir.y);
                if(target.x >= 0 && target.y >= 0 && target.x < root.Length && target.y < root[0].Length) {
                    if(root[target.x][target.y] < root[pos.x][pos.y]) {
                        yield return StartCoroutine(MoveCoroutine(new Vector3(target.x * 1.05f, target.y * 1.05f, 0), time));
                        break;
                    }
                }
            }

            pos = GetCoordinate(transform.position);
        }

        isDead = false;
        yield break;
    }

    public void RootSearch() {
        var map = MapController.map;
        root = new int[map.Width][];
        for(int k = 0; k < map.Width; k++) {
            //root[k] = new int[map.Height];
            root[k] = Enumerable.Repeat(1000, map.Height).ToArray();
        }
        (int x, int y) respown = (0,0);
        foreach(var (x,i) in map.MapData.Select((x,i) => (x,i))) {
            foreach(var (y,j) in x.Select((y,j) => (y,j))) {
                if(y == MapChip.Respown) {
                    respown = (i, j);
                }
            }
        }

        // 幅優先探索
        var direction = new (int x, int y)[4];
        direction[0] = (0, 1);
        direction[1] = (1, 0);
        direction[2] = (0, -1);
        direction[3] = (-1, 0);

        var q = new Queue<(int x,int y)>();
        q.Enqueue(respown);
        root[respown.x][respown.y] = 0;

        while(q.Count > 0) {
            var tmp = q.Dequeue();
            foreach(var l in direction) {
                var x = tmp.x + l.x;
                var y = tmp.y + l.y;
                if (x >= 0 && y >= 0 && x < root.Length && y < root[0].Length) {
                    if(!isWall(map.MapData[x][y])) {
                        if(root[x][y] == 1000) {
                            q.Enqueue((x, y));
                        }
                        root[x][y] = Math.Min(root[tmp.x][tmp.y] + 1, root[x][y]);
                    }
                }
            }
        }
        for(int x = 0; x < root.Length; x++) {
            Array.Reverse(root[x]);
        }

        var str = "";
        for (int y = 0; y < map.Height; y++) {
            for (int x = 0; x < map.Width; x++) {
                if(root[x][y] >= 10) {
                    str += " " + root[x][y].ToString();
                } else {
                    str += "  " + root[x][y].ToString();
                }
            }
            str += '\n';
        }

        File.WriteAllText(Application.dataPath + "/mapdata.txt", str);
    }
}
