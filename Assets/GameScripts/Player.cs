using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    Vector2 buffer = new Vector2(0, 0); // 入力バッファ
    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向
    float rad = 0f;
    public float speed = 1.0f;
    public float delta = 0.001f;

    void Start() {
        rad = gameObject.GetComponent<CircleCollider2D>().radius;
        rb = gameObject.GetComponent<Rigidbody2D>();
        status.Add(Vector2.up.ToString(), false);
        status.Add(Vector2.right.ToString(), false);
        status.Add(Vector2.down.ToString(), false);
        status.Add(Vector2.left.ToString(), false);
        status.Add(Vector2.zero.ToString(), false);
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
        Debug.Log("aaa");
        if (collision.tag == "Right") {
            // ワープ
            this.transform.position = new Vector2(-13.4f, 0.5f);
        } else if (collision.tag == "Left") {
            this.transform.position = new Vector2(13.4f, 0.5f);
        }
    }
}
