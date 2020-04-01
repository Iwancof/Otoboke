using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController: MonoBehaviour
{
    Animator anim;
    Vector3 firstPos;
    Vector3 nowPos;
    Vector3 prevPos;
    Vector3 distance;
    public Vector3 targetPos;

    float startTime;
    public float time = 10f;
    float diff;

    bool firsttime = true;

    Rigidbody2D rb;
    Dictionary<string, bool> status = new Dictionary<string, bool>();   // Playerから見た方向

    [SerializeField, HeaderAttribute ("うまく曲がれないときに大きくする"), Range (0, 10)]
    public float gap = 3.0f;
    float rad = 0f;
    public float speed = 1.0f;
    public float delta = 0.001f;

    [SerializeField]
    AudioClip bait0, bait1;
    AudioSource source;
    bool whichSound = false; // true -> bait0 , false -> bait1

    void Start()
    {
        anim = GetComponent<Animator>();
        prevPos = transform.position;
        nowPos = transform.position;

        rb = gameObject.GetComponent<Rigidbody2D>();
        rad = gameObject.GetComponent<CircleCollider2D>().radius;

        source = GetComponent<AudioSource>();
        status.Add(Vector2.up.ToString(), false);
        status.Add(Vector2.right.ToString(), false);
        status.Add(Vector2.down.ToString(), false);
        status.Add(Vector2.left.ToString(), false);
        status.Add(Vector2.zero.ToString(), false);
    }

    void Update()
    {
        //CheckStatus();
        Move(targetPos, time);
        nowPos = transform.position;
        if (prevPos.x < nowPos.x) {
            anim.speed = 1.0f;
            anim.SetBool ("right", true);
            anim.SetBool ("left", false);
            anim.SetBool ("up", false);
            anim.SetBool ("down", false);
        } else if (prevPos.x > nowPos.x) {
            anim.speed = 1.0f;
            anim.SetBool ("right", false);
            anim.SetBool ("left", true);
            anim.SetBool ("up", false);
            anim.SetBool ("down", false);
        } else if (prevPos.y < nowPos.y) {
            anim.speed = 1.0f;
            anim.SetBool ("right", false);
            anim.SetBool ("left", false);
            anim.SetBool ("up", true);
            anim.SetBool ("down", false);
        } else if (prevPos.y > nowPos.y) {
            anim.speed = 1.0f;
            anim.SetBool ("right", false);
            anim.SetBool ("left", false);
            anim.SetBool ("up", false);
            anim.SetBool ("down", true);
        } else {
            anim.speed = 0;
        }
        prevPos = transform.position;
    }

    // posの座標までtime秒間かけて移動
    void Move(Vector3 pos, float time) {
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
            firsttime = true;
            return;
        }

        transform.position = firstPos + distance * rate;
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Collision!!!!");
        /*
        if(collision.tag == "Teleport") {
            this.transform.position = MapController.map.TeleportPoint[collision.gameObject];
        }*/
        switch(collision.tag) {
            case "Teleport":
                this.transform.position = MapController.map.TeleportPoint[collision.gameObject];
                break;
            case "Bait":
                PointManager.baites++;
                whichSound = !whichSound;
                if(whichSound)
                    source.clip = bait0;
                else
                    source.clip = bait1;
                source.Play();
                Destroy(collision.gameObject);
                break;
            case "PowerBait":
                PointManager.startTime = Time.timeSinceLevelLoad;
                PointManager.hasPower = true;
                Destroy(collision.gameObject);
                break;
        }
    }

    /*
    void CheckStatus() {
        status[Vector2.up.ToString ()] = !Physics2D.Raycast (transform.position + transform.up * rad, transform.up, 0.5f);
        status[Vector2.right.ToString ()] = !Physics2D.Raycast (transform.position - transform.up * (rad - delta * gap) + transform.right * (rad - delta), transform.right, 0.5f) &&
            !Physics2D.Raycast (transform.position + transform.up * (rad - delta * gap) + transform.right * (rad - delta), transform.right, 0.5f);
        status[Vector2.down.ToString ()] = !Physics2D.Raycast (transform.position - transform.up * rad, -transform.up, 0.5f);
        status[Vector2.left.ToString ()] = !Physics2D.Raycast (transform.position - transform.up * (rad - delta * gap) - transform.right * (rad - delta), -transform.right, 0.5f) &&
            !Physics2D.Raycast (transform.position + transform.up * (rad - delta * gap) - transform.right * (rad - delta), -transform.right, 0.5f);

        Debug.DrawRay (transform.position + transform.up * rad, transform.up * rad);
        Debug.DrawRay (transform.position - transform.up * (rad - delta * gap) + transform.right * (rad - delta), transform.right * 0.5f, Color.blue);
        Debug.DrawRay (transform.position + transform.up * (rad - delta * gap) + transform.right * (rad - delta), transform.right * 0.5f, Color.blue);
        Debug.DrawRay (transform.position - transform.up * rad, -transform.up * rad, Color.red);
        Debug.DrawRay (transform.position - transform.up * (rad - delta * gap) - transform.right * (rad - delta), -transform.right * 0.5f, Color.yellow);
        Debug.DrawRay (transform.position + transform.up * (rad - delta * gap) - transform.right * (rad - delta), -transform.right * 0.5f, Color.yellow);
    }*/

}
