using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warp : MonoBehaviour
{

    public GameObject warp;
    GameObject parent;
    void Start() {
        if (warp == null)
            Debug.LogError("ワープオブジェクトがセットされてません");
        parent = transform.root.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player") {
            collision.transform.position = warp.transform.position + new Vector3(warp.transform.localPosition.x > 0 ? -1.1f : 1.1f, 0, 0);
        }
    }

}

