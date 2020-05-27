using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningController : MonoBehaviour
{
    Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-Vector3.right * 1.5f * Time.deltaTime);
        if(transform.position.x < -12f) {
            transform.position = startPosition;
        }
    }
}
