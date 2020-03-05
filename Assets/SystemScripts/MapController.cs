using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SafePointer;

public class MapController : MonoBehaviour
{

    public SafeEntity<Map> map = new SafeEntity<Map>(null);
    public SafeEntity<bool> IsMapReceived = new SafeEntity<bool>(false);
    bool IsMapDeployed = false;

    // Start is called before the first frame update
    void Start()
    {
        NetworksManager nm = new NetworksManager();
        nm.Connect();

        nm.GetMapDataAsync(map.getrf(), IsMapReceived.getrf());

    }

    // Update is called once per frame
    void Update()
    {
        if(!IsMapDeployed && IsMapReceived.value) {
            //map.value.ShowInConsole();
            map.value.OverwriteTile();
            IsMapDeployed = true;
        }
        
    }
}
