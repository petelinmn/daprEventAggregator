
using System.Globalization;
using Configuration.Actors.Contract;
using Dapr.Actors;
using Dapr.Actors.Client;
using WorkerManager.Actors.Contract;

namespace TestWorker
{
    public class WorkerTask
    {
        public Task? Task { get; set; }
        public Guid WorkerId { get; set; }
    }

    public class TestWorker
    {
        public static async Task Main(string[] args)
        {
            var countWorkers = 2;
            if (args.Length != 0)
                int.TryParse(args[0], out countWorkers);

            var managerActor = ActorProxy.Create<IWorkerManagerActor>(
                new ActorId("WorkerManagerActor"), "WorkerManagerActor");
            List<WorkerTask> tasks = new();
            var msg = "";
            while (true)
            {
                var newMsg = "task Count:" + tasks.Count;
                if (msg != newMsg)
                {
                    msg = newMsg;
                    Console.WriteLine(msg);
                }

                if (tasks.Count < countWorkers)
                {
                    var workerArgs = await managerActor.StartNext(GetImplementations());
                    if (workerArgs?.WorkerId.HasValue == true)
                    {
                        Guid workerId = workerArgs.WorkerId.Value;
                        var arguments = workerArgs.Args;

                        async Task<bool> ShouldStop() =>
                            (await managerActor.GetStatus(workerId)) == WorkerStatus.Stop;

                        async Task SetWorkerData(WorkerData data) =>
                            await managerActor.SetWorkerData(workerId, data);

                        async Task<WorkerData> GetWorkerData() =>
                            await managerActor.GetWorkerData(workerId);

                        async Task Stop()
                        {
                            Console.WriteLine($"Stop worker Id:{workerId}");
                            await managerActor.Stop(workerId);
                        }

                        var worker = GetWorker(arguments?.FirstOrDefault());
                        if (worker != null)
                        {
                            tasks.Add(new WorkerTask
                            {
                                WorkerId = workerId,
                                Task = Task.Run(() => worker.Run(workerId, arguments,
                                        SetWorkerData, GetWorkerData, ShouldStop, Stop))
                                    .ContinueWith(async _ =>
                                    {
                                        tasks = tasks.Where(task => task.WorkerId != workerId).ToList();
                                        await managerActor.Stop(workerId);
                                    })
                            });
                        }
                    }
                }

                await Task.Delay(1000);
            }
        }

        private static string[] GetImplementations() =>
            new string[] { "PI", "MiningEmulator", "Coffee", "Walking", "TakePills", "Sleep" };

        public static IWorker? GetWorker(string? workerName = null)
        {
            switch (workerName)
            {
                case "PI":
                    return new PiCalculateWorker();
                case "MiningEmulator":
                    return new MiningEmulatorWorker();
                case "Coffee":
                    return new CureWorker(workerName);
                case "Walking":
                    return new CureWorker(workerName);
                case "TakePills":
                    return new CureWorker(workerName);
                case "Sleep":
                    return new CureWorker(workerName);
                default:
                    return null;
            }
        }
    }

    public class CureWorker : IWorker
    {
        string Name { get; }

        public CureWorker(string name)
        {
            Name = name;
        }
        public async Task Run(Guid workerId, string[] args,
            Func<WorkerData, Task> setWorkerData,
            Func<Task<WorkerData>> getWorkerData,
            Func<Task<bool>> shouldStop, Func<Task> stop)
        {
            Console.WriteLine($"Start worker Name:{Name},\tId: {workerId}");
            var countOfCycles = 10;
            for (var i = 1; i <= countOfCycles; i++)
            {
                try
                {
                    if (await shouldStop?.Invoke()!)
                        break;

                    var data = await getWorkerData?.Invoke()!;
                    Console.WriteLine($"Worker: {Name}\tprogress: {(i+1)*10}%");
                    await Task.Delay(3000);
                    data.Result = i.ToString();
                    await setWorkerData?.Invoke(data)!;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            Console.WriteLine($"End worker:\t Name: {Name},\tId: {workerId}");
            await stop();
        }
    }

    public class DefaultWorker : IWorker
    {
        public async Task Run(Guid workerId, string[] args,
            Func<WorkerData, Task> setWorkerData,
            Func<Task<WorkerData>> getWorkerData, 
            Func<Task<bool>> shouldStop, Func<Task> stop)
        {
            Console.WriteLine($"Start_{workerId}");
            Console.WriteLine("Args: " + string.Join("", args));

            var countOfCycles = 100;

            if (args.Length > 1)
                int.TryParse(args[1], out countOfCycles);

            for (var i = 0; i < countOfCycles; i++)
            {
                try
                {
                    if (await shouldStop?.Invoke()!)
                        break;
                    
                    var data = await getWorkerData?.Invoke()!;
                    Console.WriteLine($"Task___{i}___{workerId}__status:{data.Result}");
                    await Task.Delay(1000);
                    data.Result = i.ToString();
                    await setWorkerData?.Invoke(data)!;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + "!" + e.StackTrace);
                }
            }
            
            Console.WriteLine($"End_{workerId}");
            await stop();
        }
    }

    public class PiCalculateWorker : IWorker
    {
        public async Task Run(Guid workerId, string[] args, 
            Func<WorkerData, Task> setWorkerData,
            Func<Task<WorkerData>> getWorkerData, 
            Func<Task<bool>> shouldStop,
            Func<Task> stop)
        {
            Console.WriteLine("start pi calculate");
            
            if (args.Length < 2 || !int.TryParse(args[1], out var cycleCount))
            {
                Console.WriteLine("Count of cycles should be pointed as second parameter");
                throw new Exception("Count of cycles should be pointed as second parameter");
            }

            var foo = 0;
            double pi = 0;
            for (var bar = 0; bar < cycleCount; bar++)
            {
                var x = Random.Shared.NextDouble();
                var y = Random.Shared.NextDouble();
                if (x * x + y * y < 1)
                    foo++;

                pi = 4 * (double)foo / bar;
            }
            
            Console.WriteLine($"PI: {pi}");
            await setWorkerData?.Invoke(new WorkerData
            {
                Result = pi.ToString(CultureInfo.InvariantCulture)
            })!;
            await stop();
        }
    }
    
    public class MiningEmulatorWorker : IWorker
    {
        public async Task Run(Guid workerId, string[] args, 
            Func<WorkerData, Task> setWorkerData,
            Func<Task<WorkerData>> getWorkerData, 
            Func<Task<bool>> shouldStop,
            Func<Task> stop)
        {
            Console.WriteLine("start mining");
            
            if (args.Length < 2)
            {
                Console.WriteLine("Wallet number should be pointed as second parameter");
                throw new Exception("Wallet number should be pointed as second parameter");
            }
            
            var walletNumber = args[1];

            var miningActor = ActorProxy.Create<IMiningActor>(
                new ActorId("MiningActor"), "MiningActor");

            long i = 0;
            while (true)
            {
                await miningActor.Mine(walletNumber);

                if (i % 100 == 0)
                {
                    var balance = await miningActor.GetBalance(walletNumber);
                    Console.WriteLine(balance);
                    
                    await setWorkerData?.Invoke(new WorkerData
                    {
                        Result = balance.ToString(CultureInfo.InvariantCulture)
                    })!;
                    
                    if (await shouldStop())
                        break;
                }
                
                i++;
            }
        }
    }
}
