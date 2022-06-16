using System.Collections.Generic;

namespace Configuration.Actors.Contract;

public class Snake
{
    public List<SnakePoint> Body { get; set; } = new List<SnakePoint>();
}

public class SnakePoint
{
    public int X { get; set; }
    public int Y { get; set; }
}
