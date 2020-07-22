using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyCameraControll : MonoBehaviour
{
    Camera dummy, main;
    // Start is called before the first frame update
    void Start()
    {
        dummy = GetComponent<Camera>();
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
