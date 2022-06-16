using System;
using Configuration.Actors.Contract;

namespace Configuration.Actors
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Dapr.Actors.Runtime;
    using Dapr.Client;
    using Newtonsoft.Json;

    public class SnakeActor : Actor, ISnakeActor
    {
        public async Task<Snake> GetSnake() =>
            (await Client.GetStateAsync<Snake>(StoreName, SnakeKey)) ?? new Snake
            {
                Body = new List<SnakePoint>
                { 
                    new SnakePoint { X = 5, Y = 5 }
                }
            };

        public async Task SetSnake(Snake snake) =>
            await Client.SaveStateAsync(StoreName, SnakeKey, snake);

        public async Task Move(string direction)
        {
            var snake = await GetSnake();

            var previousPoint = snake.Body[snake.Body.Count - 1];
            var deltaX = 0;
            var deltaY = 0;
            switch (direction)
            {
                case "up":
                    deltaY -= 1;
                    break;
                case "down":
                    deltaY += 1;
                    break;
                case "left":
                    deltaX -= 1;
                    break;
                case "right":
                    deltaX += 1;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(direction));
            }

            snake.Body.Add(new SnakePoint
            {
                X = previousPoint.X + deltaX,
                Y = previousPoint.Y + deltaY,
            });

            await SetSnake(snake);
        }

        private DaprClient Client { get; }
        private readonly string StoreName = "statestore";

        private string SnakeKey { get; } = $"snake_key";

        public SnakeActor(ActorHost host, DaprClient daprClient)
            : base(host)
        {
            Client = daprClient;
        }
    }
}
