using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

public delegate void MainThreadTransfer();

public class MapController : MonoBehaviour {

    public static NetworksManager nm;
    public static Map map;
        bool 
        isMapReceived = false,
        isMapDeployed = false,
        canPlayerAdd = false,
        canPlayerUpdateCoordinate = false,
        canPacmanUpdateCoordinate = false,
        GameReady = false;
    GameObject player; //To get player coordinate
    PacmanController pacmanController;
    int playerCount;
    Dictionary<int, GameObject> players;
    ClientCoordinateForJson PlayerTemporaryCoordinate;
    PacmanCoordinateForJson PacmanTemporaryCoordinate;
    List<(int x, int y)> PacedTemporaryCoordinates = new List<(int x, int y)>();
    Text textobj;
    public static CancellationTokenSource tokenSource = new CancellationTokenSource();
    private Color[] Colors = { Color.red, Color.green, Color.blue };
    public static float size = 1.05f; //this will fix

    public enum SystemStatus {
        WaitServerCommunication,
        WaitPlayOpening,
        WaitOtherPlayer,
        GameStarted,
    };
    public static SystemStatus systemStatus; // いろいろなところで書き換わるので注意。

    Queue<MainThreadTransfer> mainThreadTransfers = new Queue<MainThreadTransfer>();

    GameObject test_object;

    // Start is called before the first frame update
    void Start() {
        /* ロガーの有効化 */
        Logger.enable(Logger.GameSystemProcTag);
        Logger.enable(Logger.CommunicationDebugTag);

        systemStatus = SystemStatus.WaitServerCommunication;

        player = GameObject.Find("Player");
        test_object = GameObject.Find("test_object");
        pacmanController = GameObject.Find("Pacman").GetComponent<PacmanController>();

        /* マップ情報取得 */
        nm.ProcessReservation((string str) => {
            map = Map.CreateByString(str);
            isMapReceived = true;
        }, "Map");

        /* サーバとの通信の初期化 */
        nm.ProcessReservation((string str) => {
            playerCount = JsonUtility.FromJson<ForCountPlayerClass>(str).value;
            Logger.Log(Logger.CommunicationShowTag, "Count : " + playerCount);
            Logger.Log(Logger.CommunicationDebugTag, "Count : " + playerCount);
            //canPlayerAdd = true;
            doInMainThread(new MainThreadTransfer(() => {
                Logger.Log(Logger.GameSystemProcTag, "Call add player" );
                AddPlayer();
            }));

            systemStatus = SystemStatus.WaitPlayOpening;
        }, "CountPlayer");

        nm.ProcessReservation((string str) => {
            Logger.Log(Logger.CommunicationDebugTag, str);
            if (str == "GameReady") {
                GameReady = true;
            }
        }, "WaitGameReady");

        textobj = GameObject.Find("LogText").GetComponent<Text>();
    }

    LoopTimer communicateCoordinate = new LoopTimer(0.2f);
    LoopTimer update_bait_by_server = new LoopTimer(0.08f);

    FirstTimeClass toServerEndEffect = new FirstTimeClass();

    // Update is called once per frame
    void Update() {

        /*
        if (canPlayerAdd) {
            Logger.Log(Logger.GameSystemProcTag, "Call add player" );
            AddPlayer();
            canPlayerAdd = false;
        }
        */

        if (!isMapDeployed && isMapReceived) {
            map.OverwriteTile();
            isMapDeployed = true;
            pacmanController.Baits = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bait"));

            void tmp_func(string str) {
                nm.ProcessReservation(tmp_func, "Get coordinate LoopCast()A");

                var get_data = str.TrimTo(';');
                if (get_data.Tag == "PLAYER") {
                    PlayerTemporaryCoordinate = JsonUtility.FromJson<ClientCoordinateForJson>(get_data.Data);
                    doInMainThread(new MainThreadTransfer(() => {
                        UpdatePlayerInfo(PlayerTemporaryCoordinate);
                    }));
                } else if (get_data.Tag == "PACMAN") {
                    PacmanTemporaryCoordinate = JsonUtility.FromJson<PacmanCoordinateForJson>(get_data.Data);
                    doInMainThread(new MainThreadTransfer(() => {
                        UpdatePacmanInfo(PacmanTemporaryCoordinate);
                    }));
                } else if (get_data.Tag == "PACCOL") {
                    PacedCoordinateForJson paced_crd = JsonUtility.FromJson<PacedCoordinateForJson>(get_data.Data);
                    foreach (var e in paced_crd.Coordinate) {
                        PacedTemporaryCoordinates.Add(((int)e.x, (int)e.y));
                    }
                } else {
                    throw new Exception($"{get_data.Tag} is Invalid tag. ");
                }

            }
            nm.ProcessReservation(tmp_func, "Get coordinate LoopCast()B");
        }
        switch (systemStatus) {
            case SystemStatus.WaitServerCommunication: {
                /* waiting... */
                /* nm.ProcessReservertionのCountPlayerで書き換わる。 */
                return;
            }
            case SystemStatus.WaitPlayOpening: {
                /* waiting... */
                /* AutoControllerのUpdate内で書き換わる。 */
                return;
            }
            case SystemStatus.WaitOtherPlayer: {
                if (toServerEndEffect) {
                    nm.WriteLine("END_EFFECT");
                }
                if (GameReady) {
                    systemStatus = SystemStatus.GameStarted;
                }
                return;
            }
            case SystemStatus.GameStarted: {
                LoopTimer.timeUpdate(Time.deltaTime);
                break;
            }
            default: {
                Debug.LogError("Invalid SystemStatus");
                break;
            }
        }

        while(mainThreadTransfers.Count() != 0) {
            mainThreadTransfers.Dequeue()();
        }

        if (update_bait_by_server.reached) {
            /* これが呼ばれるたびに、食べられた餌の処理が入る。 */
            foreach(var e in PacedTemporaryCoordinates) {
                Logger.Log(Logger.GameSystemPacTag, $"({e.x}, {e.y})");
                pacmanController.PacBaitAt(e.x, e.y);
            }
            PacedTemporaryCoordinates = new List<(int x, int y)>();
        }

        if (isMapDeployed && communicateCoordinate.reached) {
            /* サーバにプレイヤーの情報を送信 */
            Debug.Log(count++);
            nm.WriteLine(
                $"{player.transform.position.x}," +
                $"{player.transform.position.y}," +
                $"{player.transform.position.z}");
        }
    }
    int count = 0;

    public void doInMainThread(MainThreadTransfer mtt) {
        mainThreadTransfers.Enqueue(mtt);
    }

    public void AddPlayer() {
        players = new Dictionary<int, GameObject>();
        for (int i = 0; i < playerCount; i++) {
            if (i == nm.client_id) continue;
            var obj = MonoBehaviour.Instantiate((GameObject)Resources.Load("OtherPlayer"), new Vector3(15, 20, 0), Quaternion.identity);
            obj.name = $"client{i}";
            obj.transform.Find("Anim").GetComponent<SpriteRenderer>().material.color = Colors[i];
            players.Add(i, GameObject.Find($"client{i}"));
        }
    }
    public void UpdatePlayerInfo(ClientCoordinateForJson cc) {
        foreach (var t in cc.Coordinate.Select((e, i) => (e, i))) {
            if (nm.client_id == t.i) continue;
            players[t.i].transform.position = t.e.ToVector();
        }
    }
    public void UpdatePacmanInfo(PacmanCoordinateForJson cc) {
        pacmanController.targetPos = cc.Pacman.ToVector() * size;
        pacmanController.time = 0.2f;
    }

    public static string VectorToString(Vector3 vc) {
        return $"{vc.x},{vc.y},{vc.z}";
    }

    private void OnApplicationQuit() {
        tokenSource.Cancel();
    }
}

class FirstTimeClass {
    bool b = true;
    public FirstTimeClass() { }
    public static implicit operator bool(FirstTimeClass f) {
        bool ret = f.b;
        f.b = false;
        return ret;
    }
}


public class LoopTimer {
    private static List<LoopTimer> objects = new List<LoopTimer>();
    private float total = 0f;
    private float threshold;

    public LoopTimer(float _thres) {
        threshold = _thres;
        objects.Add(this);
    }

    public bool reached {
        get {
            if (threshold <= total) {
                total = 0;
                return true;
            }
            return false;
        }
    }

    public static void timeUpdate(float delta_time) {
        foreach (var e in objects) {
            e.total += delta_time;
        }
    }
}

[System.Serializable]
public class CoordinateForJson {
    public float x, y, z;
    public Vector3 ToVector() {
        return new Vector3(x, y, z);
    }
    public override string ToString() {
        return $"{x},{y},{z}";
    }
}

[System.Serializable]
public class ClientCoordinateForJson {
    public CoordinateForJson[] Coordinate;
    public override string ToString() {
        string ret = "";
        foreach(var t in Coordinate.Select((e,i) => (e,i))) {
            ret += $"{t.i}:({t.e.x},{t.e.y},{t.e.z}),";
        }
        return ret.TrimEnd(',');
    }
}

[System.Serializable]
public class PacmanCoordinateForJson {
    public CoordinateForJson Pacman;
}

[System.Serializable]
public class PacedCoordinateForJson {
    public CoordinateForJson[] Coordinate;
}

[System.Serializable]
class ForCountPlayerClass {
    public int value;
};

static class StringExtension {
    public static (string Tag, string Data) TrimTo(this string x, char d) {
        int index = x.IndexOf(d);
        return (Tag: x.Substring(0, index), Data: x.Substring(index + 1, x.Length - index - 1));
    }
}


