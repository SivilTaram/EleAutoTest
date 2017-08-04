using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequestGenerator.Controller
{
    public class PassengerReq : IRequest
    {
        readonly RequestType _type;
        readonly int _reqtime;
        readonly int _directionReqSrc;
        readonly Direction _direction;
        readonly int _destinationReqDst;
        IElevator _inElevator;

        public IElevator ElevatorReqIn { get { return _inElevator; } }
        public PassengerReq(RequestType type, int time, int floor, Direction direction)
        {
            _type = type;
            _reqtime = time;
            if (type == RequestType.DirectionReq)
            {
                _directionReqSrc = floor;
                _direction = direction;
            }
            else
            {
                _destinationReqDst = floor;
            }
            _inElevator = null;
        }

        public void SetReqInElevator(IElevator elevator)
        {
            _inElevator = elevator;
        }

        public RequestType Type { get { return _type; } }
        public int ReqTime { get { return _reqtime; } }
        public int DirectionReqSource { get { return _directionReqSrc; } }
        public Direction UpDown { get { return _direction; } }
        public int DestinationReqDest { get { return _destinationReqDst; } }
    }

    public class SigmaPassenger : IPassenger  //provided by teacher
    {
        private readonly int _directionReqTime;
        private int _targetReqTime;
        private int _reqCompleteTime;
        private readonly int _sourceFloor;
        private readonly int _targetFloor;
        private bool _isInside;
        private bool _isArrived;
        private IElevator _myElev;
        private readonly int _weight;
        private readonly string _name;

        public bool IsArrived { get { return _isArrived; } }
        public bool IsInside { get { return _isInside; } }

        public SigmaPassenger(
            string name,
            int directionReqTime,
            int sourceFloor,
            int targetFloor,
            int weight
            )
        {
            _directionReqTime = directionReqTime;
            _sourceFloor = sourceFloor;
            _targetFloor = targetFloor;
            _isInside = false;
            _isArrived = false;
            _targetReqTime = 0;
            _reqCompleteTime = 0;
            _myElev = null;
            _name = name;
            _weight = weight;
        }
        public int Weight => _weight;
        public string Name => _name;
        public int DirectionReqTime => _directionReqTime;
        public int TargetReqTime => _targetReqTime;
        public int ReqCompleteTime => _reqCompleteTime;
        public int SourceFloor => _sourceFloor;
        public int TargetFloor => _targetFloor;

        private void RegElevatorStopNotify(List<IElevator> elevs)
        {
            foreach (IElevator elev in elevs)
            {
                elev.RegStopHandler(StopAlarmHandler);
            }
        }

        public IRequest GenerateReq(int nowTicks, List<IElevator> elevs)
        {
            if (!_isInside)
            {
                if (nowTicks == _directionReqTime)
                {
                    Console.WriteLine("[Passenger] {0} is pressing, from {1} to {2}, ",
                        _name, _sourceFloor, _targetFloor);
                    IRequest req = new PassengerReq(
                        RequestType.DirectionReq, nowTicks, _sourceFloor, this.UpDown);
                    RegElevatorStopNotify(elevs);
                    return req;
                }
            }
            return null;
        }

        public Direction UpDown
        {
            get
            {
                if (SourceFloor > TargetFloor) return Direction.Down;
                if (SourceFloor == TargetFloor)
                    return Direction.No;
                else
                    return Direction.Up;
            }
        }

        bool CheckElevatorAndPassenger(uint floor, IElevator e)
        {
            ElevatorStatus status = e.CurrentStatus;

            return (status.CurrentDirection == Direction.No) &&
                    (status.CurrentFloor == floor);
        }

        private void EnterElevator(IScheduler scheduler, int tick, IElevator elev)
        {
            SenElevator senElev = elev as SenElevator;
            if (senElev.FreeCapability >= _weight)
            {
                Console.WriteLine("[Passenger] {0} enter elevator {1}, at floor {2}",
                    _name, elev.ID, _sourceFloor);
                _isInside = true;
                _targetReqTime = tick;
                _myElev = elev;
                senElev.SubCapability(_weight);
                PassengerReq req = new PassengerReq(RequestType.DestinationReq, tick, _targetFloor, Direction.No);
                req.SetReqInElevator(senElev);
                scheduler.QueueReq(req);

            }
            else if (senElev.CurrentStatus.DoorCloseOpenTicks < 4) //wait for one tick to try enter again.
            {
                Console.WriteLine("[Passenger] {0} can't get in due to over-weighted, press up/down again", this.Name);
                scheduler.QueueReq(
                    new PassengerReq(RequestType.DirectionReq, tick, this._sourceFloor, this.UpDown));
            }
        }

        private void LeaveElevator(int tick, IElevator elev)
        {
            // Added by Xiaobin
            SenElevator senElev = elev as SenElevator;
            senElev.AddCapability(_weight);

            Console.WriteLine("[Passenger] {0} leave elevator {1}, floor {2}",
                _name, elev.ID, _targetFloor);
            _reqCompleteTime = tick;
            _isArrived = true;
            _isInside = false;
        }

        public void ElevatorStopped(object sender, EventArgs e)
        {
            IElevator elev = (IElevator)sender;
            ElevStoppedEventArgs args = (ElevStoppedEventArgs)e;


            if (args.StoppedFloor == _sourceFloor && !_isInside && UpDown == elev.HistoryDirection && !_isArrived)
            {
                EnterElevator(args.Scheduler, args.Tick, elev);
            }

            if (args.StoppedFloor == _targetFloor && _isInside && _myElev == elev)
            {
                LeaveElevator(args.Tick, elev);
            }

            if (_isArrived)
            {
                elev.UnregStopHandler(StopAlarmHandler);
            }
            //if (CheckElevatorAndPassenger(this.SourceFloor, args.StoppedElevator))
            //{
            //    this._TargetReqTime = Elevator.SenElevator.SenClock.Ticks;
            //    StudentScheduler.QueueTargetFloor(this);
            //}
            //else
            //{
            //    Console.WriteLine("passenger can't enter into elevator");
            //}

            return;
        }


        public EventHandler StopAlarmHandler
        {
            get
            {
                return ElevatorStopped;
            }
        }


        public void PrintStatistics()
        {
            Console.WriteLine("{0}: requesting time {1}, entering time {2}, arrival time {3}, cost {4} ticks",
                Name, _directionReqTime, _targetReqTime, _reqCompleteTime, _reqCompleteTime - _directionReqTime);
            return;
        }
    }
}
