using System;
using Configuration.Actors.Contract;

namespace Configuration.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    using Newtonsoft.Json;

    public class ConfigurationActor : Actor, IConfigurationActor
    {
        public async Task<string> Get(string key) =>
            await Client.GetStateAsync<string>(StoreName, GetAppConfigurationKey(key));
        
        public async Task Set(string key, string value) =>
            await Client.SaveStateAsync(StoreName, GetAppConfigurationKey(key), value);
        
        public async Task<AppConfiguration> GetAppConfiguration(string key) =>
            await Client.GetStateAsync<AppConfiguration>(StoreName, GetAppConfigurationKey(key));
        
        public async Task SetAppConfiguration(string key, AppConfiguration value) =>
            await Client.SaveStateAsync(StoreName, GetAppConfigurationKey(key), value);
        private DaprClient Client { get; }
        private readonly string StoreName = "statestore";

        private string GetAppConfigurationKey(string key) => $"appConfiguration_{key}";

        private string StereotypesConfigurationKey { get; set; } = $"stereotypesConfiguration";

        public ConfigurationActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            Client = daprClient;
        }
    }
}
