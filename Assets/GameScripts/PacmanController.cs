using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacmanController: MonoBehaviour
{
    private Animator anim;
    private Vector3 prevPos;

    void Start()
    {
        anim = GetComponent<Animator>();
        prevPos = transform.position;
    }

    void Update()
    {
        //transform.position += new Vector3(0.1f, 0, 0);
        if (transform.position == prevPos) {
            anim.SetBool("move", false);
        } else {
            anim.SetBool("move", true);
        }
        prevPos = transform.position;
    }
}
