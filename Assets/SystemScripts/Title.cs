using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Title : MonoBehaviour {
    public Text arrowObject, statusObject;
    ArrowState arrowState = ArrowState.JoinToServer;
    public Vector3 initialArrowCoordinate;
    public float arrowMoveSize = 17;
    private bool isConnectingServer = false;
    public static bool canStartGame = false;

    // Start is called before the first frame update
    void Start() {
        initialArrowCoordinate = 
            GameObject.Find("Canvas").transform.position +
            new Vector3(11, 15, 0);
    }

    float time = 0f;

    // Update is called once per frame
    void Update() {
        if(canStartGame) {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
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

        if (Input.GetKey(KeyCode.J)) arrowState++;
        if (Input.GetKey(KeyCode.K)) arrowState--;
        EnumBoundCheck();

        if (Input.GetKey(KeyCode.Return)) {
            ExecuteCommand(arrowState);
        }


        arrowObject.rectTransform.position = initialArrowCoordinate - (int)arrowState * arrowMoveSize * new Vector3(0, -1, 0);
    }

    private void ExecuteCommand(ArrowState e) {
        switch(e) {
            case ArrowState.JoinToServer: {
                statusObject.text = "Connecting....";

                MapController.nm = new NetworksManager("localhost", 8080);
                //MapController.nm = new NetworksManager("2400:4051:99c2:5800:2cad:351d:e2d8:fc07", 5522);
                MapController.nm.Connect();
                isConnectingServer = true;

                break;
            }
        }
    }

    enum ArrowState {
        JoinToServer = 0,
    }
    private void EnumBoundCheck() =>
        arrowState = (ArrowState)Math.Max(0, Math.Min((int)ArrowState.JoinToServer, (int)arrowState));
}
