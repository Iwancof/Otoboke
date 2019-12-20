using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using System.Net.Sockets;
using System.Net;

namespace OtobokeServer {
    class Program {
        static int port = 10000;
        static IPAddress addr = IPAddress.Parse("127.0.0.1");

        static void Main(string[] args) {
            TcpListener listener = new TcpListener(addr, port);
            listener.Start();

            TcpClient client = listener.AcceptTcpClient();

            WriteLine("Server started!");
            NetworkStream stream = client.GetStream();

            //byte[] buffer = null;
            byte[] buffer = new byte[255];

            int size;

            while ((size = stream.Read(buffer, 0, buffer.Length)) != 0) {
                string data = Encoding.ASCII.GetString(buffer, 0, size);
                WriteLine("Received data = " + data);

                byte[] write_data = { 1, 2, 3, 4, 5 };
                stream.Write(write_data, 0, write_data.Length);
            }



            client.Close();
        }
    }
}
