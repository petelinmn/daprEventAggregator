
using Dapr;

namespace Common
{
    public class EventRequest
    {
        public Guid ContextId { get; set; } = Guid.Empty;
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid[] Parents { get; set; } = new Guid[0];
        public DateTime DateTime { get; set; } = DateTime.Now;
        public string? Name { get; set; }
        public string? Arg { get; set; }
    }
}
