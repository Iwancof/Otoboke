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
//using System.Date;

public delegate void ProcessManager(string data);

public class NetworksManager {
    public string ip;
    public int port;
    public int client_id;

    public TcpClient client;
    public Stream stream;
    public StreamReader reader;
    public StreamWriter writer;
    public bool IsNetworkClientInitialized { get; private set; } = false;

    public Queue<string> ReadBuffer;
    public Queue<KeyValuePair<string,ProcessManager>> ProcessMM1;

    public NetworksManager(string ip, int port) {
        this.ip = ip;
        this.port = port;
        ReadBuffer = new Queue<string>();
        ProcessMM1 = new Queue<KeyValuePair<string, ProcessManager>>();
    }

    public NetworksManager() : this("localhost", 8080) {
        Debug.Log("[Warning]Selected dafalt server(localhost:8080).");
    }
    [System.Serializable]
    class ForIDCounterClass { public int counter; };
    public void Connect() {
        (new Action(async () => {
            await Task.Run(() => {
                try {
                    client = new TcpClient(ip, port);
                } catch (SocketException) {
                    Debug.LogError($"[Error] Socket client to {ip}:{port} is timeout.");
                    return;
                } catch (AggregateException) {
                    Debug.LogError($"[Error] Could not connect client to {ip}:{port}.");
                    return;
                } catch (Exception e) {
                    Debug.LogError($"[Error] Unknown error occured. {e.Message}");
                    return;
                }

                IsNetworkClientInitialized = true;

                stream = client.GetStream();
                reader = new StreamReader(stream, Encoding.UTF8);
                writer = new StreamWriter(stream, Encoding.UTF8);
                stream.WriteTimeout = 100;

                StartReadingNetwork('|');
                StartProcessDequeue();

                ProcessReservation((string str) => {
                    client_id = JsonUtility.FromJson<ForIDCounterClass>(str).counter;
                    Debug.Log("id = " + client_id);
                }, "Json");

                return;
            });
        }))();
        System.Threading.Thread.Sleep(1000);
        if (!IsNetworkClientInitialized) {
            Debug.LogError("[Error] Connection timeout");
            throw new SocketException(10060);
        }
    }

    public void WriteLine(string str) {
        str += '\n';
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
        Debug.Log("Send : " + str);
        stream.Write(bytes, 0, bytes.Length);
    }

    public async void StartProcessDequeue() {
        await Task.Run(() => {
            while (true) {
                try {
                    while (ReadBuffer.Count < 1 || ProcessMM1.Count < 1) {
                        if (ProcessMM1.Count >= 10) Debug.LogError("Process reservation limit reached. name : " + ProcessMM1.Dequeue().Key);
                        if (ReadBuffer.Count >= 10) Debug.LogError("ReadBuffer limit reached. data : " + ReadBuffer.Dequeue());
                    }
                    //Debug.Log("Dequeue : " + ReadBuffer.Peek());
                    ProcessMM1.Dequeue().Value(ReadBuffer.Dequeue());
                } catch(Exception e) {
                    Debug.LogError($"[Error] {e.Message}");
                }
            }
        });
    }

    public void ProcessReservation(ProcessManager pm,string name) {
        //Debug.Log($"Process name ({name}) joined to MM1 queue");
        ProcessMM1.Enqueue(new KeyValuePair<string, ProcessManager>(name, pm));
    }

    public async void StartReadingNetwork(char delim) {
        InitCheck();
        await Task.Run(() => {
            StringBuilder sb = new StringBuilder();
            char c;
            while (true) {
                c = (char)reader.Read();
                if(c == delim) {
                    ReadBuffer.Enqueue(sb.ToString());

                    sb.Clear();
                    continue;
                }
                sb.Append(c);
            }
        });
    }


    private void InitCheck() {
        if (!IsNetworkClientInitialized) throw new Exception("NotworkManager has not be initialized yet");
    }
}
