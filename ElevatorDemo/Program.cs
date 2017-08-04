using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConnectAsTcpClient();
            Console.ReadLine();
        }

        private static readonly string[] ClientRequestString =
        {
            "{\"User\":\"Student\",\"Elevators\":[{\"ID\": 1,\"Capability\": 1500,\"FloorMax\": 25,\"FloorHeight\": 10,\"InitHeight\": 20}],\"TaskID\":\"2\",\"Operation\":\"CONFIG\"}"
            ,"{\"Tick\":0,\"FinishRequests\":[],\"Operation\":\"GETREQS\"}"
            ,"{\"Tick\":-1,\"FinishRequests\":[{\"PassengerName\":\"Sen_1\",\"FinishTime\":20,\"ElevatorID\":1}],\"Operation\":\"GETREQS\"}"};
        private static async void ConnectAsTcpClient()
        {
            using (var tcpClient = new TcpClient())
            {
                Console.WriteLine("Client >> Connecting to autotest framework");
                await tcpClient.ConnectAsync("127.0.0.1", 8989);
                Console.WriteLine("Client >> Connected to autotest framework");
                using (var networkStream = tcpClient.GetStream())
                {
                    foreach (var s in ClientRequestString)
                    {
                        while (!networkStream.CanWrite)
                        {
                            Thread.Sleep(20);
                        }
                        Console.WriteLine("Client >> Send Data :\t {0}", s);
                        var clientRequestBytes = Encoding.UTF8.GetBytes(s);
                        await networkStream.WriteAsync(clientRequestBytes, 0, clientRequestBytes.Length);
                        networkStream.Flush();
                        var buffer = new byte[8192];
                        var byteCount = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                        var response = Encoding.UTF8.GetString(buffer, 0, byteCount);
                        Console.WriteLine("Client >>Server Response :\t {0}", response);
                    }
                }
            }
        }

    }
}
