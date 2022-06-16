using Dapr.Actors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configuration.Actors.Contract
{
    public interface IConfigurationActor : IActor
    {
        Task<string> Get(string key);
        Task Set(string key, string value);
        Task<AppConfiguration> GetAppConfiguration(string key);
        Task SetAppConfiguration(string key, AppConfiguration value);
    }
}
