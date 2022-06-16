using System;
using System.Threading.Tasks;

namespace WorkerManager.Actors.Contract
{
    public interface IWorker
    {
        public Task Run(Guid workerId, string[] args, Func<WorkerData, Task> setWorkerData = null,
            Func<Task<WorkerData>> getWorkerData = null, Func<Task<bool>> shouldStop = null,
            Func<Task> stop = null);
    }
}
