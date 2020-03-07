using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;

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
            nm.ProcessReservation((string str) => {

            },"Reading other client's coordinate");
            time = 0;
        }

        if (IsMapDeployed) {
        }
    }

    public static string VectorToString(Vector3 vc) {
        return $"{vc.x},{vc.y},{vc.z}";
    }
}

