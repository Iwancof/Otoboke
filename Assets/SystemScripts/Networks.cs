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

public class NetworksManager
{
    public string ip;
    public int port;
    public int client_id;

    TcpClient client;
    Stream stream;
    StreamReader reader;

    IEnumerator<string> ReadBuffer;

    public NetworksManager(string ip,int port) {
        this.ip = ip;
        this.port = port;
    }
    public NetworksManager() : this("localhost", 8080) {
        Debug.Log("Selected dafalt server(localhost:8080).");
    }
    class tmp { public int counter; };
    public void Connect() {
        client = new TcpClient(ip, port);
        stream = client.GetStream();
        reader = new StreamReader(stream);

        ReadBuffer = ReadUntil(reader, '|');

        client_id = JsonUtility.FromJson<tmp>(ReadBuffer.Next()).counter;
    }
    
    public string test() {
        byte[] data = new byte[1024];
        int bytes = stream.Read(data, 0, data.Length);
        return Encoding.ASCII.GetString(data, 0, bytes);
    }

    public async void GetMapDataAsync(SafePointer<Map> map_ptr,SafePointer<bool> ptr) {
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
}

static class StringBufferItarator {
    public static string Next(this IEnumerator<string> enumerator) {
        enumerator.MoveNext();
        return enumerator.Current;
    }
}
