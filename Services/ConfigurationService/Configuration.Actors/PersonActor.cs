using System;
using Configuration.Actors.Contract;

namespace Configuration.Actors
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    using Newtonsoft.Json;

    public class PersonActor : Actor, IPersonActor
    {
        public async Task<List<Person>> GetPersons()
        {
            List<Person> result = await Client.GetStateAsync<List<Person>>(StoreName, PersonsKey);
            if (result == null)
                result = new List<Person>();
            return result;
        }

        public async Task<Person> GetPerson(string Name) =>
            (await GetPersons()).Find(p => p.Name == Name);

        public async Task Save(Person person, bool shouldPublishEvent)
        {
            var persons = await GetPersons();
            var oldPerson = persons.Find(p => p.Name == person.Name);
            if (oldPerson == null)
            {
                persons.Add(person);
                await Client.SaveStateAsync(StoreName, PersonsKey, persons);
                Console.WriteLine(32);
                return;
            }

            var updatedList = persons.Where(p => p.Name != person.Name)
                .ToList();
            updatedList.Add(person);
            Console.WriteLine(39);
            var oldMeasurements = oldPerson.CardiovascularSystem.Measurements.ToList();
            var newMeasurements = person.CardiovascularSystem.Measurements.ToList();
            var measurementsToPublish = newMeasurements.Where(m => !oldMeasurements.Any(o => o.DateTime == m.DateTime));
            Console.WriteLine(measurementsToPublish.Count());

            await Client.SaveStateAsync(StoreName, PersonsKey, updatedList);

            var types = measurementsToPublish.Select(m => m.Type).ToList().Distinct();

            foreach (var type in types)
            {
                List<Measure> measuresOfType;
                switch (type)
                {
                    case MeasureType.Pulse:
                        measuresOfType = measurementsToPublish.Where(m => m.Type == MeasureType.Pulse).ToList();
                        break;
                    case MeasureType.Pressure:
                        measuresOfType = measurementsToPublish.Where(m => m.Type == MeasureType.Pressure).ToList();
                        break;
                    default:
                        measuresOfType = new List<Measure>();
                        break;
                }

                if (measuresOfType.Any())
                {
                    var measure = measuresOfType.FirstOrDefault();
                    await Client.PublishEventAsync("pubsub", $"{type}Event", new EventRequest()
                    {
                        Name = $"{type}Event",
                        Arg = JsonConvert.SerializeObject(measure)
                    });
                }
            }
        }

        private DaprClient Client { get; }
        private readonly string StoreName = "statestore";

        private string PersonsKey { get; } = $"persons_key2";

        public PersonActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            Client = daprClient;
        }
    }
}
