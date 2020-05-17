using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MapController : MonoBehaviour {

    public static NetworksManager nm;
    public static Map map;
        bool 
        isMapReceived = false,
        isMapDeployed = false,
        canPlayerAdd = false,
        canPlayerUpdateCoordinate = false,
        canPacmanUpdateCoordinate = false;
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

    GameObject test_object;

    // Start is called before the first frame update
    void Start() {
        player = GameObject.Find("Player");
        test_object = GameObject.Find("test_object");
        pacmanController = GameObject.Find("Pacman").GetComponent<PacmanController>();

        nm.ProcessReservation((string str) => {
            map = Map.CreateByString(str);
            isMapReceived = true;
            //nm.WriteLine("MAP_RECEIVED|");
        }, "Map");

        nm.ProcessReservation((string str) => {
            playerCount = JsonUtility.FromJson<ForCountPlayerClass>(str).value;
            Debug.Log("Count : " + playerCount);
            canPlayerAdd = true;
        }, "CountPlayer");

        textobj = GameObject.Find("LogText").GetComponent<Text>();
    }

    LoopTimer communicateCoordinate = new LoopTimer(0.08f);
    LoopTimer update_bait_by_server = new LoopTimer(0.08f);
    LoopTimer test_player_defeat = new LoopTimer(15f);
    LoopTimer print_player_coordinate = new LoopTimer(0.1f);
    LoopTimer test_print_queue = new LoopTimer(0.2f);

    string tmp = "";

    // Update is called once per frame
    void Update() {
        LoopTimer.timeUpdate(Time.deltaTime);

        if (canPlayerAdd) {
            AddPlayer();
            canPlayerAdd = false;
        }
        if (canPlayerUpdateCoordinate) {
            UpdatePlayerInfo(PlayerTemporaryCoordinate);
            canPlayerUpdateCoordinate = false;
        }
        if(canPacmanUpdateCoordinate) {
            UpdatePacmanInfo(PacmanTemporaryCoordinate);
            canPacmanUpdateCoordinate = false;
        }


        if (!isMapDeployed && isMapReceived) {
            map.OverwriteTile();
            isMapDeployed = true;

            void tmp_func(string str) {
                nm.ProcessReservation(tmp_func, "Get coordinate LoopCast()A");
                tmp = str + '\n' + nm.ReadBuffer.Count + " : " + nm.ProcessMM1.Count;

                var get_data = str.TrimTo(';');
                if (get_data.Tag == "PLAYER") {
                    PlayerTemporaryCoordinate = JsonUtility.FromJson<ClientCoordinateForJson>(get_data.Data);
                    canPlayerUpdateCoordinate = true;
                } else if (get_data.Tag == "PACMAN") {
                    PacmanTemporaryCoordinate = JsonUtility.FromJson<PacmanCoordinateForJson>(get_data.Data);
                    canPacmanUpdateCoordinate = true;
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

        if(update_bait_by_server.reached) {
            foreach(var e in PacedTemporaryCoordinates) {
                //Debug.Log($"({e.x}, {e.y})");
                //Debug.Log("Destroy ... " + map.DestroyList[(e.x, e.y)]);
                pacmanController.PacBaitAt(e.x, e.y);
            }
            PacedTemporaryCoordinates = new List<(int x, int y)>();
        }


        if (test_print_queue.reached) {
            //nm.QueueLog();
        }

        if (isMapDeployed && communicateCoordinate.reached) {
            nm.WriteLine(
                $"{player.transform.position.x}," +
                $"{player.transform.position.y}," +
                $"{player.transform.position.z}");
        }

        if (isMapDeployed) {
            textobj.text = tmp;
        }

        if (test_player_defeat.reached) {
            Debug.Log("Defeat");
        }

        if (print_player_coordinate.reached) {
            /*
            test_object.transform.position = new Vector3(
                (int)((player.transform.position.x - size / 2 + 1) / size) * size,
                (int)((player.transform.position.y - size / 2 + 1) / size) * size,
                (int)((player.transform.position.z - size / 2 + 1) / size) * size
                );
            */
            /*
            Debug.Log($"" +
                $"{(int)((player.transform.position.x - size / 2 + 1) / size)}," +
                $"{(int)((player.transform.position.y - size / 2 + 1) / size)}," +
                $"{(int)((player.transform.position.z - size / 2 + 1) / size)}"
                );
            */
        }
    }

    public void AddPlayer() {
        players = new Dictionary<int, GameObject>();
        for (int i = 0; i < playerCount; i++) {
            if (i == nm.client_id) continue;
            var obj = MonoBehaviour.Instantiate((GameObject)Resources.Load("Player"), new Vector3(15, 20, 0), Quaternion.identity);
            obj.name = $"client{i}";
            obj.GetComponent<SpriteRenderer>().material.color = Colors[i];
            players.Add(i, GameObject.Find($"client{i}"));
        }
    }
    public void UpdatePlayerInfo(ClientCoordinateForJson cc) {
        foreach (var t in cc.Coordinate.Select((e, i) => (e, i))) {
            if (nm.client_id == t.i) continue;
            players[t.i].transform.position = t.e.ToVector();

            //Debug.Log(Players[t.i].transform.position.ToString());

            //GameObject.Find($"client{t.i}(Clone)").transform.position = t.e.ToVector();
            //Debug.Log(t.e.ToString());
        }
        /*
        var time = 0.2f; // 目的の座標まで移動するのに掛ける時間
        pacmanController.targetPos = cc.Pacman.ToVector();
        pacmanController.time = time;
        */
        //Debug.Log("Pacman:" + cc.Pacman.ToString());
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


