using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestGenerator.Controller
{
    public class OwnExcepetion : Exception
    {
        public OwnExcepetion(string message) : base(message)
        {
            
        }
    }

    public static class Loader
    {
        public static Dictionary<int, List<string>> LoadEnumaterPassengers(string taskId)
        {
            Dictionary<int, List<string>> all = new Dictionary<int, List<string>>();
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"RequestGenerator.Resources.test{taskId}.txt"))
            {
                if (stream == null)
                {
                    throw new OwnExcepetion($"Task ID {taskId} doesn't exist!");
                }
                StreamReader reader = new StreamReader(stream);
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var passParams = line.Split(new[] {','}, 2);
                    int tick = int.Parse(passParams[0]);
                    if (!all.ContainsKey(tick))
                    {
                        all[tick] = new List<string>();
                    }
                    all[tick].Add(passParams[1]);
                }
                reader.Close();
                var dicSort = from objDic in all
                    orderby objDic.Key
                    ascending
                    select objDic;
                all = dicSort.ToDictionary(k => k.Key, o => o.Value);
            }
            return all;
        }
        public static List<SigmaPassenger> LoadPassengers(string taskId)
        {
            List<SigmaPassenger> all = new List<SigmaPassenger>();
            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream($"RequestGenerator.Resources.test{taskId}.txt"))
            {
                if (stream == null)
                {
                    throw new OwnExcepetion($"Task ID {taskId} doesn't exist!");
                }
                StreamReader reader = new StreamReader(stream);
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    var passParams = line.Split(',');
                    SigmaPassenger obj = new SigmaPassenger(
                        //Name
                        passParams[1],
                        //CommingTime
                        int.Parse(passParams[0]),
                        //FromFloor
                        int.Parse(passParams[2]),
                        //ToFloor
                        int.Parse(passParams[3]),
                        //Weight
                        int.Parse(passParams[4])
                    );
                    all.Add(obj);
                }
                reader.Close();
            }
            return all;
        }
        public static List<SenElevator> LoadElevators(List<ElevatorConf> conf)
        {
            List<SenElevator> all = new List<SenElevator>();
            try
            {
                foreach (var ele in conf)
                {
                    SenElevator obj = new SenElevator(null,
                        ele.ID,
                        ele.Capability,
                        ele.FloorHeight,
                        ele.FloorMax,
                        ele.InitHeight
                        );
                    all.Add(obj);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return all;
        }
    }

    public class SimulateEngine
    {
        public static List<SenElevator> AllElevators;
        public static List<SigmaPassenger> AllPassengers; //sorted by DirectionReqTime
        public static Dictionary<int, List<string>> MappingPassengers;
        static IScheduler NaiveScheduler;
        private int _ticks = 0;

        bool AreElevatorsIdle()
        {
            bool ret = true;
            foreach (SenElevator e in AllElevators)
                ret = ret && e.IsIdle;
            return ret;
        }

        public Tuple<int, List<string>> GetPassengers(int tick)
        {
            int nextTick = -1;
            var ticks = MappingPassengers.Keys.ToList();
            if (MappingPassengers.ContainsKey(tick))
            {
                var passengers = MappingPassengers[tick];
                var index = ticks.IndexOf(tick);
                if (index < ticks.Count - 1)
                {
                    nextTick = ticks[index + 1];
                }
                var ret = new Tuple<int, List<string>>(nextTick, passengers);
                return ret;
            }
            return null;
        }

        public void Init(List<ElevatorConf> config, string taskId)
        {
            //Init mapping passengers
            MappingPassengers = Loader.LoadEnumaterPassengers(taskId);
            AllPassengers = Loader.LoadPassengers(taskId);
            AllElevators = Loader.LoadElevators(config);
            //Load all passengers and elevators
            NaiveScheduler = SchedulerFactory.CreateScheduler();
            foreach (SenElevator elev in AllElevators)
            {
                elev.Scheduler = NaiveScheduler;
            }
            List<IElevator> elevs = new List<IElevator>();
            foreach (var e in AllElevators)
            {
                elevs.Add(e);
            }
            NaiveScheduler.Initialize(elevs);
        }

        private void TickUpdate(int nextActiveTick)
        {
            if (AreElevatorsIdle() && nextActiveTick > _ticks)
            {
                _ticks = nextActiveTick;
            }
            else
            {
                _ticks++;
            }
        }

        public Task Simulate()
        {
            return Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        int nextActiveTick = -1;
                        bool complete = DrivePassenger(ref nextActiveTick);
                        if (complete)
                        {
                            break;
                        }
                        DriveElevator();
                        NaiveScheduler.Run();
                        TickUpdate(nextActiveTick);
                    }
                }
            );
        }

        private bool DrivePassenger(ref int nextActiveTick)
        {
            bool complete = true;
            bool active = false;
            int nextTick = 0x7fffffff;
            foreach (SigmaPassenger p in AllPassengers)
            {
                if (!p.IsArrived)
                {
                    complete = false;
                }
                IRequest req = p.GenerateReq(_ticks, AllElevators.ConvertAll<IElevator>(o => o));
                if (req != null)
                {
                    NaiveScheduler.QueueReq(req);
                    active = true;
                }

                if (p.IsInside)
                {
                    active = true;
                }
                else
                {
                    if (!p.IsArrived)
                    {
                        nextTick = Math.Min(nextTick, p.DirectionReqTime);
                    }
                }
            }

            if (!active)
            {
                nextActiveTick = nextTick;
            }
            return complete;
        }

        void DriveElevator()
        {
            foreach (SenElevator elev in AllElevators)
            {
                elev.StatusUpdateForEveryTick(_ticks);
            }
        }

        private Task<float> Start()
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var task = Simulate();
                    task.Wait();
                    int num = 0;
                    int cost = 0;
                    foreach (SigmaPassenger p in AllPassengers)
                    {
                        p.PrintStatistics();
                        cost += p.ReqCompleteTime - p.DirectionReqTime;
                        num++;
                    }
                    var averageCost = (float) cost/num;
                    return averageCost;

                });
        }

        public JudgeScore GetScore(List<ElevatorLog> logs)
        {
            var judgeScore = new JudgeScore();
            var task = Start();
            task.Wait();
            judgeScore.Standard = task.Result;
            //Calculate cost of student
            float costTime = 0;
            foreach (var elevator in AllElevators)
            {
                var curEleLog = logs.Where(o => o.ElevatorID == elevator.ID).ToList();
                if (curEleLog.Count != 0)
                {
                    costTime += curEleLog.Max(o => o.FinishTime);
                }
            }
            judgeScore.Cost = costTime/AllElevators.Count;
            //Get if correct
            judgeScore.Correct = CheckCorrect(logs);
            return judgeScore;
        }

        public bool CheckCorrect(List<ElevatorLog> logs)
        {
            //NotImplemented
            return true;
        }
        public EncryScore GetEncryScore(JudgeScore judge, string user)
        {
            var basicScore = judge.Correct ? 30 : 0;
            //Use speed up to calculate the score
            var perScore = (int)Math.Min(
                Math.Pow(2, judge.Standard / judge.Cost), basicScore);
            var encryScore = new EncryScore
            {
                User = user,
                CorrectScore = basicScore,
                PerfScore = perScore
            };
            return encryScore;
        }
    }
}
