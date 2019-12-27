using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;


public class NetworksManager
{
    TcpClient client;

    public NetworksManager(string ip,int port) {
        client = new TcpClient(ip, port);
    }
    public NetworksManager() : this("127.0.0.1", 8080) {
        Debug.Log("Selected dafalt server(localhost:8080).");
    }
}
