using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private float x = 0;
    private float y = 0;
    public float speed = 1.0f;

    private enum direction {
        up = 0,
        down = 1,
        right = 2,
        left = 3
    }
    direction state;
    direction buffer;

    private Dictionary<direction, Ray> ray = new Dictionary<direction, Ray>();
    private RaycastHit hit;
    private float rad;
    private float mar = 0.005f;

    private Rigidbody2D rb;


    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rad = GetComponent<CircleCollider2D>().radius;
    }

    void Update() {
        InputProcessing();
        CheckStatus();
        Move();

    }

    void InputProcessing() {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            buffer = direction.up;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            buffer = direction.down;
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            buffer = direction.right;
        } else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            buffer = direction.left;
        }
    }

    void CheckStatus() {
        ray[direction.up] = new Ray(rb.position, new Vector2(0, rad + mar));
        ray[direction.down] = new Ray(rb.position, new Vector2(0, -rad - mar));
        ray[direction.right] = new Ray(rb.position, new Vector2(rad + mar, 0));
        ray[direction.left] = new Ray(rb.position, new Vector2(-rad - mar, 0));

        foreach(var dir in Enum.GetValues(typeof(direction))) { 
            if(Physics.Raycast(ray[i], out hit, 10.0f)) {

            }
        }

        Debug.DrawRay(ray[direction.up].origin, new Vector2(0, rad), Color.red);
        Debug.DrawRay(ray[direction.down].origin, new Vector2(0, -rad), Color.red);
        Debug.DrawRay(ray[direction.left].origin, new Vector2(-rad, 0), Color.red);
        Debug.DrawRay(ray[direction.right].origin, new Vector2(rad, 0), Color.red);
    }

    void Move() {
        rb.velocity = new Vector2(x, y) * speed;
    }
}
