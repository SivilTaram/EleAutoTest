using System;
using System.Collections.Generic;

namespace RequestGenerator
{
    public enum Direction
    {
        No,
        Up,
        Down
    }

    public struct ElevatorStatus
    {
        public int CurrentFloor;
        public Direction CurrentDirection;
        public int CurrentHeight;
        public int DoorCloseOpenTicks;
    }

    public enum RequestType
    {
        DirectionReq = 0,
        DestinationReq = 1,
    }

    public interface IRequest
    {
        RequestType Type { get; }
        int ReqTime { get; }
        int DirectionReqSource { get; }
        int DestinationReqDest { get; }
        Direction UpDown { get; }
        IElevator ElevatorReqIn { get; }
    }

    public interface IElevator
    {
        int HighestFloor { get; }
        int FloorHeight { get; }
        void RegStopHandler(EventHandler handler);
        void UnregStopHandler(EventHandler handler);
        uint Clock { get; }
        int ID { get; }

        bool ReqStopAt(int targetFloor);
        bool TargetCheck(int targetFloor);
        ElevatorStatus CurrentStatus { get; }
        Direction HistoryDirection { get; set; }
        int ControlBufferSpace { get; }
        int FreeCapability { get; }
        //only when elevator is stopped, 
        //or current target is in the middle of last target and target floor is not disabled, true returned
    }

    public interface IPassenger
    {
        //for student evaluation
        int DirectionReqTime { get; }
        int TargetReqTime { get; }
        int ReqCompleteTime { get; }

        int SourceFloor { get; }
        int TargetFloor { get; }
        Direction UpDown { get; }

        EventHandler StopAlarmHandler { get; }
    }

    public interface IScheduler
    {
        void Initialize(List<IElevator> Elevators);
        bool QueueReq(IRequest req);

        void Run();
    }

    public class AlarmEventArgs : EventArgs
    {
        public IElevator StoppedElevator;

        public AlarmEventArgs(IElevator e)
        {
            StoppedElevator = e;
        }

    }

    public class ElevStoppedEventArgs : EventArgs
    {
        public int StoppedFloor;
        public int Tick;
        public IScheduler Scheduler;
        public ElevStoppedEventArgs(int floor, int tick, IScheduler scheduler)
        {
            StoppedFloor = floor;
            Tick = tick;
            Scheduler = scheduler;
        }
    }
}
