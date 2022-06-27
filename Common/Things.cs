using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public abstract class BaseEntity : ICloneable
    {
        public Guid Id { get; set; } = Guid.Empty;
        public Guid ContextId { get; set; } = Guid.Empty;
        public DateTime? DateTime { get; set; }
        public Guid[] Parents { get; set; } = new Guid[0];
        public string Name { get; set; } = "Noname";
        public override string ToString()
        {
            return $"{DateTime?.ToString("HH:mm:ss")} - {Name}";
        }

        public object Clone()
        {
            return new Stereotype()
            {
                Id = Guid.NewGuid(),
                ContextId = ContextId,
                DateTime = DateTime,
                Parents = Parents
            };
        }
    }

    public class Event : BaseEntity
    {
        public string? Arg { get; set; }
        public new object Clone()
        {
            return new Stereotype()
            {
                Id = Guid.NewGuid(),
                ContextId = ContextId,
                DateTime = DateTime,
                Parents = Parents,
                Arg = Arg
            };
        }
    }

    public class Stereotype : Event
    {
        public string? StereotypeName { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public List<string> ConfirmedProperties { get; set; } = new List<string>();
        public Dictionary<string, List<PointF>> UpperBounds { get; set; }
        public Dictionary<string, List<PointF>> LowerBounds { get; set; }
    }
}
