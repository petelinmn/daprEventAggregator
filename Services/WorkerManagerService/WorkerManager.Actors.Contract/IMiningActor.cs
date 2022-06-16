using System;
using Dapr.Actors;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace WorkerManager.Actors.Contract
{
    public interface IMiningActor : IActor
    {
        Task Mine(string wallet);
        Task<double> GetBalance(string wallet);
    }
}
