using Common;
using Configuration.Actors.Contract.Model;
using Dapr.Actors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configuration.Actors.Contract
{
    public interface IPersonActor : IActor
    {
        Task<Person> GetPerson(string Name);
        Task<List<Person>> GetPersons();
        Task Save(Person person, bool shouldPublishEvent);
    }
}
