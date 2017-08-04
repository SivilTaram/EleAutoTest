using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RequestGenerator.Controller;
using RequestGenerator.Encrypt;

namespace RequestGenerator
{
    class Program
    {
        public static SimulateEngine SimulateEngine;
        public static EncrypLog HistoryLog;
        static void Main(string[] args)
        {
            StartListener();
            while (true)
            {
                Thread.Sleep(10000);
            }
        }

        private static void StartListener()
        {
            int port = 8989;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            var tcpListener = new TcpListener(localAddr, port);
            Console.WriteLine($"RequestGenerator >> Start Listening in {tcpListener.LocalEndpoint}");
            tcpListener.Start();
            var buffer = new byte[8192];
            var tcpClient = tcpListener.AcceptTcpClient();
            Console.WriteLine("RequestGenerator >> Your elevator connected successfully!");
            int lastTick = 0;
            using (var networkStream = tcpClient.GetStream())
            {
                while (true)
                {
                    while (!networkStream.DataAvailable)
                    {
                        Thread.Sleep(100);
                    }
                    //Ready to read
                    int byteCount = buffer.Length;
                    StringBuilder builder = new StringBuilder();
                    while (byteCount == buffer.Length)
                    {
                        byteCount = networkStream.Read(buffer, 0, buffer.Length);
                        builder.Append(Encoding.UTF8.GetString(buffer, 0, byteCount));
                        Array.Clear(buffer, 0, buffer.Length);
                    }
                    var request = builder.ToString();
                    //Get complete request
                    Console.WriteLine("RequestGenerator >> Receive Data : {0}", request);
                    //Parse request, reply to Elevator
                    var serverResponse = GenerateRequests(request, ref lastTick);
                    var serverResponseBytes = Encoding.UTF8.GetBytes(serverResponse);
                    networkStream.Write(serverResponseBytes, 0, serverResponseBytes.Length);
                    networkStream.Flush();
                    Console.WriteLine($"RequestGenerator >> Response : {serverResponse}");
                    Array.Clear(buffer, 0, buffer.Length);
                }
            }
        }

        private static string GenerateRequests(string request, ref int lastTick)
        {
            try
            {
                var operation = (string) JObject.Parse(request)["Operation"];
                switch (operation)
                {
                    case Operation.Config:
                        //alloc a new engine
                        ConfigData config = JsonConvert.DeserializeObject<ConfigData>(request);
                        SimulateEngine = new SimulateEngine();
                        SimulateEngine.Init(config.Elevators, config.TaskID);
                        HistoryLog = new EncrypLog
                        {
                            User = config.User,
                            TaskID = config.TaskID,
                            ElevatorLogs = new List<ElevatorLog>()
                        };
                        return "Config Success";
                    case Operation.GetRequests:
                        RequestPostData reqs = JsonConvert.DeserializeObject<RequestPostData>(request);
                        int lower = lastTick;
                        //Receive reqs data, log into the dictionary.
                        int upper = reqs.Tick;
                        if (reqs.Tick == -1)
                        {
                            upper = int.MaxValue;
                        }
                        if (reqs.FinishRequests != null && reqs.FinishRequests.Count != 0)
                        {
                            HistoryLog.ElevatorLogs.AddRange(
                                reqs.FinishRequests.Where(p => p.FinishTime >= lower && p.FinishTime <= upper));
                        }
                        lastTick = reqs.Tick;
                        if (reqs.Tick != -1)
                        {
                            //Use current tick to request data
                            var currentData = SimulateEngine.GetPassengers(reqs.Tick);
                            var retData = new RequestResponeData
                            {
                                Passengers = currentData.Item2,
                                NextTick = currentData.Item1
                            };
                            return JsonConvert.SerializeObject(retData);
                        }
                        else
                            //Need a judge result
                        {
                            //Judge the result
                            HistoryLog.Score = SimulateEngine.GetScore(HistoryLog.ElevatorLogs);
                            //Log the result
                            string plainLog =
                                JsonConvert.SerializeObject(SimulateEngine.GetEncryScore(HistoryLog.Score,
                                    HistoryLog.User));
                            //Encryp the log
                            var encryScore = EncryptorRSA.EncryptText(plainLog, GenratedKeys.PublicKey);
                            //log to the file
                            StreamWriter writer = new StreamWriter($"test-{HistoryLog.TaskID}.log");
                            writer.WriteLine(encryScore);
                            writer.Close();
                            return JsonConvert.SerializeObject(HistoryLog.Score);
                        }
                    default:
                        return "Wrong Parameters, Please check you json data.";
                }
            }
            catch (OwnExcepetion e)
            {
                return e.Message;
            }
            catch (Exception e)
            {
                return "Bad Arguments!";
            }
        }
    }


    public static class Operation
    {
        public const string Config = "CONFIG";
        public const string GetRequests = "GETREQS";
    }

    public class ConfigData
    {
        public string User { get; set; }
        public List<ElevatorConf> Elevators { get; set; }
        public string TaskID { get; set; }
        public string Operation { get; set; }
    }

    public class ElevatorConf
    {
        public int ID { get; set; }
        public int Capability { get; set; }
        public int FloorMax { get; set; }
        public int FloorHeight { get; set; }
        public int InitHeight { get; set; }
    }

    public class RequestPostData
    {
        public int Tick { get; set; }
        public List<ElevatorLog> FinishRequests { get; set; }
        public string Operation { get; set; }
    }

    public class ElevatorLog
    {
        public string PassengerName { get; set; }
        public int FinishTime { get; set; }
        public int ElevatorID { get; set; }
    }
    public class RequestResponeData
    {
        public List<string> Passengers { get; set; }
        public int NextTick { get; set; }
    }

    public class EncrypLog
    {
        public string User { get; set; }
        public List<ElevatorLog> ElevatorLogs { get; set; }
        public string TaskID { get; set; }
        public JudgeScore Score { get; set; }
    }

    public class JudgeScore
    {
        public bool Correct { get; set; }
        public double Cost { get; set; }
        public double Standard { get; set; }
    }

    public class EncryScore
    {
        [JsonProperty("u")]
        public string User { get; set; }
        [JsonProperty("c")]
        public int CorrectScore { get; set; }
        [JsonProperty("p")]
        public int PerfScore { get; set; }
    }
}
