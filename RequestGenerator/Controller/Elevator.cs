using System;
using System.Collections.Generic;
using System.Threading;

namespace RequestGenerator.Controller
{
    public class InternalClock
    {
        Timer _timer;
        uint _clock;
        List<SenElevator> _elevs;
        Dictionary<uint, List<Thread>> _waitList;

        private void Wakeup(uint timepoint)
        {
            if (_waitList.ContainsKey(timepoint))
            {
                //List<Thread> pendings = WaitList[timepoint];
                //foreach (Thread t in pendings)
                //    t.Resume();
            }

        }

        public uint Ticks
        {
            get { return _clock; }
        }

        public InternalClock()
        {
            _clock = 0;
        }

        public void Wait(uint timepoint)
        {
            if (_waitList.ContainsKey(timepoint))
            {
                List<Thread> pendings = _waitList[timepoint];
                pendings.Add(Thread.CurrentThread);
            }
            else
            {
                List<Thread> pendings = new List<Thread>();
                pendings.Add(Thread.CurrentThread);
                _waitList.Add(timepoint, pendings);
            }
            //Thread.CurrentThread.Suspend();

        }

        public void ClockUpdator(object obj)
        {
            lock (this)
            {
                Wakeup(_clock);

                _clock++;
                //foreach (SenElevator e in Elevs)
                //e.StatusUpdateForEveryTick();
            }
        }
        public void Start(List<SenElevator> elevs)
        {
            TimerCallback Updator = new TimerCallback(ClockUpdator);
            _timer = new Timer(Updator, null, 0, 1);
            _waitList = new Dictionary<uint, List<Thread>>();
            this._elevs = elevs;
        }
    }

    public class SenElevator : IElevator
    {
        public static InternalClock SenClock = new InternalClock();

        List<uint> _DisabledFloors;
        int _FloorHeight;
        ElevatorStatus _Status;
        const int _MaxSpeed = 2;
        int _target;
        int _id;
        int _capability;
        int _freeCapability;

        event EventHandler _StopEventHandler;

        public int FreeCapability
        {
            get { return _freeCapability; }
        }

        public void SubCapability(int weight)
        {
            _freeCapability -= weight;
        }

        // Added by Xiaobin
        public void AddCapability(int weight)
        {
            _freeCapability += weight;
            Console.WriteLine("Elev{0}: freeCapability is {1}", _id, _freeCapability);
        }

        private void ResetDoorCloseOpenTickCountOnce()
        {
            if (_Status.DoorCloseOpenTicks == -1)
            {
                _Status.DoorCloseOpenTicks = 5;
                _Status.CurrentDirection = Direction.No;
            }

        }

        public void StatusUpdateForEveryTick(int ticks)
        {
            if (_Status.DoorCloseOpenTicks == -1) // not closing or opening
            {
                switch (_Status.CurrentDirection)
                {
                    case Direction.Down:
                        _Status.CurrentHeight -= _MaxSpeed;
                        Console.WriteLine("[Elevator]#{0}:{1} D", _id, _Status.CurrentHeight);
                        break;
                    case Direction.Up:
                        _Status.CurrentHeight += _MaxSpeed;
                        Console.WriteLine("[Elevator]#{0}:{1} U", _id, _Status.CurrentHeight);
                        break;
                }
            }

            _Status.CurrentFloor = _Status.CurrentHeight / _FloorHeight;

            if (_Status.CurrentHeight == _target * this._FloorHeight || _Status.DoorCloseOpenTicks >= 0)
            {
                ResetDoorCloseOpenTickCountOnce();
                Console.WriteLine("[Elevator]  #{0} stopped at floor {1}, {2} ticks left before door closing",
                    _id, _Status.CurrentFloor, _Status.DoorCloseOpenTicks);

                EventArgs e = new ElevStoppedEventArgs(_Status.CurrentFloor, ticks, Scheduler);

                _Status.DoorCloseOpenTicks--;
                if (_StopEventHandler != null)
                    _StopEventHandler(this, e);
            }
        }

        public SenElevator(List<uint> disabledFloors,
            int id,
            int capability,
            int floorheight,
            int highestFloor,
            int initHeight)
        {
            _DisabledFloors = disabledFloors;
            _FloorHeight = floorheight;


            HighestFloor = highestFloor;
            _Status.CurrentDirection = Direction.No;
            _Status.CurrentFloor = initHeight / floorheight;
            _Status.CurrentHeight = initHeight;
            _Status.DoorCloseOpenTicks = -1;
            _target = _Status.CurrentFloor;
            _id = id;
            _freeCapability = _capability = capability;

            HistoryDirection = Direction.No;

        }

        public IEnumerable<uint> DisabledFloors
        {
            get
            {
                List<uint> a = new List<uint>();
                return a;
            }
        }

        public void RegStopHandler(EventHandler handler)
        {
            _StopEventHandler += handler;
        }

        public void UnregStopHandler(EventHandler handler)
        {
            _StopEventHandler -= handler;
        }

        public bool ReqStopAt(int targetFloor)
        {
            if (TargetCheck(targetFloor))
            {
                _target = targetFloor;
                if (CurrentStatus.CurrentHeight / 10.0 > _target)
                {
                    _Status.CurrentDirection = Direction.Down;
                }
                else
                {
                    _Status.CurrentDirection = Direction.Up;
                }
                Console.WriteLine(
                        "[Elevator] accepted scheduler command: #{0} GO to floor {1}, my direction {2}, current floor {3}",
                        _id,
                        targetFloor,
                        _Status.CurrentDirection,
                        _Status.CurrentFloor);
                return true;
            }
            else
            {
                Console.Write(
                    "[Elevator] rejected scheduler command: #{0} GO to floor {1}, my direction {2}, current height{3}",
                    _id,
                    targetFloor,
                    _Status.CurrentDirection,
                    _Status.CurrentHeight);
            }
            return false;
        }

        public bool TargetCheck(int targetFloor)
        {
            bool result = false;
            switch (_Status.CurrentDirection)
            {
                case Direction.No:
                    if (targetFloor != _Status.CurrentFloor)
                        result = true;
                    break;
                case Direction.Up:
                    if (targetFloor * _FloorHeight > _Status.CurrentHeight + this.ControlBufferSpace) //
                        result = true;
                    break;
                case Direction.Down:
                    if (targetFloor * _FloorHeight < _Status.CurrentHeight - this.ControlBufferSpace) //
                        result = true;
                    break;
            }

            return result;
        }

        public ElevatorStatus CurrentStatus => _Status;

        public uint Clock => SenElevator.SenClock.Ticks;

        public int ControlBufferSpace => 5;

        public int HighestFloor { get; }

        public int FloorHeight => _FloorHeight;

        public IScheduler Scheduler { private get; set; }

        public int ID => _id;
        public Direction HistoryDirection { get; set; }

        public bool IsIdle => CurrentStatus.CurrentDirection == Direction.No
                              || CurrentStatus.DoorCloseOpenTicks >= 0;
    }
}
