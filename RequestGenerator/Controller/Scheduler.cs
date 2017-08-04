using System.Collections.Generic;

namespace RequestGenerator.Controller
{
    public static class SchedulerFactory
    {
        public static IScheduler CreateScheduler()
        {
            return new NaiveScheduler();
        }
    }

    public class NaiveScheduler : IScheduler
    {
        private Queue<IRequest> _passengerQueue;
        private List<IElevator> _elevators;

        public void Initialize(List<IElevator> Elevators)
        {
            _passengerQueue = new Queue<IRequest>();
            _elevators = Elevators;
            return;
        }

        public bool QueueReq(IRequest req)
        {
            lock (_passengerQueue)
            {
                _passengerQueue.Enqueue(req);
            }
            return true;
        }

        bool IsAtTopFloor(IElevator elev)
        {
            ElevatorStatus status = elev.CurrentStatus;
            return elev.CurrentStatus.CurrentHeight >= elev.HighestFloor * elev.FloorHeight;
        }

        bool IsAtBottomFloor(IElevator elev)
        {
            ElevatorStatus status = elev.CurrentStatus;
            return elev.CurrentStatus.CurrentHeight == 0;
        }

        private void StopAtEachFloor(IElevator elev)
        {
            ElevatorStatus status = elev.CurrentStatus;

            if (status.CurrentDirection != Direction.No)
                return;
            if (IsAtTopFloor(elev))
            {
                elev.ReqStopAt(status.CurrentFloor - 1);
                elev.HistoryDirection = Direction.Down;
            }
            else
            {
                if (IsAtBottomFloor(elev))
                {
                    elev.ReqStopAt(status.CurrentFloor + 1);
                    elev.HistoryDirection = Direction.Up;
                }
                else
                {
                    switch (elev.HistoryDirection)
                    {
                        case Direction.No:
                        case Direction.Up:
                            elev.ReqStopAt(status.CurrentFloor + 1);
                            elev.HistoryDirection = Direction.Up;
                            break;
                        case Direction.Down:
                            elev.ReqStopAt(status.CurrentFloor - 1);
                            elev.HistoryDirection = Direction.Down;
                            break;
                    }
                }
            }
            
        }

        public void Run()  //scan the request and make correct decision to control elevator;
        {
            foreach (IElevator elev in _elevators)
            {
                StopAtEachFloor(elev);
            }
            return;
        }
    }
}
