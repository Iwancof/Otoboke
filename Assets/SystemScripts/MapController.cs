using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;

public class MapController : MonoBehaviour
{

    NetworksManager nm;
    public SafeEntity<Map> map = new SafeEntity<Map>(null);
    public SafeEntity<bool> IsMapReceived = new SafeEntity<bool>(false);
    bool IsMapDeployed = false;
    GameObject Player; //To get player coordinate

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        nm = new NetworksManager();
        nm.Connect();

        nm.GetMapDataAsync(map.getrf(), IsMapReceived.getrf());
    }

    float time = 0f;

    int count = 0;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (!IsMapDeployed && IsMapReceived.value) {
            //map.value.ShowInConsole();
            map.value.OverwriteTile();
            IsMapDeployed = true;
        }

        if (time >= 0.2 && IsMapDeployed) {
            nm.WriteLine(VectorToString(Player.transform.position));
            //nm.WriteLine("");
            time = 0;
        }
    }

    public static string VectorToString(Vector3 vc) {
        return $"{vc.x},{vc.y},{vc.z}";
    }
}
