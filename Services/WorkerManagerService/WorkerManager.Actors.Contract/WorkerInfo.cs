using Common;
using System;

namespace WorkerManager.Actors.Contract
{
    public class WorkerInfo : BaseEntity
    {
        public string[] Args { get; set; }
        public WorkerStatus Status { get; set; }
        public WorkerData Data { get; set; }

        public override string ToString() => $"{DateTime?.ToString("HH:mm:ss")} - {Name}, {Status}";
    }

    public class WorkerData
    {
        public string Result { get; set; }
    }

    public class WorkerInfoSlim
    {
        public Guid Id { get; set; }
        public WorkerStatus Status { get; set; }
    }
}
