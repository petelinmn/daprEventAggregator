using System;
using Dapr.Actors;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WorkerManager.Actors.Contract
{
    public interface IWorkerManagerActor : IActor
    {
        Task<Guid> Register(string Name, string[] args, Guid? contextId, Guid[] parents);
        Task<WorkerArgs> StartNext(string[] implementations);
        Task<WorkerStatus> GetStatus(Guid id);
        Task Stop(Guid id);
        Task SetWorkerData(Guid id, WorkerData data);
        Task<WorkerData> GetWorkerData(Guid id);
        Task<List<WorkerInfoSlim>> GetWorkersByStatus();
        Task<WorkerInfo> GetWorker(Guid id);
        Task<List<WorkerInfo>> GetWorkersByContext(Guid contextId);
    }
}
