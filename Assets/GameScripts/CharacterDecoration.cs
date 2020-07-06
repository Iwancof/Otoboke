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
    [SerializeField]
    float speed = 0.2f;
    [SerializeField, TooltipAttribute("Turn Blink時のみ")]
    float waitTime = 1;
    string turnTxt;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        turnTxt = text.text;
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
                    StartCoroutine(Turn());
                    break;
                }
            }
        }
    }

    IEnumerator Synchro() {
        while(activeObject.activeInHierarchy) {
            text.color = color;
            yield return new WaitForSeconds(speed);
            text.color = prevColor;
            yield return new WaitForSeconds(speed);
        }
    }

    IEnumerator Turn() {
        while(activeObject.gameObject.activeInHierarchy) {
            int i = 0;
            while(turnTxt.Length > i) {
                string txt;
                if(turnTxt.Length-1 > i) {
                    txt = turnTxt.Substring(0, i) + "<color=" + Col2String(color) + ">" + turnTxt[i] + "</color>" + turnTxt.Substring(i+1);
                } else {
                    txt = turnTxt.Substring(0, i) + "<color=" + Col2String(color) + ">" + turnTxt[i] + "</color>";
                }
                text.text = txt;
                yield return new WaitForSeconds(speed);
                i++;
            }
            text.text = turnTxt;
            yield return new WaitForSeconds(waitTime);
        }
    }
    string Col2String(Color color) {
        return "#" + ((int)(color.r * 255)).ToString("X") + ((int)(color.g * 255)).ToString("X") + ((int)(color.b * 255)).ToString("X");
    }
}
