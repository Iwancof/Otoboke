using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController: MonoBehaviour
{
    Animator anim;
    Vector3 firstPos;
    Vector3 prevPos;
    Vector3 distance;

    float startTime;
    float time = 10f;
    float diff;

    bool firsttime = true;

    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向
    float rad = 0f;
    public float speed = 1.0f;
    public float delta = 0.001f;

    void Start()
    {
        anim = GetComponent<Animator>();
        prevPos = transform.position;

        rb = gameObject.GetComponent<Rigidbody2D>();
        rad = gameObject.GetComponent<CircleCollider2D>().radius;
        status.Add(Vector2.up.ToString(), false);
        status.Add(Vector2.right.ToString(), false);
        status.Add(Vector2.down.ToString(), false);
        status.Add(Vector2.left.ToString(), false);
        status.Add(Vector2.zero.ToString(), false);
    }

    void Update()
    {
        CheckStatus();

        if (rb.velocity == Vector2.zero) {
            anim.SetBool("move", false);
        } else {
            anim.SetBool("move", true);
        }
        prevPos = transform.position;
        //Move(transform.position, time);
    }

    void CheckStatus() {
        status[Vector2.up.ToString()]    = !Physics2D.Raycast(transform.position + transform.right * rad, transform.right, delta);
        status[Vector2.right.ToString()] = !Physics2D.Raycast(transform.position - transform.right * rad, -transform.up, rad + delta);
        status[Vector2.down.ToString()]  = !Physics2D.Raycast(transform.position - transform.right * rad, -transform.right, delta);
        status[Vector2.left.ToString()]  = !Physics2D.Raycast(transform.position - transform.right * rad, transform.up, rad + delta);

        Debug.DrawRay(transform.position + transform.right * rad, transform.right * rad);
        Debug.DrawRay(transform.position - transform.right * rad, -transform.up * rad, Color.blue);
        Debug.DrawRay(transform.position - transform.right * rad, -transform.right * rad, Color.red);
        Debug.DrawRay(transform.position - transform.right * rad, transform.up * rad, Color.yellow);
    }

    // posの座標までtime秒間かけて移動
    public void Move(Vector3 pos, float time) {
        if(firsttime) {
            startTime = Time.timeSinceLevelLoad;
            firstPos = transform.position;
            this.time = time;
            distance = pos - transform.position;
            firsttime = false;
        }
        if (time <= 0) {
            transform.position = pos;
            return;
        }
        diff = Time.timeSinceLevelLoad - startTime;
        var rate = diff / time;
        if(rate >= 1) {
            return;
        }

        transform.position = firstPos + distance * rate;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Collision!!!!");
        if(collision.tag == "Teleport") {
            this.transform.position = MapController.map.TeleportPoint[collision.gameObject];
        }
    }
}
