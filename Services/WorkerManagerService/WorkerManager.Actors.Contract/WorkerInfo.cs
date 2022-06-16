using Common;
using System;

namespace WorkerManager.Actors.Contract
{
    public class WorkerInfo : BaseEntity
    {
        public Guid? ContextId { get; set; }
        public DateTime? StartTime { get; set; }
        public string[] Args { get; set; }
        public WorkerStatus Status { get; set; }
        public string Tag { get; set; }
        public string SourceName { get; set; }
        public WorkerData Data { get; set; }

        public override string ToString() => $"{StartTime?.ToString("HH:mm:ss")} - {Args[0]}, {Status}";
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
