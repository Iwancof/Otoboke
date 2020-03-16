using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using SafePointer;
using System.Linq;

public class Player : MonoBehaviour {
    Vector2 buffer = new Vector2(0, 0); // 入力バッファ
    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向
    float rad = 0f;
    public float speed = 1.0f;
    public float delta = 0.001f;
    public static Vector3 respownPoint;
    public static int[][] map;
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

        InputBuffer();
        CheckStatus();
        Move();
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
        if(collision.gameObject.tag == "Pacman") {
            Defeat();
        }
    }

    public void RootSearch() {
        root = new int[map.Length][];
        for(int k = 0; k < map.Length; k++) {
            root[k] = new int[map[k].Length];
        }
        int i = 0, j = 0;
        foreach(var x in map) {
            j = 0;
            foreach(var y in x) {
                if(y == 2) goto end;
                j++;
            }
            i++;
        }
        end:

        // 幅優先探索
        var list = new (int x, int y)[4];
        list[0] = (0, 1);
        list[1] = (1, 0);
        list[2] = (0, -1);
        list[3] = (-1, 0);

        var q = new Queue<(int x,int y)>();
        q.Enqueue((i,j));

        while(q.Count > 0) {
            var tmp = q.Peek();
            foreach(var l in list) {
                var x = tmp.x + l.x;
                var y = tmp.y + l.y;
                if (x >= 0 && y >= 0 && x < root.Length && y < root[0].Length && root[x][y] == 0) {
                    if(!isWall(map[x][y])) {
                        root[x][y] = root[tmp.x][tmp.y] + 1;
                        q.Enqueue((x, y));
                    } else {
                        root[x][y] = 99;
                    }
                }
            }
            q.Dequeue();
        }

        for(int x = 0; x < root.Length; x++) {
            Array.Reverse(root[x]);
        }
        var arr = new int[root[0].Length][];
        for(int x = 0; x < arr.Length; x++) {
            arr[x] = new int[root.Length];
        }
        for(int x = 0; x < arr.Length; x++) {
            for(int y = 0; y < arr[0].Length; y++) {
                arr[x][y] = root[y][x];
            }
        }
        Array.Reverse(arr);
        root = arr;

        /*
        var str = "";
        for(int x = 0; x < root.Length; x++) {
            for(int y = 0; y < root[0].Length; y++) {
                if(root[x][y] >= 10) {
                    str += " " + root[x][y].ToString();
                } else {
                    str += "  " + root[x][y].ToString();
                }
            }
            str += '\n';
        }
        File.WriteAllText(Application.dataPath + "/mapdata.txt", str);
        */
    }

    bool isWall(int m) {
        if(m == 1 || m == 3 || m == 4) {
            return true;
        }
        return false;
    }

/*
    (int x, int y) GetCoordinate(Vector3 position) {

    }
    */

    public async void Defeat() { //やられた時の処理
        this.buffer = new Vector2(0, 0);

        this.gameObject.SetActive(false);

        await System.Threading.Tasks.Task.Run(() => {
            System.Threading.Thread.Sleep(2000); //復活までの時間
        });

        this.transform.position = respownPoint;
        this.gameObject.SetActive(true);
    }
}
