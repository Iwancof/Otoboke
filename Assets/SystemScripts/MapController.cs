using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Threading;
using System.Threading.Tasks;

public class MapController : MonoBehaviour
{

    public static NetworksManager nm;
    public static Map map;
    bool isMapReceived = false;
    bool isMapDeployed = false;
    GameObject player; //To get player coordinate
    PacmanController pacmanController;
    int playerCount;
    Dictionary<int, GameObject> players;
    bool canPlayerAdd = false;
    ClientCoordinateForJson TemporaryCoordinate;
    bool canUpdateCoordinate = false;
    Text textobj;
    public static CancellationTokenSource tokenSource = new CancellationTokenSource();
    private Color[] Colors = { Color.red, Color.green, Color.blue };

    GameObject test_object;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        test_object = GameObject.Find("test_object");
        pacmanController = GameObject.Find("Pacman").GetComponent<PacmanController>();

        nm.ProcessReservation((string str) => {
            map = Map.CreateByString(str);
            isMapReceived = true;
        },"Map");

        nm.ProcessReservation((string str) => {
            playerCount = JsonUtility.FromJson<ForCountPlayerClass>(str).value;
            Debug.Log("Count : " + playerCount);
            canPlayerAdd = true;
        },"CountPlayer");

        textobj = GameObject.Find("LogText").GetComponent<Text>();
    }

    LoopTimer communicateCoordinate = new LoopTimer(0.2f);
    LoopTimer test_player_defeat = new LoopTimer(15f);
    LoopTimer print_player_coordinate = new LoopTimer(0.5f);

    string tmp = "";

    // Update is called once per frame
    void Update()
    {
        LoopTimer.timeUpdate(Time.deltaTime);


        if(canPlayerAdd) {
            AddPlayer();
            canPlayerAdd = false;
        }
        if(canUpdateCoordinate) {
            UpdatePlayerInfo(TemporaryCoordinate);
            canUpdateCoordinate = false;
        }

        if(!isMapDeployed && isMapReceived) {
            map.OverwriteTile();
            isMapDeployed = true;
        }

        if (isMapDeployed && communicateCoordinate.reached) {
            nm.WriteLine(
                $"{player.transform.position.x}," +
                $"{player.transform.position.y}," +
                $"{player.transform.position.z}");
            /*
            Debug.Log(
                "Now position = " + 
                $"{Player.transform.position.x}," +
                $"{Player.transform.position.y}," +
                $"{Player.transform.position.z}");
            */
            nm.ProcessReservation((string str) => {
                tmp = str + '\n' + nm.ReadBuffer.Count + " : " + nm.ProcessMM1.Count;
                //Debug.Log(tmp);
                TemporaryCoordinate = JsonUtility.FromJson<ClientCoordinateForJson>(str);
                canUpdateCoordinate = true;
            },"Reading other client's coordinate");
            //Debug.Log("Count : " + nm.ReadBuffer.Count + "," + nm.ProcessMM1.Count);
        }

        if (isMapDeployed) {
            textobj.text = tmp;
        }

        if (test_player_defeat.reached) { 
            Debug.Log("Defeat");
            //GameObject.Find("Player").GetComponent<Player>().Defeat();
        }

        if(print_player_coordinate.reached) {
            Debug.Log(
                $"{(int)(player.transform.position.x / 1.05f)}," +
                $"{(int)(player.transform.position.y / 1.05f)}," +
                $"{(int)(player.transform.position.z / 1.05f)},");
            test_object.transform.position = new Vector3(
                (int)(player.transform.position.x / 1.05f) * 1.05f,
                (int)(player.transform.position.y / 1.05f) * 1.05f,
                (int)(player.transform.position.z / 1.05f) * 1.05f
                );
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
        foreach(var t in cc.Coordinate.Select((e,i) => (e,i))) {
            if (nm.client_id == t.i) continue;
            players[t.i].transform.position = t.e.ToVector();

            //Debug.Log(Players[t.i].transform.position.ToString());

            //GameObject.Find($"client{t.i}(Clone)").transform.position = t.e.ToVector();
            //Debug.Log(t.e.ToString());
        }
        var time = 0.2f; // 目的の座標まで移動するのに掛ける時間
        pacmanController.targetPos = cc.Pacman.ToVector();
        pacmanController.time = time;
        //Debug.Log("Pacman:" + cc.Pacman.ToString());
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
public class ClientCoordinateForJson {
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
    public CoordinateForJson[] Coordinate;
    public CoordinateForJson Pacman;
    public override string ToString() {
        string ret = "";
        foreach(var t in Coordinate.Select((e,i) => (e,i))) {
            ret += $"{t.i}:({t.e.x},{t.e.y},{t.e.z}),";
        }
        return ret.TrimEnd(',');
    }
}
[System.Serializable]
class ForCountPlayerClass {
    public int value;
};
