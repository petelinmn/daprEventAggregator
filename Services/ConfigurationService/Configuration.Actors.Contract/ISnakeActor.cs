using Dapr.Actors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Configuration.Actors.Contract
{
    public interface ISnakeActor : IActor
    {
        Task<Snake> GetSnake();
        Task Move(string direction);
    }
}
