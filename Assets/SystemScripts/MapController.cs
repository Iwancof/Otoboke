using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;
using System.Linq;

public class MapController : MonoBehaviour
{

    NetworksManager nm;
    //public SafeEntity<Map> map = new SafeEntity<Map>(null);
    //public SafeEntity<bool> IsMapReceived = new SafeEntity<bool>(false);
    Map map;
    bool IsMapReceived = false;
    bool IsMapDeployed = false;
    GameObject Player; //To get player coordinate

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

        MonoBehaviour.Instantiate((GameObject)Resources.Load("WallBlock"),
            new Vector3(0, 0, 0), Quaternion.identity);
    }

    float time = 0f;

    string tmp;

    int count = 0;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

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

