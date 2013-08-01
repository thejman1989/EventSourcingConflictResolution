using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemDeleted : IEvent
    {
        public Guid aggregateId { get; set; }

        public TodoItemDeleted(Guid AggregateId)
        {
            this.aggregateId = AggregateId;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemDeleted))
            {
                TodoItemDeleted toCompareEvent = (TodoItemDeleted)toCompare;
                {
                    if (toCompareEvent.aggregateId == this.aggregateId)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
