using System;
using System.Threading.Tasks;
using Dapr.Actors.Runtime;
using WorkerManager.Actors.Contract;
using Dapr.Client;

namespace WorkerManager.Actors;

public class MiningActor : Actor, IMiningActor
{
    public async Task Mine(string wallet)
    {
        var amount = Random.Shared.NextDouble();
        var balance = await GetBalance(wallet);
        await Client.SaveStateAsync(StoreName, GetWalletKey(wallet), balance + amount);
    }

    public async Task<double> GetBalance(string wallet) =>
        await Client.GetStateAsync<double>(StoreName, GetWalletKey(wallet));
    
    private string GetWalletKey(string wallet) => $"wallet_{wallet}";
    private readonly string StoreName = "statestore";
    private DaprClient Client { get; }
    
    public MiningActor(ActorHost host, DaprClient daprClient)
        : base(host)
    {
        Client = daprClient;
    }
}