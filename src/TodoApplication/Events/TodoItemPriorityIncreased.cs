using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TodoApplication.Events
{
    public class TodoItemPriorityIncreased : IEvent
    {
        public Guid aggregateId { get; set; }

        public TodoItemPriorityIncreased(Guid aggregateId)
        {
            this.aggregateId = aggregateId;
        }

        public bool Equals(IEvent toCompare)
        {
            if (toCompare.GetType() == typeof(TodoItemPriorityIncreased))
            {
                TodoItemPriorityIncreased toCompareEvent = (TodoItemPriorityIncreased)toCompare;
                if (toCompareEvent.aggregateId == this.aggregateId)
                {
                        return true;
                }
            }
            return false;
        }

    }
}
