using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public interface IEvent
    {
        Guid aggregateId { get; set;}

        bool Equals(IEvent toCompare);

    }
}
