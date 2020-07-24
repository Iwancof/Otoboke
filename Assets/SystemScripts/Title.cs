using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class Title : MonoBehaviour {
    public Text arrowObject, statusObject;
    ArrowState arrowState = ArrowState.JoinToServer;
    ArrowState prevArrowState = ArrowState.JoinToServer;
    PageState pageState = PageState.MainMenu;
    PageState prevPageState = PageState.MainMenu;
    public Vector3 initialArrowCoordinate;
    public float arrowMoveSize = 17;
    private bool isConnectingServer = false;
    public static bool canStartGame = false;
    public AudioClip selectSound;
    public AudioClip enterSound;
    AudioSource source;

    [SerializeField]
    GameObject mainMenu = default, howToPlay = default, settings = default;
    InputField ipObject;

    Vector2 swipeDir = Vector2.zero;

    string ip = "192.168.179.2";//"35.188.97.54";
    int port = 5522;

    // Start is called before the first frame update
    void Start() {
        PlayerPrefs.SetString("IP", "35.188.97.54");
        initialArrowCoordinate = 
            GameObject.Find("Canvas").transform.position +
            new Vector3(11, 15, 0);
        canStartGame = false;
        isConnectingServer = false;
        source = GetComponent<AudioSource>();
        ipObject = settings.transform.Find("InputField").GetComponent<InputField>();
        ip = PlayerPrefs.GetString("IP", ip);
        settings.transform.Find("InputField").Find("Placeholder").gameObject.GetComponent<Text>().text = ip;
    }

    float time = 0f;
    (float, float) swipeLog = (0, 0);

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

        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) {
            swipeDir = Swipe.SwipeDirection();
            swipeLog = (swipeLog.Item1 + swipeDir.x, swipeLog.Item2 + swipeDir.y);
        }

        switch(pageState) {
            case PageState.MainMenu: {
                mainMenu.SetActive(true);
                settings.SetActive(false);
                howToPlay.SetActive(false);

                if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.DownArrow) || swipeDir == Vector2.down) {
                    arrowState++;
                    source.clip = selectSound;
                    source.Play();
                } else if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.UpArrow) || swipeDir == Vector2.up) {
                    arrowState--;
                    source.clip = selectSound;
                    source.Play();
                }
                EnumBoundCheck();
                string[] str = GetString();
                 //"　" + str[(int)prevArrowState].Remove(0, 1);
                char[] tmp = str[(int)prevArrowState].ToCharArray();
                if(tmp.Length >= 14) {
                    tmp[0] = '　';
                    tmp[14] = 'F';
                    tmp[15] = 'F';
                    str[(int)prevArrowState] = new String(tmp);
                }
                char[] tmp2 = str[(int)arrowState].ToCharArray();
                if(tmp2.Length >= 14) {
                    tmp2[0] = '▶';
                    tmp2[14] = '0';
                    tmp2[15] = '0';
                    str[(int)arrowState] = new String(tmp2);
                }
                SetString(str);

                if (Input.GetKeyDown(KeyCode.Return) || swipeDir == Vector2.right) {
                    ExecuteCommand(arrowState);
                }
                prevArrowState = arrowState;
                break;
            }

            case PageState.HowToPlay: {
                mainMenu.SetActive(false);
                settings.SetActive(false);
                howToPlay.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Return) || swipeDir == Vector2.left) {
                    source.clip = enterSound;
                    source.Play();
                    pageState = PageState.MainMenu;
                }
                break;
            }
            case PageState.Settings: {
                mainMenu.SetActive(false);
                howToPlay.SetActive(false);
                settings.SetActive(true);
                if(Input.GetKeyDown(KeyCode.Return) || swipeDir == Vector2.left) {
                    source.clip = enterSound;
                    source.Play();
                    pageState = PageState.MainMenu;
                }
                break;
            }
        }
        prevPageState = pageState;

        //arrowObject.rectTransform.position = initialArrowCoordinate - (int)arrowState * arrowMoveSize * new Vector3(0, -1, 0);
    }

    /// <summary>
    /// IPアドレスの入力とフォーマットの確認
    /// </summary>
    public void InputLogger() {
        ip = ipObject.text;
        PlayerPrefs.SetString("IP", ip);
        return;
        /*
        int[] tmpIp = null;
        try {
            tmpIp = ipObject.text.Split('.').ToList().Select(x => int.Parse(x)).ToArray();
            bool err = false;
            foreach(var t in tmpIp) {
                if(!(0 <= t && t <= 255)) {
                    err = true;
                    break;
                }
            }
            if(tmpIp.Length != 4) err = true;
            if(err) {
                Debug.LogError("ipアドレスのフォーマットが違うよ");
                return;
            }
            ip = ipObject.text;
            settings.transform.Find("InputField").Find("Placeholder").gameObject.GetComponent<Text>().text = ip;
        } catch(FormatException fe) {
            Debug.LogError("ipアドレスのフォーマットが違うよ:" + fe);
            return;
        }
        ipObject.text = "";*/
    }

    private void ExecuteCommand(ArrowState e) {
        source.clip = enterSound;
        source.Play();
        switch(e) {
            case ArrowState.JoinToServer: {
                statusObject.text = "Connecting....";

                MapController.nm = new NetworksManager(ip, port);
                MapController.nm.Connect();
                isConnectingServer = true;
                break;
            }
            case ArrowState.Settings: {
                // おかしい？
                if(pageState != PageState.Settings)
                    pageState = PageState.Settings;
                break;
            }
            case ArrowState.Howtoplay: {
                if(pageState != PageState.HowToPlay)
                    pageState = PageState.HowToPlay;
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
        Settings = 1,
        Howtoplay = 2,
        Exit = 3,
    }
    enum PageState {
        MainMenu = 0,
        Settings = 1,
        HowToPlay = 2,
        Ranking = 3,
    }
    private void EnumBoundCheck() =>
        arrowState = (ArrowState)Math.Max(0, Math.Min((int)ArrowState.Exit, (int)arrowState));
}
