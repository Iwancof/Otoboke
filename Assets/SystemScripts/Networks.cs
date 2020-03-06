using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System;
using SafePointer;

public class NetworksManager {
    public string ip;
    public int port;
    public int client_id;

    public TcpClient client;
    public Stream stream;
    StreamReader reader;
    public StreamWriter writer;
    public bool IsNetworkClientInitialized { get; private set; } = false;

    IEnumerator<string> ReadBuffer;

    public NetworksManager(string ip, int port) {
        this.ip = ip;
        this.port = port;
    }
    public NetworksManager() : this("localhost", 8080) {
        Debug.Log("[Warning]Selected dafalt server(localhost:8080).");
    }
    class tmp { public int counter; };
    public void Connect() {
        try {
            client = new TcpClient(ip, port);
        } catch (SocketException e) {
            Debug.Log($"[Error]Could not bind server. server : {ip}:{port}");
            throw e;
        }
        IsNetworkClientInitialized = true;
        stream = client.GetStream();
        reader = new StreamReader(stream);
        writer = new StreamWriter(stream,Encoding.UTF8);

        ReadBuffer = ReadUntil(reader, '|');

        client_id = JsonUtility.FromJson<tmp>(ReadBuffer.Next()).counter;
    }

    public void WriteLine(string str) {
        //writer.WriteLine(str);
        byte[] bytes = Encoding.UTF8.GetBytes(str + '\n');
        stream.Write(bytes, 0, bytes.Length);
    }
    
    public async void GetMapDataAsync(SafePointer<Map> map_ptr,SafePointer<bool> ptr) {
        InitCheck();
        string result = await Task.Run<string>(new Func<string>(ReadBuffer.Next));
        map_ptr.indir(Map.CreateByString(result));
        ptr.indir(true);
    }

    public static IEnumerator<string> ReadUntil(StreamReader sr, char delim) {
        StringBuilder sb = new StringBuilder();

        while (!sr.EndOfStream) {
            char c = (char)sr.Read();

            if (c == delim) {
                yield return sb.ToString();
                sb = new StringBuilder();
                continue;
            }

            sb.Append(c);
        }

        //return sb.ToString();
    }
    private void InitCheck() {
        if (!IsNetworkClientInitialized) throw new Exception("NotworkManager has not be initialized yet");
    }
}

static class StringBufferItarator {
    public static string Next(this IEnumerator<string> enumerator) {
        enumerator.MoveNext();
        return enumerator.Current;
    }
}
