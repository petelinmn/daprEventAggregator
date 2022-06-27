using System;
using System.Collections.Generic;
using System.Linq;
using WorkerManager.Actors.Contract;

namespace WorkerManager.Actors
{
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    
    public class WorkerManagerActor : Actor, IWorkerManagerActor
    {
        public async Task<Guid> Register(string Name, string[] args, Guid? contextId, Guid[] parents)
        {
            var worker = new WorkerInfo
            {
                Id = Guid.NewGuid(),
                Status = WorkerStatus.Init,
                Args = args ?? new string[] { },
                ContextId = contextId.Value,
                DateTime = DateTime.Now,
                Parents = parents,
                Name = Name
            };

            await SaveWorker(worker);
            await AddWorker(worker.Id);

            return worker.Id;
        }

        public async Task<WorkerArgs> StartNext(string[] implementations)
        {
            var nextWorkerId = await GetWorkerFromQueue();

            if (!nextWorkerId.HasValue)
                return null;

            var worker = await GetWorker(nextWorkerId.Value);

            if (worker == null)
                return null;

            if (implementations.Contains(worker.Name))
            {
                worker.Status = WorkerStatus.Work;

                await SaveWorker(worker);

                return new WorkerArgs
                {
                    WorkerId = worker.Id,
                    Args = worker.Args
                };
            }

            return null;

            //new event OnStart Worker
        }

        public async Task<WorkerStatus> GetStatus(Guid id)
        {
            var worker = await GetWorker(id);
            return worker.Status;
        }
        
        public async Task Stop(Guid id)
        {
            var worker = await GetWorker(id);
            
            worker.Status = WorkerStatus.Stop;
            
            await SaveWorker(worker);
            
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var workers = await Client.GetStateAsync<Dictionary<Guid, WorkerStatus>>(StoreName, key) ??
                          new Dictionary<Guid, WorkerStatus>();

            workers[id] = WorkerStatus.Stop;
            
            await Client.SaveStateAsync(StoreName, key, workers);

            //new event OnStop Worker
        }

        public async Task SetWorkerData(Guid id, WorkerData data)
        {
            var worker = await GetWorker(id);

            worker.Data = data;

            await SaveWorker(worker);
        }

        public async Task<WorkerData> GetWorkerData(Guid id)
        {
            var worker = await GetWorker(id);
            return worker.Data ?? new WorkerData();
        }

        public async Task<List<WorkerInfoSlim>> GetWorkersByStatus()
        {
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var workers = await Client.GetStateAsync<Dictionary<Guid, WorkerStatus>>(StoreName, key) ??
                          new Dictionary<Guid, WorkerStatus>();

            return workers.Select(i => new WorkerInfoSlim
            {
                Id = i.Key,
                Status = i.Value
            }).ToList();
        }
        
        private string GetWorkerStateKey(string key) => $"worker_{key}";
        private string QueueToWorkStateKey { get; } = $"queueToWorkStateKey";
        private readonly string StoreName = "statestore";
        private Guid WorkerId { get; }
        private WorkerStatus Status { get; set; }
        private DaprClient Client { get; }

        public async Task<WorkerInfo> GetWorker(Guid id) =>
            await Client.GetStateAsync<WorkerInfo>(StoreName, GetWorkerStateKey(id.ToString()))
                         ?? throw new Exception("Worker not found");

        private async Task SaveWorker(WorkerInfo worker) =>
            await Client.SaveStateAsync(StoreName, GetWorkerStateKey(worker.Id.ToString()), worker);
        
        private async Task AddWorker(Guid id)
        {
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var workers =
                await Client.GetStateAsync<Dictionary<Guid, WorkerStatus>>(StoreName, key) ??
                        new Dictionary<Guid, WorkerStatus>();
            
            workers.Add(id, WorkerStatus.Init);

            await Client.SaveStateAsync(StoreName, key, workers);
        }
        
        private async Task<Guid?> GetWorkerFromQueue()
        {
            try
            {
                var key = GetWorkerStateKey(QueueToWorkStateKey);
                var workers = await Client.GetStateAsync<Dictionary<Guid, WorkerStatus>>(StoreName, key) ??
                              new Dictionary<Guid, WorkerStatus>();

                if (workers == null || !workers.ContainsValue(WorkerStatus.Init))
                    return null;

                var workerId = workers.FirstOrDefault(i =>
                    i.Value == WorkerStatus.Init).Key;
                workers[workerId] = WorkerStatus.Work;
                await Client.SaveStateAsync(StoreName, key, workers);

                return workerId;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }

        public async Task<List<WorkerInfo>> GetWorkersByContext(Guid contextId)
        {
            var key = GetWorkerStateKey(QueueToWorkStateKey);
            var workers = await Client.GetStateAsync<Dictionary<Guid, WorkerStatus>>(StoreName, key) ??
                          new Dictionary<Guid, WorkerStatus>();

            var result = new List<WorkerInfo>();
            foreach (var workerInfo in workers)
            {
                var worker = await GetWorker(workerInfo.Key);
                if (worker.ContextId == contextId)
                {
                    result.Add(worker);
                }
            }

            return result;
        }

        public WorkerManagerActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            WorkerId = Guid.NewGuid();
            Status = WorkerStatus.Init;
            Client = daprClient;
        }
    }
}
