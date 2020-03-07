using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;
using System.Linq;

public class MapController : MonoBehaviour
{

    NetworksManager nm;
    Map map;
    bool IsMapReceived = false;
    bool IsMapDeployed = false;
    GameObject Player; //To get player coordinate
    int PlayerCount;
    Dictionary<int, GameObject> Players;
    bool CanPlayerAdd = false;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        nm = new NetworksManager();
        nm.Connect();

        nm.ProcessReservation((string str) => {
            map = Map.CreateByString(str);
            IsMapReceived = true;
        },"Map");

        nm.ProcessReservation((string str) => {
            PlayerCount = JsonUtility.FromJson<ForCountPlayerClass>(str).value;
            Debug.Log("Count : " + PlayerCount);
            CanPlayerAdd = true;
        },"CountPlayer");

    }

    float time = 0f;

    string tmp;

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if(CanPlayerAdd) {
            AddPlayer();
            CanPlayerAdd = false;
        }

        if(!IsMapDeployed && IsMapReceived) {
            map.OverwriteTile();
            IsMapDeployed = true;
        }

        if (time >= 0.2 && IsMapDeployed) {
            nm.WriteLine(
                $"{Player.transform.position.x}," +
                $"{Player.transform.position.y}," +
                $"{Player.transform.position.z}");
            //nm.ProcessMM1.Clear();
            //nm.ReadBuffer.Clear(); //うーん...意味わかんない！ｗ
            nm.ProcessReservation((string str) => {
                tmp = (string)str.Clone();
                ClientCoordinateForJson cc = JsonUtility.FromJson<ClientCoordinateForJson>(str);
            },"Reading other client's coordinate");
            //Debug.Log("Count : " + nm.ReadBuffer.Count + "," + nm.ProcessMM1.Count);
            time = 0;
        }

        if (IsMapDeployed) {
        }
    }

    public void AddPlayer() {
        Players = new Dictionary<int, GameObject>();
        for (int i = 0; i < PlayerCount; i++) {
            if (i == nm.client_id) continue;
            GameObject obj = (GameObject)Resources.Load("Player");
            obj.name = $"client{i}";
            Players.Add(i, obj);
            MonoBehaviour.Instantiate(obj, new Vector3(15, 20, 0), Quaternion.identity);
        }
    }

    public static string VectorToString(Vector3 vc) {
        return $"{vc.x},{vc.y},{vc.z}";
    }
}

[System.Serializable]
public class ClientCoordinateForJson {
    [System.Serializable]
    public class CoordinateForJson {
        public float x, y, z;
    }
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
class ForCountPlayerClass {
    public int value;
};
