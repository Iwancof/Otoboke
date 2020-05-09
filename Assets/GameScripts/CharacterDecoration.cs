using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDecoration : MonoBehaviour
{
    Text text;
    enum DecorationType {
        synchroBlink,
        turnBlink,
    }
    [SerializeField]
    Color color;
    Color prevColor;
    [SerializeField]
    DecorationType decoration;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        prevColor = text.color;

        switch(decoration) {
            case DecorationType.synchroBlink: {
                StartCoroutine(Synchro());
                break;
            }
            case DecorationType.turnBlink: {
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Synchro() {
        while(true) {
            text.color = color;
            yield return new WaitForSeconds(0.2f);
            text.color = prevColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Turn() {
        while(true) {
            int i = 0;
            while(text.text.Length > i) {
                //text.text.
                yield return new WaitForSeconds(0.2f);
            }
            
        }
    }
}
