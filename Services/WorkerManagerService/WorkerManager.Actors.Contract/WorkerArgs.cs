using System;

namespace WorkerManager.Actors.Contract
{
    public class WorkerArgs
    {
        public Guid? WorkerId { get; set; }
        public string[] Args { get; set; }
    }
}
