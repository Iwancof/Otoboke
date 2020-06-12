using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Title : MonoBehaviour {
    public Text arrowObject, statusObject;
    ArrowState arrowState = ArrowState.JoinToServer;
    ArrowState prevArrowState = ArrowState.JoinToServer;
    public Vector3 initialArrowCoordinate;
    public float arrowMoveSize = 17;
    private bool isConnectingServer = false;
    public static bool canStartGame = false;
    public AudioClip selectSound;
    public AudioClip enterSound;
    AudioSource source;

    // Start is called before the first frame update
    void Start() {
        initialArrowCoordinate = 
            GameObject.Find("Canvas").transform.position +
            new Vector3(11, 15, 0);
        canStartGame = false;
        isConnectingServer = false;
        source = GetComponent<AudioSource>();
    }

    float time = 0f;

    // Update is called once per frame
    void Update() {
        if(canStartGame) {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
            return;
        }

        if (isConnectingServer) {
            time += Time.deltaTime;
            if(time <= 1f) return;
            if (MapController.nm.IsNetworkClientInitialized) {//ネットワークの初期化が完了
                statusObject.text = "Wait other players...";
                isConnectingServer = false;
                return;
            }
            statusObject.text = "Connection refused";
            isConnectingServer = false;
            time = 0f;
        }

        if (Input.GetKeyDown(KeyCode.J)) {
            arrowState++;
            source.clip = selectSound;
            source.Play();
        }
        if (Input.GetKeyDown(KeyCode.K)) {
            arrowState--;
            source.clip = selectSound;
            source.Play();
        }
        EnumBoundCheck();
        string[] str = GetString();
        str[(int)prevArrowState] = "　" + str[(int)prevArrowState].Remove(0, 1);
        str[(int)arrowState] = "▶" + str[(int)arrowState].Remove(0, 1);
        SetString(str);
        //statusObject.text = arrowState.ToString() + ":" + str.Length;

        if (Input.GetKey(KeyCode.Return)) {
            ExecuteCommand(arrowState);
        }


        //arrowObject.rectTransform.position = initialArrowCoordinate - (int)arrowState * arrowMoveSize * new Vector3(0, -1, 0);
        prevArrowState = arrowState;
    }

    private void ExecuteCommand(ArrowState e) {
        source.clip = enterSound;
        source.Play();
        switch(e) {
            case ArrowState.JoinToServer: {
                statusObject.text = "Connecting....";

                MapController.nm = new NetworksManager("192.168.1.7", 5522);
                MapController.nm.Connect();
                isConnectingServer = true;
                break;
            }
            case ArrowState.Selectserver: {
                
                break;
            }
            case ArrowState.Howtoplay: {

                break;
            }
            case ArrowState.Exit: {
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                #elif UNITY_STANDALONE
                Application.Quit();
                #endif
                break;
            }
        }
    }

    string[] GetString() {
        return arrowObject.text.Split('\n');
    }

    void SetString(string[] str) {
        string res = "";
        foreach(var tmp in str) {
            res += tmp + "\n";
        }
        res = res.Remove(res.Length - 1);
        arrowObject.text = res;
    }

    enum ArrowState {
        JoinToServer = 0,
        Selectserver = 1,
        Howtoplay = 2,
        Exit = 3,
    }
    private void EnumBoundCheck() =>
        arrowState = (ArrowState)Math.Max(0, Math.Min((int)ArrowState.Exit, (int)arrowState));
}
