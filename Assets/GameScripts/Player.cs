using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int x = 0;
    private int y = 0;
    public float speed = 1.0f;

    private enum direction {
        up = 0,
        down = 1,
        right = 2,
        left = 3
    }
    direction state;

    private Dictionary<direction, Ray> ray = new Dictionary<direction, Ray>();
    private RaycastHit hit;
    private float rad;
    private float mar = 0.005f;

    private Rigidbody2D rb;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rad = GetComponent<CircleCollider2D>().radius;
    }

    void Update()
    {
        InputProcessing();
        Debug.DrawRay(ray[direction.up].origin, new Vector2(0, rad), Color.red);
        Debug.DrawRay(ray[direction.down].origin, new Vector2(0, -rad), Color.red);
        Debug.DrawRay(ray[direction.left].origin, new Vector2(-rad, 0), Color.red);
        Debug.DrawRay(ray[direction.right].origin, new Vector2(rad, 0), Color.red);

        rb.velocity = new Vector2(x, y) * speed;
    }

    void InputProcessing() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            x = 0;
            y = 1;
            state = direction.up;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            x = 0;
            y = -1;
            state = direction.down;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            y = 0;
            x = 1;
            state = direction.right;
        } else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            y = 0;
            x = -1;
            state = direction.left;
        }

        ray[direction.up] = new Ray(rb.position, new Vector2(0, rad + mar));
        ray[direction.down] = new Ray(rb.position, new Vector2(0, -rad -mar));
        ray[direction.right] = new Ray(rb.position, new Vector2(rad + mar, 0));
        ray[direction.left] = new Ray(rb.position, new Vector2(-rad - mar, 0));
    }

    private direction[] GetSide(direction dir) {
        var tmp = new direction[2];
        switch (dir) {
            case direction.up: {
                goto case direction.down;
            }
            case direction.down: {
                tmp[0] = direction.left;
                tmp[1] = direction.right;
                return tmp;
            }
            case direction.left: { 
                tmp[0] = direction.up;
                tmp[1] = direction.down;
                goto case direction.right;
            }
            case direction.right: {
                tmp[0] = direction.up;
                tmp[1] = direction.down;
                return tmp;
            }
            default: {
                tmp[0] = direction.left;
                tmp[1] = direction.right;
                return tmp;
            }
        }
    }
}
