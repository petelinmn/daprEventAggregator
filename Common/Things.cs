using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.Empty;
    }

    public class Event : BaseEntity
    {
        public Guid ContextId { get; set; } = Guid.Empty;
        public Guid[] Parents { get; set; } = new Guid[0];
        public DateTime DateTime { get; set; }
        public string Name { get; set; }
        public string Arg { get; set; }
        public override string ToString()
        {
            return $"{DateTime.ToString("HH:mm:ss")} - {Name}"/* + " " + $"{Arg}"*/;
        }
    }
}
