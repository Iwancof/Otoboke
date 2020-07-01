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
    Color color = default;
    Color prevColor;
    [SerializeField]
    DecorationType decoration = default;
    bool firstFlag = true;
    [SerializeField]
    GameObject activeObject = default;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        prevColor = text.color;
        firstFlag = true;
    }
    
    void OnEnable() {
        firstFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(activeObject.activeInHierarchy && firstFlag) {
            firstFlag = false;
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
    }

    IEnumerator Synchro() {
        while(activeObject.activeInHierarchy) {
            text.color = color;
            yield return new WaitForSeconds(0.2f);
            text.color = prevColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    IEnumerator Turn() {
        while(activeObject.gameObject.activeInHierarchy) {
            int i = 0;
            while(text.text.Length > i) {
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}
